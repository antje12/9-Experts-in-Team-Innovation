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
    private IHDFS_Service _hdfs;
    private ISqlDatabasePluginService _sql;
    private IMongoDatabasePluginService _mongo;
    private CancellationTokenSource _cancellationTokenSource;
    private readonly ProducerConfig _producerConfig;
    private readonly ConsumerConfig _consumerConfig;
    private readonly SchemaRegistryConfig _schemaRegistryConfig;
    private readonly AvroSerializerConfig _avroSerializerConfig;

    private const string KafkaServers = "kafka-1:9092,kafka-2:9092,kafka-3:9092";
    private const string GroupId = "KafkaPlugin";
    private const string SchemaRegistry = "http://schema-registry:8081";
    private const string DateFormat = "dd/MM/yyyy HH.mm.ss";
    private const int BatchSize = 100;

    public KafkaService(
        IHDFS_Service hdfs,
        ISqlDatabasePluginService sql,
        IMongoDatabasePluginService mongo
    )
    {
        this._hdfs = hdfs;
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
    }

    public string Test()
    {
        return "Tested!";
    }

    public string Status()
    {
        return "Collecting data: " + !_cancellationTokenSource?.IsCancellationRequested;
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
            result = producer.ProduceAsync("leak", new Message<string, Leak>
            {
                Key = leak.DataRaw_id,
                Value = leak
            }).Result.Value;
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
        var results = new List<T>();
        using (var schemaRegistry = new CachedSchemaRegistryClient(_schemaRegistryConfig))
        using (var consumer = new ConsumerBuilder<string, T>(_consumerConfig)
                   .SetValueDeserializer(new AvroDeserializer<T>(schemaRegistry).AsSyncOverAsync())
                   .SetErrorHandler((_, e) => Console.WriteLine($"Error: {e.Reason}"))
                   .Build())
        {
            consumer.Subscribe(topicName);
            while (!_cancellationTokenSource.Token.IsCancellationRequested)
            {
                var consumeResult = consumer.Consume(_cancellationTokenSource.Token);
                var result = consumeResult.Message.Value;
                results.Add(result);
                if (results.Count >= BatchSize)
                {
                    // Use switch to handle different types
                    switch (results)
                    {
                        case List<Leak> leaks:
                            await SaveData(leaks);
                            break;
                        case List<Shower> showers:
                            await SaveData(showers);
                            break;
                        default: throw new Exception("Invalid type");
                    }

                    results.Clear();
                }
            }

            consumer.Close();
        }
    }

    private async Task SaveData(List<Shower> results)
    {
        var showerData = results.Select(s => new ShowerSensorDataSimple()
        {
            DataRawId = int.Parse(s.DataRawId),
            //DCreated = DateTime.ParseExact(l.DCreated, DateFormat, null),
            DCreated = s.DCreated,
            //DReported = DateTime.ParseExact(l.DReported, DateFormat, null),
            DReported = s.DReported,
            SensorId = int.Parse(s.SensorId),
            //DShowerState = int.Parse(s.DShowerState),
            DShowerState = s.DShowerState,
            DTemperature = float.Parse(s.DTemperature),
            DHumidity = int.Parse(s.DHumidity),
            DBattery = int.Parse(s.DBattery)
        }).ToList();
        
        if (showerData.Any())
        {
            Console.WriteLine("Saved Data");
            Console.WriteLine(results.FirstOrDefault());
            Console.WriteLine(showerData.FirstOrDefault());
            await _hdfs.InsertShowerSensorDataAsync(showerData);
        }
        else
        {
            Console.WriteLine("Satans");
        }
    }

    private async Task SaveData(List<Leak> results)
    {
        var leakData = results.Select(l => new LeakSensorDataSimple()
        {
            DataRawId = int.Parse(l.DataRaw_id),
            //DCreated = DateTime.ParseExact(l.DCreated, DateFormat, null),
            DCreated = l.DCreated,
            //DReported = DateTime.ParseExact(l.DReported, DateFormat, null),
            DReported = l.DReported,
            DLifeTimeUseCount = int.Parse(l.DLifeTimeUseCount),
            LeakLevelId = int.Parse(l.LeakLevel_id),
            SensorId = int.Parse(l.Sensor_id),
            DTemperatureOut = float.Parse(l.DTemperatureOut),
            DTemperatureIn = float.Parse(l.DTemperatureIn)
        }).ToList();

        if (leakData.Any())
        {
            Console.WriteLine("Saved Data");
            Console.WriteLine(results.FirstOrDefault());
            Console.WriteLine(leakData.FirstOrDefault());
            await _hdfs.InsertLeakSensorDataAsync(leakData);
        }
        else
        {
            Console.WriteLine("Satans");
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

        return "Consuming ended...";
    }
}