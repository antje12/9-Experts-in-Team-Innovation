using Interfaces;
using Confluent.Kafka;
using Confluent.Kafka.SyncOverAsync;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;
using KafkaPlugin.DTOs;

namespace KafkaPlugin;

public class KafkaService : IKafkaPluginService
{
    //https://github.com/confluentinc/confluent-kafka-dotnet/blob/master/examples/AvroSpecific/Program.cs
    private CancellationTokenSource _cancellationTokenSource;
    private Task _consumeTask;
    private readonly ProducerConfig _producerConfig;
    private readonly ConsumerConfig _consumerConfig;
    private readonly SchemaRegistryConfig _schemaRegistryConfig;
    private readonly AvroSerializerConfig _avroSerializerConfig;
    private readonly List<Shower> _results;

    public KafkaService()
    {
        _producerConfig = new ProducerConfig
        {
            BootstrapServers = "localhost:19092"
        };
        _consumerConfig = new ConsumerConfig
        {
            BootstrapServers = "localhost:19092",
            GroupId = "foo",
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
        _results = new List<Shower>();
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
        _consumeTask = Task.Run(() => ConsumeLoop(_cancellationTokenSource.Token), _cancellationTokenSource.Token);
        return "Consuming started...";
    }

    private void ConsumeLoop(CancellationToken cancellationToken)
    {
        using (var schemaRegistry = new CachedSchemaRegistryClient(_schemaRegistryConfig))
        using (var consumer = new ConsumerBuilder<string, Shower>(_consumerConfig)
                   .SetValueDeserializer(new AvroDeserializer<Shower>(schemaRegistry).AsSyncOverAsync())
                   .SetErrorHandler((_, e) => Console.WriteLine($"Error: {e.Reason}"))
                   .Build())
        {
            consumer.Subscribe("my-topic");
            while (!_cancellationTokenSource.Token.IsCancellationRequested)
            {
                var consumeResult = consumer.Consume(_cancellationTokenSource.Token);
                var result = consumeResult.Message.Value;

                // Temp code for test
                // Put results in database using database service
                _results.Add(result);
                Thread.Sleep(1000);
            }

            consumer.Close();
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

        var results = _results.Select(x => x.DataRawId).ToList();
        return string.Join(", ", results);
    }
}