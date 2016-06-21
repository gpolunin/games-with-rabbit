using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
			_isInitialized = false;
		}

		public void Init()
		{
			for (var i = 0; i < _countOfConsumers; i++)
			{
				_queueConsumers.Add(new QueueConsumer(_queuePool));
			}

			_isInitialized = true;
		}
	}
}
