namespace Flashcards.Barakisbrown.Model;

public class DbConfig
{
    public string InitDbScript { get; set; } = string.Empty;
    public string InitCardTableScript { get; set; } = string.Empty;
    public string InitStackTableScript { get; set;} = string.Empty;
    public string NotImplementedMsg { get; set; } = string.Empty;
    public string DbName { get; set; } = string.Empty;
    public string CardTableName { get; set; } = string.Empty;
    public string StackTableName { get; set; } = string.Empty;
}
