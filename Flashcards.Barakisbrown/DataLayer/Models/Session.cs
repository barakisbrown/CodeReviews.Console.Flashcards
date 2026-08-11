namespace DataLayer.Models;
/// <summary>
/// Session POCO Class
/// </summary>
/// <param name="ID">ID of the Study Session</param>
/// <param name="StudyDate">Date/Time the study was done</param>
/// <param name="StudyScore">The Score of the Study Session</param>
/// <param name="StackID">Foreign Key to the stack table</param>
public class Session
{
    public int ID { get; set; }
    public DateTime StudyDate { get; set; }
    public int StudyScore { get; set; }
    public int StackID { get; set; }
    public string StackName { get; set; } = string.Empty;
    public int NumQuestions { get; set; }
}
