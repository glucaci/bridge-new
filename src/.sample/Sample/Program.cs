namespace Bridge.Sample;

public class Program
{
    public static void Main(string[] args)
    {
        Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration(builder => builder.AddJsonFile("appsettings.user.json", true))
            .ConfigureLogging(builder => builder.AddConsole())
            .ConfigureWebHost(builder => builder.UseStartup<Startup>().UseKestrel())
            .Build()
            .Run();
    }
}
