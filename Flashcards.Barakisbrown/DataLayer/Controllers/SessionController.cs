namespace DataLayer.Controller;

using Dapper;
using DataLayer.Models;
using DataLayer.Models.DTO;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Reflection;

public class SessionController
{
    private readonly StackController _stack = new();
    private readonly string _tableName = "dbo.Sessions";
    private readonly static DbUser _dataSource = Configuration.GetUserSecretsConnStrings();
    private readonly string _selectSQL;
    private readonly string _insertSQL;
    private List<Session> _sessions = new();

    public SessionController(StackController stack)
    {
        _selectSQL = "SELECT * FROM " + _tableName;
        _insertSQL = "INSERT INTO " + _tableName + " (StudyDate,StudyScore,StackID,StackName,NumQuestions)";
        _insertSQL += "values (@StudyDate,@StudyScore,@StackID,@StackName,@NumQuestions)";
        _sessions = GetAllSessions();
        _stack = stack;
    }

    /// <summary>
    /// Selects all Session data and returns it
    /// </summary>
    /// <returns>List of Session Data</returns>
    public List<Session> GetAllSessions()
    {
        using var conn = MakeConnection;
        return conn.Query<Session>(_selectSQL).ToList();
    }

    /// <summary>
    /// Propery that returns a SqlConnection
    /// </summary>
    private SqlConnection MakeConnection
    {
        get
        {
            var conn = new SqlConnection(_dataSource.Main);
            if (conn.State != System.Data.ConnectionState.Open)
            {
                try { conn.Open(); } catch (SqlException) { throw; }                   
            }
            return conn;
        }
    }
    /// <summary>
    /// Add(Insert) a new Session Data[datetime and score]
    /// </summary>
    /// <param name="data">Datetime and score of a paticular session</param>
    /// <returns>added,message => added is true then message is blank else message will be the exception message</returns>
    public (bool,string) AddSession(Session data)
    {
        string message = string.Empty;
        bool added = false;
        if (string.IsNullOrEmpty(data.StackName)) added = false;
        else
        {
            try
            {                
                using var conn = MakeConnection;
                DynamicParameters param = new();
                param.Add("StudyDate", data.StudyDate,direction: ParameterDirection.Input);
                param.Add("StudyScore", data.StudyScore, direction: ParameterDirection.Input);
                param.Add("StackID", data.StackID, direction: ParameterDirection.Input);
                param.Add("StackName", data.StackName, direction: ParameterDirection.Input);
                param.Add("NumQuestions", data.NumQuestions, direction: ParameterDirection.Input);
                param.Add("RowsInserted", dbType: DbType.Int32, direction: ParameterDirection.Output);
                conn.Execute("AddNewSession", param, commandType: CommandType.StoredProcedure);
                int rows = param.Get<int>("RowsInserted");
                if (rows == 1)
                {
                    added = true;
                    _sessions = GetAllSessions();
                }
            }
            catch (SqlException cmd)
            {
                added = false;
                message = cmd.Message;
            }
        }
        return (added,message);
    }

    public (bool,string) AddSession(SessionDTO Data,int StackID)
    {
        Session data = new()
        {
            StackName = Data.StackName,
            StudyDate = Data.Completed,
            NumQuestions = Data.TotalQuestions,
            StudyScore = Data.Score,
            StackID = StackID
        };
        return AddSession(data);
    }

    /// <summary>
    /// Returns how many session have been logged in the system
    /// </summary>
    public int COUNT => _sessions.Count;
    
    public IReadOnlyList<SessionDTO> GetUserSessionData()
    {
        _sessions = GetAllSessions();
        var dTOs  = new List<SessionDTO>();

        foreach(var single in _sessions)
        {
            var one = new SessionDTO
            {
                StackName = single.StackName,
                TotalQuestions = single.NumQuestions,
                Completed = single.StudyDate,
                Score = single.StudyScore
            };
            dTOs.Add(one);
        }

        return dTOs;
    }

}
