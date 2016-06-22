using System;
using System.Collections.Generic;
using RabbitMQ.Client;

namespace Consumer
{
	public class QueuePool : IDisposable, IInitializable
	{
		private bool _isInitialized = false;
		private IConnection _connection;

		private readonly string[] _queueNames = null;
		private readonly Dictionary<string, IModel> _channels = new Dictionary<string, IModel>(10);

		public IReadOnlyDictionary<string, IModel> Channels => _channels;
		public IReadOnlyCollection<string> QueueNames => _queueNames;

		public QueuePool(params string[] queueName)
		{
			_queueNames = queueName;
		}

		public void Init()
		{
			if (_isInitialized)
			{
				return;
			}

			var connectionFactory = new ConnectionFactory() { HostName = "192.168.99.100" };
			_connection = connectionFactory.CreateConnection();

			var channel = _connection.CreateModel();

			foreach (var queueName in _queueNames)
			{
				channel.QueueDeclare(queue: queueName,
						 durable: false,
						 exclusive: false,
						 autoDelete: false,
						 arguments: null);

				_channels[queueName] = channel;
			}

			_isInitialized = true;
		}

		public void Dispose()
		{
			foreach (var channel in Channels.Values)
			{
				channel.Dispose();
			}
			_connection.Dispose();
			_isInitialized = false;
		}
	}
}
