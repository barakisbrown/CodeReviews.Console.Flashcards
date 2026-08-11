using Microsoft.Extensions.Configuration;
using DataLayer.Models;

namespace DataLayer;


/// <summary>
/// Class that loads the AppSettings into either of the following:
/// Strongly Types Class => DataLayer.Models.DbSetup
/// Returns the Database Configuration String
/// </summary>
public static class Configuration
{
    public static DbConfig LoadSettings()
    {
        Console.WriteLine("ACCESSED");
        var path = AppContext.BaseDirectory;
        
        var builder = new ConfigurationBuilder()
            .SetBasePath(path)
            .AddJsonFile("appsettings.json", false);

        IConfiguration configuration = builder.Build();

        return configuration.GetSection("AppKeys").Get<DbConfig>();
    }

    public static string GetConnectionStrings(string whichConnection)
    {
        var path = AppDomain.CurrentDomain.BaseDirectory;

        var builder = new ConfigurationBuilder()
            .SetBasePath(path)     
            .AddJsonFile("appsettings.json", false);

        IConfiguration configuration = builder.Build();

        return configuration.GetConnectionString(whichConnection);
    }

    public static DbUser GetUserSecretsConnStrings()
    {
        var path = Directory.GetCurrentDirectory();

        var builder = new ConfigurationBuilder()
            .SetBasePath(path)
            .AddJsonFile("appsettings.json", false)
            .AddUserSecrets<DbConfig>(true);

        IConfiguration config = builder.Build();

        return config.GetSection("ConnectionStrings").Get<DbUser>();            
    }
}