using Microsoft.Data.SqlClient;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using Spectre.Console;

namespace Flashcards.Barakisbrown.Data;

public class DbSetup
{
    private readonly string? _dataSource;
    private readonly string? _dbName = Configuration.LoadSettings().DbName;
    private readonly string? _cardTableName = Configuration.LoadSettings().CardTableName;
    private readonly string? _stackTableName = Configuration.LoadSettings().StackTableName;
    private readonly string? _dbNameScript = Configuration.LoadSettings().InitDbScript;
    private readonly string? _cardScript = Configuration.LoadSettings().InitCardTableScript;
    private readonly string? _stackScript = Configuration.LoadSettings().InitStackTableScript;

    public DbSetup(string? dataSource)
    {
        _dataSource = dataSource;
    }

    public bool InitSetup()
    {
        AnsiConsole.WriteLine("Startup Msg .. Is the DB Created??");
        if (!DbExist())
        {
            AnsiConsole.WriteLine("Database does not exist..Creating.");
            bool db_success = CreateDB();
            AnsiConsole.WriteLine("Also Initializing Tables that will be used.");
            bool table_success = CreateTables();
            return db_success && table_success;
        }
        AnsiConsole.WriteLine("Rare Occasion DB is created but tables were not generated. Checking Now ..");
        if (TableExist(_cardTableName))
        {
            AnsiConsole.WriteLine("Tables where successfully created.");
            return true;
        }
        else
        {
            AnsiConsole.WriteLine("Database exist but tables didn't get created. So creating tables now.");
            return CreateTables();
        }
    }

    private bool CreateDB()
    {
        if (DbExist())
        {
            AnsiConsole.WriteLine("Database already exist so creating it is not needed.");
            return false;
        }
        else
        {
            AnsiConsole.WriteLine("Database does not exist so creating it now");
            bool success = ExecuteSqlScript(_dbNameScript);
            if (success)
            {
                AnsiConsole.WriteLine("Database was successfully created");
            }
            else
            {
                AnsiConsole.WriteLine("Database was not created due to issue");
            }
            return success;
        }
    }

    private bool CreateTables()
    {
        if (!TableExist(_cardTableName) && !TableExist(_stackTableName))
        {
            AnsiConsole.MarkupLineInterpolated($"Tables => {_cardTableName} and {_stackTableName} does not exist.");
            AnsiConsole.MarkupLineInterpolated($"Creating Tables {_cardTableName} and {_stackTableName}");
            // Stacks
            AnsiConsole.WriteLine("Stacks Table");
            bool stack_success = ExecuteSqlScript(_stackScript);
            if (stack_success)
            {
                AnsiConsole.WriteLine("Stacks Table Created Successfully");
            }
            else
            {
                AnsiConsole.WriteLine("Error Creating Stack Table.");
            }
            // Card
            AnsiConsole.WriteLine("Cards Table");
            bool card_success = ExecuteSqlScript(_cardScript);
            if (card_success)
            {
                AnsiConsole.WriteLine("Card Table Created Successfully");
            }
            else
            {
                AnsiConsole.WriteLine("Error Created Card Table.");
            }            
            if ((card_success && stack_success))
            {
                AnsiConsole.WriteLine("Database ans Tables are initialized.");
                return true;
            }
        }
        return false;
    }

    private bool TableExist(string? tableName)
    {
        using var conn = new SqlConnection(_dataSource);
        if (conn.State != System.Data.ConnectionState.Open)
            conn.Open();

        using var cmd = conn.CreateCommand();
        cmd.Connection = conn;
        cmd.CommandText = "IF OBJECT_ID(@table,'U') IS NOT NULL SELECT 1 ELSE SELECT 0;";
        SqlParameter parm = new("@table", System.Data.SqlDbType.NChar, tableName.Length) { Value = tableName };
        cmd.Parameters.Add(parm);
        cmd.Prepare();

        int? result = cmd.ExecuteScalar() as int?;
        return result == 1;
    }

    private bool DbExist()
    {
        using var conn = new SqlConnection(_dataSource);
        if (conn.State != System.Data.ConnectionState.Open)
            conn.Open();

        using var stmnt = new SqlCommand();
        stmnt.CommandText = "SELECT DB_ID(@DbName)";
        stmnt.Connection = conn;
        SqlParameter dbName = new("@DbName", System.Data.SqlDbType.NChar, _dbName.Length) { Value = _dbName };
        stmnt.Parameters.Add(dbName);
        stmnt.Prepare();

        return stmnt.ExecuteScalar() != DBNull.Value;
    }

    private bool ExecuteSqlScript(string fileName)
    {
        Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);
        string root = Directory.GetCurrentDirectory();
        string script = File.ReadAllText(root + "\\Sql_Scripts\\" + fileName);
        using var conn = new SqlConnection(_dataSource);
        Server server = new(new ServerConnection(conn));

        try
        {
            server.ConnectionContext.ExecuteNonQuery(script);
        }
        catch (Exception _ex)
        {
            AnsiConsole.WriteLine($"Error Processing Sql Script.  Script being used is {fileName}");
            AnsiConsole.WriteLine($"Excpetion Message that was caught is : \n{_ex.Message}");
            throw;
        }
        AnsiConsole.WriteLine($"{fileName} script was successfully executed.");
        return true;
    }
}
