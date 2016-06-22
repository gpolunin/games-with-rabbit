using System;

namespace Consumer
{
	class Program
	{
		static void Main(string[] args)
		{
			using (var queuePool = new QueuePool("queue1"))
			using (var consumerPool = new ConsumerPool(10, queuePool))
			{
				consumerPool.Init();

				Console.WriteLine("Press enter to exit.");

				Console.ReadLine();
			}

		}
	}
}
