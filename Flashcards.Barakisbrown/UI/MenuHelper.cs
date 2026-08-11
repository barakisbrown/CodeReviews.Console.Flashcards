
using DataLayer.Controller;
using DataLayer.Models;
using Microsoft.SqlServer.Management.HadrModel;
using Spectre.Console;

namespace UI;

public static class MenuHelper
{
    private readonly static CardController CardController = new();
    private static StackController _stackController = new();
    
    public static Stack GetStackNamePrompt(List<Stack>? stackList)
    {
        const string title = $"  STACK";
        var prompt = new SelectionPrompt<Stack>()
            .Title(title)
            .UseConverter(s => $"[bold]{s.Name}[/]")
            .AddChoices<Stack>(stackList);
        return AnsiConsole.Prompt(prompt);
    }
    
    public static Card GetCardNamesPrompt(int stackID)
    {
        var list = CardController.GetAllCardsByStack(stackID);
        list.Add(new DataLayer.Models.Card
        {
            Answer = "CANCEL",
            Prompt = "EDIT"
        });

        var title = $"{"  Prompt",-15} {" Answer"}";
        
        var prompt = new SelectionPrompt<Card>()
            .Title(title)
            .UseConverter(s => $"{s.Prompt,-15} {s.Answer}")
            .AddChoices<Card>(list);
        return AnsiConsole.Prompt(prompt);
    }

    public static Stack GetStackListPrompt(List<Stack> stackList, string title)
    {
        var prompt = new SelectionPrompt<Stack>()
            .Title(title)
            .UseConverter(s => $"[bold]{s.Name}[/]")
            .AddChoices<Stack>(stackList);
        return AnsiConsole.Prompt(prompt);
    }

    public static string GetTextPrompt(string prompt)
    {
        var prmpt = new TextPrompt<string>(prompt)
                .AllowEmpty();
        return AnsiConsole.Prompt(prmpt);
    }   
}