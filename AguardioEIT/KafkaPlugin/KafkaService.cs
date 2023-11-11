using Interfaces;
using Confluent.Kafka;
using Confluent.Kafka.SyncOverAsync;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;
using KafkaPlugin.DTOs;
using Avro.Specific;
using Common.Models;

namespace KafkaPlugin;

public class KafkaService : IKafkaPluginService
{
    //https://www.codeproject.com/Articles/5321450/ASP-NET-Core-Web-API-Plugin-Controllers-and-Servic
    //https://github.com/confluentinc/confluent-kafka-dotnet/blob/master/examples/AvroSpecific/Program.cs
    private ISqlDatabasePluginService _sql;
    private IMongoDatabasePluginService _mongo;
    private CancellationTokenSource _cancellationTokenSource;
    private readonly ProducerConfig _producerConfig;
    private readonly ConsumerConfig _consumerConfig;
    private readonly SchemaRegistryConfig _schemaRegistryConfig;
    private readonly AvroSerializerConfig _avroSerializerConfig;
    private readonly List<ISpecificRecord> _results;
    private readonly List<string> _errors;

    private const string KafkaServers = "kafka-1:9092,kafka-2:9092,kafka-3:9092";
    private const string GroupId = "KafkaPlugin";
    private const string SchemaRegistry = "http://schema-registry:8081";
    private const string DateFormat = "dd/MM/yyyy HH.mm.ss";

    public KafkaService(
        ISqlDatabasePluginService sql,
        IMongoDatabasePluginService mongo
    )
    {
        this._sql = sql;
        this._mongo = mongo;
        _producerConfig = new ProducerConfig
        {
            BootstrapServers = KafkaServers
        };
        _consumerConfig = new ConsumerConfig
        {
            BootstrapServers = KafkaServers,
            GroupId = GroupId,
            AutoOffsetReset = AutoOffsetReset.Earliest
        };
        _schemaRegistryConfig = new SchemaRegistryConfig
        {
            Url = SchemaRegistry
        };
        _avroSerializerConfig = new AvroSerializerConfig
        {
            BufferBytes = 100
        };
        _results = new List<ISpecificRecord>();
        _errors = new List<string>();
    }

    public string Test()
    {
        return "Tested!";
    }

    public string Status()
    {
        return "Messages collected: " + _results.Count + ", Collecting: " +
               !_cancellationTokenSource?.IsCancellationRequested + ", Errors: " + string.Join(", ", _errors);
    }

    public string Produce()
    {
        Leak result;
        using (var schemaRegistry = new CachedSchemaRegistryClient(_schemaRegistryConfig))
        using (var producer = new ProducerBuilder<string, Leak>(_producerConfig)
                   .SetValueSerializer(new AvroSerializer<Leak>(schemaRegistry, _avroSerializerConfig))
                   .Build())
        {
            var leak = new Leak()
            {
                DataRaw_id = "1337",
                DCreated = "31/08/2023 10.22.18",
                DReported = "31/08/2023 07.24.10",
                DLifeTimeUseCount = "2",
                LeakLevel_id = "5",
                Sensor_id = "22",
                DTemperatureOut = "15",
                DTemperatureIn = "17"
            };
            result = producer.ProduceAsync("leak-topic", new Message<string, Leak>
                {
                    Key = leak.DataRaw_id,
                    Value = leak
                }).Result
                .Value;
        }

        return result.DataRaw_id;
    }

    public string ConsumeStart()
    {
        _cancellationTokenSource = new CancellationTokenSource();
        var consumeShowerTask = Task.Run(() => ConsumeLoop<Shower>("shower", _cancellationTokenSource.Token),
            _cancellationTokenSource.Token);
        var consumeLeakTask = Task.Run(() => ConsumeLoop<Leak>("leak", _cancellationTokenSource.Token),
            _cancellationTokenSource.Token);
        return "Consuming started...";
    }

    private async Task ConsumeLoop<T>(string topicName, CancellationToken cancellationToken) where T : ISpecificRecord
    {
        using (var schemaRegistry = new CachedSchemaRegistryClient(_schemaRegistryConfig))
        using (var consumer = new ConsumerBuilder<string, T>(_consumerConfig)
                   .SetValueDeserializer(new AvroDeserializer<T>(schemaRegistry).AsSyncOverAsync())
                   .SetErrorHandler((_, e) => Console.WriteLine($"Error: {e.Reason}"))
                   .Build())
        {
            consumer.Subscribe(topicName + "-topic");
            while (!_cancellationTokenSource.Token.IsCancellationRequested)
            {
                var consumeResult = consumer.Consume(_cancellationTokenSource.Token);
                var result = consumeResult.Message.Value;

                _results.Add(result);
                await SaveData(result);
                Thread.Sleep(1000);
            }

            consumer.Close();
        }
    }

    private async Task SaveData<T>(T result) where T : ISpecificRecord
    {
        switch (result)
        {
            case Leak l:
                try
                {
                    var leak = new LeakSensorData()
                    {
                        DataRawId = Int32.Parse(l.DataRaw_id),
                        DCreated = DateTime.ParseExact(l.DCreated, DateFormat, null),
                        DReported = DateTime.ParseExact(l.DReported, DateFormat, null),
                        DLifeTimeUseCount = Int32.Parse(l.DLifeTimeUseCount),
                        LeakLevelId = Int32.Parse(l.LeakLevel_id),
                        SensorId = Int32.Parse(l.Sensor_id),
                        DTemperatureOut = Double.Parse(l.DTemperatureOut),
                        DTemperatureIn = Double.Parse(l.DTemperatureIn)
                    };
                    //await _sql.SaveSensorDataAsync(leak);
                    await _mongo.SaveSensorDataAsync(new List<LeakSensorData> { leak });
                }
                catch (Exception e)
                {
                    _errors.Add(e.Message);
                }

                break;
            case Shower s:
                // ToDo
                //var shower = new ShowerSensorData()
                //{
                //};
                //await _sql.SaveSensorDataAsync(shower);
                //await _mongo.SaveSensorDataAsync(shower);
                break;
            default: throw new Exception("Invalid type");
        }
    }

    public string ConsumeStop()
    {
        if (_cancellationTokenSource != null)
        {
            _cancellationTokenSource.Cancel();
            //_consumeTask.Wait(_cancellationTokenSource.Token); // Wait for the background task to complete
            //_cancellationTokenSource.Dispose();
        }

        var results = _results.Select(x => x.Get(0)).ToList();
        return string.Join(", ", results);
    }
}