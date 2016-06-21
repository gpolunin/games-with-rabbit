using System;

namespace Consumer
{
	class Program
	{
		static void Main(string[] args)
		{
			using (var consumer2 = new EventConsumer("hello"))
			using (var consumer1 = new EventConsumer("hello"))
			{
				consumer1.Init();
				consumer2.Init();
				Console.WriteLine("Press enter to exit.");

				Console.ReadLine();
			}

		}
	}
}
