namespace Flashcards.Barakisbrown.Data;

using Flashcards.Barakisbrown.Model;
using Microsoft.Extensions.Configuration;


public static class Configuration
{
    private static string ConnStringName = "MyDB";

    public static DbConfig LoadSettings()
    {
        var path = AppDomain.CurrentDomain.BaseDirectory + "\\Properties";

        var builder = new ConfigurationBuilder()
            .SetBasePath(path)
            .AddJsonFile("appsettings.json", false);

        IConfiguration configuration = builder.Build();

        return configuration.GetSection("AppKeys").Get<DbConfig>();
    }

    public static string GetConnectionString()
    {
        var path = AppDomain.CurrentDomain.BaseDirectory + "\\Properties";

        var builder = new ConfigurationBuilder()
            .SetBasePath(path)
            .AddJsonFile("appsettings.json", false);

        IConfiguration configuration = builder.Build();

        return configuration.GetConnectionString(ConnStringName);
    }
}
