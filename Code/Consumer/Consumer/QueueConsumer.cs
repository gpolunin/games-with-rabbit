using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using RabbitMQ.Client;

namespace Consumer
{
	public class QueueConsumer : IDisposable, IInitializable
	{
		private static readonly TaskFactory _taskFactory = new TaskFactory(TaskCreationOptions.LongRunning,
			TaskContinuationOptions.None);
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

			_taskFactory.StartNew(() =>
			{
				try
				{
					StartProcessing();
				}
				catch (Exception e)
				{
					Console.WriteLine(e);
				}
			});

			_isInitialized = true;
		}

		private void StartProcessing()
		{
			while (_isInitialized)
			{
				foreach (var queueName in _queuePool.QueueNames)
				{
					var channel = _queuePool.Channels[queueName];
					BasicGetResult result = null; 

					try
					{
						result = channel.BasicGet(queueName, false);
					}
					catch (NotSupportedException){ }

					if (result == null)
					{
						Thread.Sleep(100);
						continue;
					}

					var body = result.Body;
					var message = Encoding.UTF8.GetString(body);
					Console.WriteLine($"Consumer with id {_id} started processing of {message} from {queueName}");
					Thread.Sleep(_random.Next(5000, 10000));

					channel.BasicAck(result.DeliveryTag, false);
					Console.WriteLine($"Consumer with id {_id} ended processing of {message} from {queueName}");
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
