using Jv.Plugins;
using Jv.Threading.Jobs;

namespace TestConsole
{
	class MainClass
	{
		static readonly Manager Plugins = new Manager();
		public static void Main(string[] args)
		{
			System.Console.WriteLine("Inicio");
			
			Pool<int> pool = new Pool<int>(4, Print);
			
			for(int i = 0; i < 9; i++)
				pool.Execute(i);
			
			System.Console.WriteLine("Fim");
			pool.ExitAndWait();
		}
		
		static void Print(int val)
		{
			if(val == 1 || val == 5)
				System.Threading.Thread.Sleep(5000);
			
			System.Console.WriteLine("{0}: {1}", Worker.CurrentWorker.Id, val);
			Plugins.MessageToPlugin<PLog>("{0}: {1}", Worker.CurrentWorker.Id, val);
		}
	}
}
