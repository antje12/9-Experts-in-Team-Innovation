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
  private CancellationTokenSource _cancellationTokenSource;
  private readonly ProducerConfig _producerConfig;
  private readonly ConsumerConfig _consumerConfig;
  private readonly SchemaRegistryConfig _schemaRegistryConfig;
  private readonly AvroSerializerConfig _avroSerializerConfig;

  private const string KafkaServers = "kafka-1:9092,kafka-2:9092,kafka-3:9092";
  private const string GroupId = "KafkaPlugin";
  private const string SchemaRegistry = "http://schema-registry:8081";
  private const int BatchSize = 100;

  public KafkaService(
      IHDFS_Service hdfs
  )
  {
    _hdfs = hdfs;
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
          Console.WriteLine("Batch collected");
          // Use switch to handle different types
          switch (results)
          {
            case List<Leak> leaks:
              Console.WriteLine("Of leak data");
              await SaveLeakData(leaks);
              break;
            case List<Shower> showers:
              Console.WriteLine("Of shower data");
              await SaveShowerData(showers);
              break;
            default: throw new Exception("Invalid type");
          }

          results.Clear();
        }
      }

      consumer.Close();
    }
  }

  private async Task SaveShowerData(List<Shower> results)
  {
    try
    {
      Console.WriteLine("SaveShowerData Called");
      var data = new List<ShowerSensorDataSimple>();
      foreach (var s in results)
      {
        var point = new ShowerSensorDataSimple()
        {
          DataRawId = int.Parse(s.DataRawId),
          //DCreated = DateTime.ParseExact(l.DCreated, DateFormat, null),
          DCreated = s.DCreated,
          //DReported = DateTime.ParseExact(l.DReported, DateFormat, null),
          DReported = s.DReported,
          SensorId = int.Parse(s.SensorId),
          //DShowerState = int.Parse(s.DShowerState),
          DShowerState = s.DShowerState,
          DTemperature = s.DTemperature,
          DHumidity = s.DHumidity,
          DBattery = s.DBattery
        };
        data.Add(point);
      }

      if (data.Any())
      {
        // Console.WriteLine("Data saved");
        await _hdfs.InsertShowerSensorDataAsync(data);
      }
      else
      {
        Console.WriteLine("Data not saved");
      }
    }
    catch (Exception e)
    {
      //Console.WriteLine("Fail!");
      Console.WriteLine(e);
      throw;
    }
  }

  private async Task SaveLeakData(List<Leak> results)
  {
    try
    {
      Console.WriteLine("SaveLeakData Called");
      var data = new List<LeakSensorDataSimple>();
      foreach (var l in results)
      {
        var point = new LeakSensorDataSimple()
        {
          DataRawId = int.Parse(l.DataRaw_id),
          //DCreated = DateTime.ParseExact(l.DCreated, DateFormat, null),
          DCreated = l.DCreated,
          //DReported = DateTime.ParseExact(l.DReported, DateFormat, null),
          DReported = l.DReported,
          DLifeTimeUseCount = l.DLifeTimeUseCount,
          LeakLevelId = int.Parse(l.LeakLevel_id),
          SensorId = int.Parse(l.Sensor_id),
          DTemperatureOut = l.DTemperatureOut,
          DTemperatureIn = l.DTemperatureIn
        };
        data.Add(point);
      }

      if (data.Any())
      {
        //Console.WriteLine("Data saved");
        await _hdfs.InsertLeakSensorDataAsync(data);
      }
      else
      {
        Console.WriteLine("Data not saved");
      }
    }
    catch (Exception e)
    {
      //Console.WriteLine("Fail!");
      Console.WriteLine(e);
      throw;
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