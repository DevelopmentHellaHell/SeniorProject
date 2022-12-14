namespace DevelopmentHell.Hubba.Client
{
	public class Program
	{
		public async static Task Main(string[] args)
		{
			var app = new ViewDemoConsole();
			await app.Run();
		}
	}
}