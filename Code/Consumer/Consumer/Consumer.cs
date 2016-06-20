using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Consumer
{
	public class Consumer : IDisposable
	{
		private string _queueName;
		private bool _isInitialized;
		private IConnection _connection;
		private IModel _channel;
		private EventingBasicConsumer _consumer;

		public Consumer(string queueName)
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
				Console.WriteLine(" [x] Received {0}", message);
			};

			_channel.BasicConsume(queue: _queueName, noAck: true, consumer: _consumer);

		}



		public void Dispose()
		{
			_channel.Dispose();
			_connection.Dispose();
		}
	}
}
