using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Consumer
{
	class Program
	{
		static void Main(string[] args)
		{
			using (var consumer = new Consumer("hello"))
			{
				consumer.Init();
				Console.WriteLine("Press enter to exit.");

				Console.ReadLine();
			}

		}
	}
}
