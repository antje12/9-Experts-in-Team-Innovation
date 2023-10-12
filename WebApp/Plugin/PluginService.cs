using Interfaces;
using Confluent.Kafka;

namespace Plugin
{
    public class PluginService : IPluginService
    {
        private CancellationTokenSource _cancellationTokenSource;
        private Task _consumeTask;
        private readonly List<string> _results;

        public PluginService()
        {
            _results = new List<string>();
        }
        
        public string Test()
        {
            return "Tested!";
        }
        
        public string Status()
        {
            return "Messages collected: " + _results.Count + ", Collecting: " + !_cancellationTokenSource?.IsCancellationRequested;
        }

        public string Produce()
        {
            var config = new ProducerConfig
            {
                BootstrapServers = "localhost:29092"
            };

            var result = "";
            using (var producer = new ProducerBuilder<Null, string>(config).Build())
            {
                result = producer.ProduceAsync("my-topic", new Message<Null, string> { Value = "hello world" }).Result
                    .Value;
            }

            return result;
        }

        public string ConsumeStart()
        {
            _cancellationTokenSource = new CancellationTokenSource();
            _consumeTask = Task.Run(() => ConsumeLoop(_cancellationTokenSource.Token), _cancellationTokenSource.Token);
            return "Consuming started...";
        }

        private void ConsumeLoop(CancellationToken cancellationToken)
        {
            var config = new ConsumerConfig
            {
                BootstrapServers = "localhost:19092",
                GroupId = "foo",
                AutoOffsetReset = AutoOffsetReset.Earliest
            };

            var result = "";
            using (var consumer = new ConsumerBuilder<Ignore, string>(config).Build())
            {
                consumer.Subscribe("my-topic");
                while (!_cancellationTokenSource.Token.IsCancellationRequested)
                {
                    var consumeResult = consumer.Consume(_cancellationTokenSource.Token);
                    result = consumeResult.Message.Value;
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
            var result = string.Join(", ", _results);
            return result;
        }
    }
}