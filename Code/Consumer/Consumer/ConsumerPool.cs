using System;
using System.Collections.Generic;

namespace Consumer
{
	public class ConsumerPool : IDisposable, IInitializable
	{
		private bool _isInitialized = false;
		private readonly QueuePool _queuePool;
		private readonly int _countOfConsumers = 0;
		private readonly List<QueueConsumer> _queueConsumers = new List<QueueConsumer>(10);

		public ConsumerPool(int countOfConsumers, QueuePool queuePool)
		{
			_queuePool = queuePool;
			_countOfConsumers = countOfConsumers;
		}

		public void Dispose()
		{
			_queueConsumers.ForEach(c => c.Dispose());
			_queueConsumers.Clear();
			_isInitialized = false;
		}

		public void Init()
		{
			if (_isInitialized)
			{
				return;
			}

			_queuePool.Init();
			for (var i = 0; i < _countOfConsumers; i++)
			{
				var consumer = new QueueConsumer(_queuePool);
				consumer.Init();
				_queueConsumers.Add(consumer);
			}

			_isInitialized = true;
		}
	}
}
