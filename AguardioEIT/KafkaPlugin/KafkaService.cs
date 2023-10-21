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
    private ILeakSensorSqlDatabasePluginService _sql;
    private ILeakSensorMongoDatabasePluginService _mongo;
    private CancellationTokenSource _cancellationTokenSource;
    private readonly ProducerConfig _producerConfig;
    private readonly ConsumerConfig _consumerConfig;
    private readonly SchemaRegistryConfig _schemaRegistryConfig;
    private readonly AvroSerializerConfig _avroSerializerConfig;
    private readonly List<ISpecificRecord> _results;

    private string _dateFormat = "dd/MM/yyyy HH.mm.ss";
    
    public KafkaService(
        ILeakSensorSqlDatabasePluginService sql, 
        ILeakSensorMongoDatabasePluginService mongo)
    {
        this._sql = sql;
        this._mongo = mongo;
        _producerConfig = new ProducerConfig
        {
            BootstrapServers = "localhost:19092,localhost:29092,localhost:39092"
        };
        _consumerConfig = new ConsumerConfig
        {
            BootstrapServers = "localhost:19092,localhost:29092,localhost:39092",
            GroupId = "KafkaPlugin",
            AutoOffsetReset = AutoOffsetReset.Earliest
        };
        _schemaRegistryConfig = new SchemaRegistryConfig
        {
            Url = "localhost:8081"
        };
        _avroSerializerConfig = new AvroSerializerConfig
        {
            BufferBytes = 100
        };
        _results = new List<ISpecificRecord>();
    }

    public string Test()
    {
        return "Tested!";
    }

    public string Status()
    {
        return "Messages collected: " + _results.Count + ", Collecting: " +
               !_cancellationTokenSource?.IsCancellationRequested;
    }

    public string Produce()
    {
        Shower result;
        using (var schemaRegistry = new CachedSchemaRegistryClient(_schemaRegistryConfig))
        using (var producer = new ProducerBuilder<string, Shower>(_producerConfig)
                   .SetValueSerializer(new AvroSerializer<Shower>(schemaRegistry, _avroSerializerConfig))
                   .Build())
        {
            var shower = new Shower()
            {
                DataRawId = "43646",
                DCreated = "31/08/2023  10.22.18",
                DReported = "31/08/2023  07.24.10",
                SensorId = "529",
                DShowerState = "NULL",
                DTemperature = "21,48",
                DHumidity = "68",
                DBattery = "100"
            };
            result = producer.ProduceAsync("shower-topic", new Message<string, Shower>
                {
                    Key = shower.DataRawId,
                    Value = shower
                }).Result
                .Value;
        }

        return result.DataRawId;
    }

    public string ConsumeStart()
    {
        _cancellationTokenSource = new CancellationTokenSource();
        var consumeShowerTask = Task.Run(() => ConsumeLoop<Shower>("shower", _cancellationTokenSource.Token), _cancellationTokenSource.Token);
        var consumeLeakTask = Task.Run(() => ConsumeLoop<Leak>("leak", _cancellationTokenSource.Token), _cancellationTokenSource.Token);
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
                var leak = new LeakSensorData()
                {
                    DataRawId = Int32.Parse(l.DataRaw_id),
                    DCreated = DateTime.ParseExact(l.DCreated, _dateFormat, null),
                    DReported = DateTime.ParseExact(l.DReported, _dateFormat, null),
                    DLifeTimeUseCount =  Int32.Parse(l.DLifeTimeUseCount),
                    LeakLevelId =  Int32.Parse(l.LeakLevel_id),
                    SensorId =  Int32.Parse(l.Sensor_id),
                    DTemperatureOut =  Double.Parse(l.DTemperatureOut),
                    DTemperatureIn = Double.Parse(l.DTemperatureIn)
                };
                await _sql.SaveSensorDataAsync(leak);
                await _mongo.SaveSensorDataAsync(leak);
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