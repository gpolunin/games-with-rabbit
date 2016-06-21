using System;
using System.Text;
using System.Threading;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Consumer
{
	public class EventConsumer : IDisposable, IInitializable
	{
		private static int _consumerIncrement = 1;
		private static Random _random = new Random(DateTime.UtcNow.Millisecond);

		private int _id = _consumerIncrement++;
		private string _queueName;
		private bool _isInitialized;
		private IConnection _connection;
		private IModel _channel;
		private EventingBasicConsumer _consumer;

		public EventConsumer(string queueName)
		{
			_queueName = queueName;
			_isInitialized = false;
		}

		public void Init()
		{
			var connectionFactory = new ConnectionFactory() { HostName = "192.168.99.100" };
			_connection = connectionFactory.CreateConnection();
			_channel = _connection.CreateModel();

			_channel.QueueDeclare(queue: _queueName,
					 durable: false,
					 exclusive: false,
					 autoDelete: false,
					 arguments: null);


			_consumer = new EventingBasicConsumer(_channel);
			_consumer.Received += (model, ea) =>
			{
				var body = ea.Body;
				var message = Encoding.UTF8.GetString(body);
				Console.WriteLine($"Consumer with id {_id} started processing of {message}");
				Thread.Sleep(_random.Next(5000, 10000));
				_channel.BasicAck(ea.DeliveryTag, false);
				Console.WriteLine($"Consumer with id {_id} ended processing of {message}");
			};

			_channel.BasicConsume(queue: _queueName, noAck: false, consumer: _consumer);

			_isInitialized = false;
		}



		public void Dispose()
		{
			_channel.Dispose();
			_connection.Dispose();
		}
	}
}
