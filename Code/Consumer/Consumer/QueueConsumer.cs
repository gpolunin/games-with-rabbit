using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Consumer
{
	public class QueueConsumer : IDisposable, IInitializable
	{
		private static int _consumerIncrement = 1;
		private static Random _random = new Random(DateTime.UtcNow.Millisecond);

		private readonly int _id = _consumerIncrement++;
		private string _queueName;
		private bool _isInitialized = false;
		private readonly QueuePool _queuePool;
		private Task _task;

		public QueueConsumer(QueuePool queuePool)
		{
			_queuePool = queuePool;
		}

		public void Init()
		{
			if (_isInitialized)
			{
				return;
			}

			_task = new Task(StartProcessing, TaskCreationOptions.LongRunning);
			_task.Start();

			_isInitialized = true;
		}

		private void StartProcessing()
		{
			while (_isInitialized)
			{
				foreach (var queueName in _queuePool.QueueNames)
				{
					var channel = _queuePool.Channels[queueName];
					var result = channel.BasicGet(queueName, false);

					if (result == null)
					{
						continue;
					}

					var body = result.Body;
					var message = Encoding.UTF8.GetString(body);
					Console.WriteLine($"Consumer with id {_id} started processing of {message}");
					Thread.Sleep(_random.Next(5000, 10000));
					channel.BasicAck(result.DeliveryTag, false);
					Console.WriteLine($"Consumer with id {_id} ended processing of {message}");
				}
			}
		}


		public void Dispose()
		{
			_isInitialized = false;
			_task.Dispose();
		}
	}
}
