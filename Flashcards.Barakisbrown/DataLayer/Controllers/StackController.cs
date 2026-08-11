namespace DataLayer.Controller;

using Dapper;
using DataLayer.Models;
using DataLayer.Models.DTO;
using Microsoft.Data.SqlClient;

/// <summary>
/// MATT
/// FIRST CREATE A PARAM OBJECT
/// Use MakConnection and then Execute except QUERYS
/// </summary>
public class StackController
{
    private readonly static DbUser _dataSource = Configuration.GetUserSecretsConnStrings();
    private const string _selectSql = "SELECT * FROM dbo.Stack;";
    private const string _selectDisplaySql = "SELECT * FROM dbo.Stack WHERE Stack.Name != 'DEFAULT'";
    private const string _insertSql = "INSERT INTO dbo.Stack (Name) VALUES (@Name)";
    private const string _editSQL = "UPDATE dbo.Stack SET Name = @NewName WHERE Name = @OldName";
    private const string _deleteSQL = "DELETE FROM dbo.Stack WHERE Name = @Name";
    private const string _viewSql = "SELECT * FROM dbo.CardsPerStack";
    private List<Stack> _stacks = new List<Stack>();

    public StackController()
    {
        _stacks = GetAllStacks();
    }

    public (bool,bool) AddStack(string? Name)
    {
        bool added = false;
        bool unique = false;
        if (string.IsNullOrEmpty(Name)) return (added,unique);
        else
        {
            try
            {
                using var conn = MakeConnection;
                var rows = conn.Execute(_insertSql, new { Name });
                if (rows == 1)
                {
                    _stacks = GetAllStacks();
                    added = true;
                }
            }
            catch (SqlException cmd) when (cmd.Number == 2627)
            {
               
                added = false;
                unique = true;
            }
        }

        return (added,unique);
    }

    public bool EditStack(string? OrigName, string NewEdit)
    {
        if (string.IsNullOrEmpty(OrigName) || string.IsNullOrEmpty(NewEdit)) return false;

        using var conn = MakeConnection;
        Object[] parm = { new { NewName = NewEdit, OldName = OrigName } };
        if (conn.Execute(_editSQL, parm) == 1)
        {
            _stacks = GetAllStacks();
            return true;
        }

        return false;
    }
    
    public bool DeleteStack(string? deleteMe)
    {
        if (string.IsNullOrEmpty(deleteMe) || (deleteMe.ToLower().Contains("default"))) return false;
        using var conn = MakeConnection;
        var deleted = conn.Execute(_deleteSQL, new { Name = deleteMe }) == 1;
        if (deleted)
        {
            _stacks = GetAllStacks();
            return true;
        }
        return false;
    }


    public List<Stack> GetAllStacks()
    {
        using var conn = MakeConnection;
        return conn.Query<Stack>(_selectSql).ToList();

    }
    
    public string GetStackNameById(int id) => _stacks.First( x=> x.ID == id).Name;
    
    public int COUNT => _stacks.Count;
    private SqlConnection MakeConnection
    {
        get
        {
            var conn = new SqlConnection(_dataSource.Main);
            if (conn.State != System.Data.ConnectionState.Open)
                try
                {
                    conn.Open();
                }
                catch (SqlException)
                {
                    throw;
                }
            return conn;
        }
    }
    /// <summary>
    /// Retrieves a list of card stack totals as data transfer objects. 
    /// </summary>
    /// <returns>A list of <see cref="CardsPerStackDTO"/> objects representing the total cards per stack. The list will be
    /// empty if no stacks are found.</returns>
    public List<CardsPerStackDTO> StackTotalCardView()
    {
        using var conn = MakeConnection;
        return conn.Query<CardsPerStackDTO>(_viewSql).ToList();
        
    }
    /// <summary>
    /// Retrieves a list of stacks for display, including a special entry for returning to the menu and excluding
    /// the default stack.
    /// </summary>
    /// <returns>A list of stacks to be displayed. The list includes a 'EXIT WITHOUT CHANGES' entry and excludes
    /// the stack named 'DEFAULT'.</returns>
    public List<Stack> GetStackForDisplay(bool defaultOk = false)
    {
        var list = GetAllStacks().ToList();
        list.Add(new DataLayer.Models.Stack { Name = "EXIT WITHOUT CHANGES" });
        if (!defaultOk)
            list.Remove(list.Find(s => s.ID == 1));
        return list;
    }

    /// <summary>
    /// Retrieves a list of available stacks, including an option to return to the menu without making changes.
    /// </summary>
    /// <remarks>The additional entry allows callers to present a 'cancel' or 'no action' option in
    /// user interfaces that require stack selection.</remarks>
    /// <returns>A list of <see cref="Stack"/> objects representing all available stacks. The list always includes an
    /// additional entry labeled "EXIT WITHOUT CHANGES" as the last item.</returns>
    public List<Stack> GetStackNames()
    {
        var list = GetAllStacks();
        list.Add(new DataLayer.Models.Stack { Name = "EXIT WITHOUT CHANGES" });
        return list;
    }
}
