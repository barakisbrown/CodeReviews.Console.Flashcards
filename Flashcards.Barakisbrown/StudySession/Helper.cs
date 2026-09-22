using Spectre.Console;
using DataLayer.Models;
namespace StudySession;

public static class Helper
{
    public static SelectionPrompt<Stack> GetStackNameList(List<Stack> stackList)
    {
        string title = Environment.NewLine + "Select Flash Card Stack to Study.";
        var prompt = new SelectionPrompt<Stack>()
            .Title(title)
            .UseConverter(s => $"[bold]{s.Name}[/]")
            .AddChoices<Stack>(stackList);
        return prompt;
    }

    public static int GetRandomFlashCardFromList(int totalCards)
    {
        var rnd = new Random();
        return rnd.Next(1, totalCards);
    }
}
