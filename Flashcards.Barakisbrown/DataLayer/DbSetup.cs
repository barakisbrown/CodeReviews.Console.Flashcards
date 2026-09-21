namespace DataLayer;

using DataLayer.Models;
using Microsoft.Data.SqlClient;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using Spectre.Console;

/*
    * DataSource => SQLITE | SQLSERVER {EITHER CASE IT IS A FILE}
    * DbName => FlashCards
    * 3 Tables {Card / Stck / Study} -- Many Cards to 1 Stack Relation 
    * 1 VIEW which is virtual table of Stack and Card Data mostly for viewing
    * Scripts for the following:
    * Creation of all tables. All will be blank except Stack which record 1 is DEFAULT
    * Reading the data / Update Data / Deleting Data              
 */
public class DbSetup
{
    private readonly DbConfig appSettings = Configuration.LoadSettings();
    private readonly DbUser userSecrets = Configuration.GetConnectionStrings();
    private static bool IsSetup = false;


    /// <summary>
    /// Initializes a new instance of the DbSetup class and configures the main and backup database connection strings.
    /// </summary>
    /// <remarks>If the setup has not already been performed, this constructor initializes the setup process
    /// automatically. This ensures that the database connections are ready for use after instantiation.</remarks>
    public DbSetup()
    {
        
        if (!IsSetup)
            InitSetup();
    }
    
    /// <summary>
    /// Performs the initial setup for the application database, ensuring that the database and required tables exist.
    /// </summary>
    /// <remarks>This method checks for the existence of the database and the necessary tables. If they do not
    /// exist, it creates them. This method should be called before performing any operations that depend on the
    /// database being initialized.</remarks>
    private void InitSetup()
    {
        if (DbExist())
        {
            var card = TableExist(appSettings.CardTable);
            var stack = TableExist(appSettings.StackTable);
            var session = TableExist(appSettings.StudiesTable);
            var CardPerStackview = TableExist(appSettings.CardsPerStackView,ServerObjects.VIEWS);
            var AddNewSessionSp = TableExist(appSettings.AddNewSessionSp, ServerObjects.SP);
            var check1 = card && stack;
            var check2 = session && CardPerStackview && AddNewSessionSp;

            if (check1 && check2)
                IsSetup = true;
            else
            {
                if (!check1)
                {
                    if (!card)
                    {
                        ShowError("Card Table does not exist!");
                        CreateTable(appSettings.CreateCardSql);
                    }
                    if (!stack)
                    {
                        ShowError("Stack Table does not exist");
                        CreateTable(appSettings.CreateStackSql);
                    }
                }
                if (!check2)
                {
                    if (!session)
                    {
                        ShowError("Session Table does not exist!");
                        CreateTable(appSettings.CreateSessionSql);
                    }
                    if (!CardPerStackview)
                    {
                        ShowError("CardPerStack View does not exist");
                        CreateTable(appSettings.CreateCardsPerStackViewSql);
                    }
                    if (!AddNewSessionSp)
                    {
                        ShowError("AddNewSession Store Procedure Not found");
                        CreateTable(appSettings.AddNewSessionStoredProcedure);
                    }
                }
                AnsiConsole.WriteLine("Tables are now created.");
                Thread.Sleep(3000);
                IsSetup = true;
            }
        }
        else
        {
            AnsiConsole.WriteLine("FlashCard App Database Setup");
            AnsiConsole.WriteLine();
            CreateDB();
            AnsiConsole.WriteLine();
            AnsiConsole.WriteLine("Creating Tables");
            if (CreateTables())
                AnsiConsole.WriteLine("Tables Created Successfully");
            AnsiConsole.WriteLine("Exiting Initial Setup.");
            Thread.Sleep(3000);
            IsSetup = true;
        }
    }

    /// <summary>
    /// Determines whether the target database exists on the SQL Server instance.
    /// </summary>
    /// <remarks>This method attempts to open a connection to the SQL Server and checks for the existence of
    /// the database specified in the application settings. If the database does not exist or the server is unreachable,
    /// the method returns false.</remarks>
    /// <returns>true if the database exists; otherwise, false.</returns>
    private bool DbExist()
    {        
        using var conn = new SqlConnection(userSecrets.Main);
        try
        {
            conn.Open();
            // Connection is OPEN
            using var cmd = new SqlCommand();
            cmd.CommandText = "SELECT DB_ID(@DbName)";

            SqlParameter dbName = new("@DbName", System.Data.SqlDbType.NChar, appSettings.DbName.Length) { Value = appSettings.DbName };
            cmd.Parameters.Add(dbName);
            cmd.Connection = conn;
            cmd.Prepare();

            var Exist = cmd.ExecuteScalar();
            return !DBNull.Value.Equals(Exist);
        }
        catch (SqlException e)
        {
            AnsiConsole.WriteLine("ERROR : DATABASE FLASHCARDS DOES NOT EXIST...");
            // DATABASE DOES NOT EXIST HERE
            return false;
        }        
    }

    /// <summary>
    /// Determines whether a table with the specified name exists in the main database.
    /// </summary>
    /// <remarks>This method attempts to open a connection to the main database if it is not already open. If
    /// the database does not exist or cannot be accessed, a SqlException is thrown.</remarks>
    /// <param name="tableName">The name of the table to check for existence. Can be null or empty, in which case the method will return false.</param>
    /// <returns>true if a table with the specified name exists in the database; otherwise, false.</returns>
    private bool TableExist(string? tableName,ServerObjects objects = ServerObjects.TABLES)
    {
        using var conn = new SqlConnection(userSecrets.Main);
        var table = "@table,'U'";
        var view = "@table,'V'";
        var sp = "@table,'P'";
        try
        {
            conn.Open();
            using var cmd = new SqlCommand();
            cmd.Connection = conn;
            switch(objects)
            {
                case ServerObjects.TABLES:
                    cmd.CommandText = $"If Object_ID({table}) IS NOT NULL SELECT 1 ELSE SELECT 0";
                    break;
                case ServerObjects.VIEWS:
                    cmd.CommandText = $"If Object_ID({view}) IS NOT NULL SELECT 1 ELSE SELECT 0";
                    break;
                case ServerObjects.SP:
                    cmd.CommandText = $"If Object_ID({sp}) IS NOT NULL SELECT 1 ELSE SELECT 0";
                    break;
            }
            SqlParameter parm = new("@table", System.Data.SqlDbType.NChar, tableName.Length) { Value = tableName };
            cmd.Parameters.Add(parm);
            cmd.Prepare();

            int? result = cmd.ExecuteScalar() as int?;
            return result == 1;
        }
        catch (SqlException e)
        {
            AnsiConsole.WriteLine("ERROR : DATABASE FLASHCARDS DOES NOT EXIST..");
            throw;
        }
    }


    /// <summary>
    /// Executes a SQL script from the specified file against the database using the provided connection string.
    /// </summary>
    /// <remarks>If an error occurs while reading the script file or executing the script, the method writes
    /// error details to the console and returns false. The current working directory is set to the application's base
    /// directory before reading the script file.</remarks>
    /// <param name="fileName">The name of the SQL script file to execute. The file must exist in the 'Scripts' directory under the
    /// application's base directory.</param>
    /// <param name="connection">The connection string used to establish a connection to the target SQL Server database.</param>
    /// <returns>true if the script executes successfully; otherwise, false.</returns>
    private bool ExectureScript(string fileName,string connection)
    {
        Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);
        string root = Directory.GetCurrentDirectory();
        string script = File.ReadAllText(root + "\\Scripts\\" + fileName);

        using var conn = new SqlConnection(connection);
        Server server = new(new ServerConnection(conn));

        try
        {
            server.ConnectionContext.ExecuteNonQuery(script);

        }catch(Exception e)
        {
            AnsiConsole.WriteLine($"Error Processing Sql Script.  Script being used is {fileName}");
            AnsiConsole.WriteLine($"Excpetion Message that was caught is : \n{e.Message}");
            return false;
        }

        AnsiConsole.WriteLine($"{fileName} script was successfully executed.");
        return true;
    }

    /// <summary>
    /// Attempts to create the database using the configured SQL script and backup connection string.
    /// </summary>
    /// <returns>true if the database was created successfully; otherwise, false.</returns>
    private bool CreateDB()
    {
        bool success = ExectureScript(appSettings.CreateDBSql,userSecrets.Backup);
        return success;
    }

    /// <summary>
    /// Calls ExecuteScript for Stack Table and then Card Table AND Session Table AND CardsPerStack View
    /// </summary>
    /// <returns>True if both tables succeeded in creation. False Otherwise</returns>
    private bool CreateTables()
    {        
        bool stackSuccess = ExectureScript(appSettings.CreateStackSql,userSecrets.Main);
        bool cardSuccess = ExectureScript(appSettings.CreateCardSql,userSecrets.Main);
        bool sessionSuccess = ExectureScript(appSettings.CreateSessionSql, userSecrets.Main);
        bool cardStackViewSuccess = ExectureScript(appSettings.CreateCardsPerStackViewSql, userSecrets.Main);
        bool addSessionSP = ExectureScript(appSettings.AddNewSessionStoredProcedure, userSecrets.Main);
        
        
        return stackSuccess && cardSuccess && sessionSuccess && cardStackViewSuccess && addSessionSP;
    }

    private bool CreateTable(string sql)
    {
        bool completed = ExectureScript(sql, userSecrets.Main);
        return completed;
    }

    private static void ShowError(string message)
    {
        AnsiConsole.MarkupLineInterpolated($"[red]{message}[/]");
    }
}

enum ServerObjects
{
    TABLES,
    VIEWS,
    SP
};