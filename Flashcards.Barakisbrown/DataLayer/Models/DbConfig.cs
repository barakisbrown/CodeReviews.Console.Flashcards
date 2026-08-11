namespace DataLayer.Models;

public class DbConfig
{
    public string DbName { get; set; } = string.Empty;
    public string StackTable { get; set; } = string.Empty;
    public string CardTable { get; set; } = string.Empty;
    public string StudiesTable { get; set; } = string.Empty;
    public string NotImplementedMsg { get; set; } = string.Empty;

    // SQL FILES BELOW
    public string CreateDBSql { get; set; } = string.Empty;
    public string CreateCardSql { get; set; } = string.Empty;
    public string CreateStackSql { get; set; } = string.Empty;   
}