namespace DataLayer.Models.DTO;

public class SessionDTO
{
    public string StackName { get; set; } = string.Empty;
    public int Score { get; set; }
    public int TotalQuestions { get; set; }
    public DateTime Completed { get; set; }

    public double percentage => TotalQuestions == 0 ? 0 : (double)Score / TotalQuestions * 100;
}
