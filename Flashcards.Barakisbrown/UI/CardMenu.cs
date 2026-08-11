using DataLayer.Controller;
using DataLayer.Models;
using Spectre.Console;
using System.ComponentModel;

namespace UI;

public class CardMenu : IMenu
{
    private readonly CardController _cardController;
    private readonly StackController _stackController;
    private string[] _menu = [];

    public CardMenu(CardController cardController, StackController stackController)
    {
        _cardController = cardController;
        _stackController = stackController;
        LoadMenu(IMenu.CardMenuTitle);
    }

    public CardController CardCtr { get => _cardController; init => _cardController = value; }
    public StackController StackCtr { get => _stackController; init => _stackController = value; }

    public void DisplayMenu()
    {
        while (true)
        {
            AnsiConsole.Clear();
            AnsiConsole.Write(new FigletText("Flash Card Menu").Color(Color.Green));
            AnsiConsole.WriteLine($"Number of Cards created currently {CardCtr.Count}");
            AnsiConsole.WriteLine();
            AnsiConsole.WriteLine();

            var choice = AnsiConsole.Prompt(new SelectionPrompt<string>()
                .Title("Select an option").AddChoices(_menu));

            switch (choice)
            {
                case "View":
                    ListAllCards();
                    AnsiConsole.WriteLine("Press any key to return to menu.");
                    Console.ReadKey(true);
                    continue;
                case "Add":
                    AddFlashCard();
                    continue;
                case "Edit":
                    var editCards = new EditCards(_cardController, _stackController);
                    editCards.Begin();
                    continue;
                case "Delete":
                    DeleteCard();
                    continue;
                case "Exit": break;
            }
            break;
        }
    }
    public void LoadMenu(string menuName)
    {
        var path = AppDomain.CurrentDomain.BaseDirectory;
        path += "//MENUS//";

        _menu = File.ReadAllLines(Path.Combine(path, menuName));
    }

    private void ListAllCards()
    {
        var cards = CardCtr.GetAllCards();
        var stacks = StackCtr.GetAllStacks();
        var stackLookup = stacks.ToDictionary(s => s.ID, s => s.Name);

        var table = new Table().AddColumns("Prompt", "Answer", "Stack");
        foreach (var c in cards)
        {
            var stackName = stackLookup.GetValueOrDefault(c.StackID, "DEFAULT");
            table.AddRow(c.Prompt ?? string.Empty, c.Answer ?? string.Empty, stackName);
        }

        AnsiConsole.Write(table);
    }

    /// <summary>
    /// Allows the user to enter a new FlashCard into the system. Stack defaults to DEFAULT.
    /// 
    /// </summary>
    private void AddFlashCard()
    {
        var stackList = _stackController.GetAllStacks();

        while (true)
        {
            AnsiConsole.Clear();
            AnsiConsole.WriteLine("ADDING NEW FLASHCARDS");
            AnsiConsole.WriteLine();
            AnsiConsole.WriteLine("To create a new FlashCard, you will need to enter a prompt followed by the answer.");
            AnsiConsole.WriteLine("You can assign to a Stack here unless you want it to be attached the DEFAULT stack.");
            AnsiConsole.WriteLine("You can change the stacks name of the flashcard later if needed.");

            var cancel = new TextPrompt<string>("Prompt => (HIT RETURN TO EXIT)").AllowEmpty();

            var prompt = AnsiConsole.Prompt(cancel);
            if (string.IsNullOrEmpty(prompt))
            {
                break;
            }
            var answer = AnsiConsole.Ask<string>("Answer => ");
            // DOH - Need to confirm if prompt and answer is correct
            Card newCard = new Card { Prompt = prompt, Answer = answer, StackID = 0 };
            // ASSIGN IT TO THE PROPER STACK EVEN DEFAULT
            var stack = new SelectionPrompt<Stack>()
           .Title("Select Stack Name to Add Card")
           .UseConverter(s => $"[bold]{s.Name}[/]")
           .AddChoices<Stack>(stackList);

            var selStack = AnsiConsole.Prompt(stack);
            newCard.StackID = selStack.ID;
            // CHECK FOR DUPLICATE CARD FROM THE SAME STACK AND ASK THE USER IF HE WANTS 
            // TO STILL KEEP IT OR DISCARD
            if (CardCtr.CardExist(newCard))
            {
                var confirmCard = AnsiConsole.Confirm("This card already exist in this stack. Do you still want to add it anyway?");
                if (!confirmCard)
                    continue;
            }


            var (success, another) = AddCard(newCard);
            if (success && another)
            {
                if (success == true && another == true)
                    continue;
                else
                    break;
            }
            else
            {
                break;
            }
        }
    }
    
    /// <summary>
    /// Attempts to add a new flashcard to the system and prompts the user to add another card if the operation
    /// succeeds.
    /// </summary>
    /// <remarks>If the card is added successfully, the method prompts the user to confirm whether to add
    /// another card. If the addition fails, an error message is displayed and the method returns (<see
    /// langword="false"/>, <see langword="false"/>).</remarks>
    /// <param name="newCard">The flashcard to add to the system. Cannot be null.</param>
    /// <returns>A tuple indicating the result of the operation. The first value is <see langword="true"/> if the card was added
    /// successfully; otherwise, <see langword="false"/>. The second value is <see langword="true"/> if the user chooses
    /// to add another card; otherwise, <see langword="false"/>.</returns>
    private (bool, bool) AddCard(Card newCard)
    {
        var (success,message) = CardCtr.AddCard(newCard);
        if (success)
        {
            var name = StackCtr.GetStackNameById(newCard.StackID);
            AnsiConsole.WriteLine($"Congrats .. Successfully added a new flashcard into the system to the {name} stack.");
            var another = AnsiConsole.Confirm("Do you want to add an another FlashCard? (Y/N) ");
            if (another)
            {
                return (true, true);
            }
            else
            {
                AnsiConsole.WriteLine("Redirecting to the Card Menu.");
                Thread.Sleep(IMenu.SLEEP);
                return (true, false);
            }
        }
        else
        {
            AnsiConsole.MarkupLineInterpolated($"[red]Error Adding a new FlashCard. Check with Administrator.[/]");
            AnsiConsole.MarkupLineInterpolated($"Exception Used => {message}");
            Thread.Sleep(IMenu.SLEEP);
            return (false, false);
        }
    }


    private void DeleteCard()
    {       
        while(true)
        {
            AnsiConsole.Clear();
            AnsiConsole.WriteLine("DELETE CARD AREA");
            AnsiConsole.MarkupLineInterpolated($"[blink red]PRECEED WITH CAUTION.[/]");
            AnsiConsole.WriteLine();
            var stackList = StackCtr?.GetStackNames();
            AnsiConsole.WriteLine("Please Select Stack of Cards to find the Card that needs to be modified.");
            AnsiConsole.WriteLine();
            while(stackList != null)
            {
                var choice = MenuHelper.GetStackNamePrompt(stackList);
                if (choice.ID == 0)
                {
                    AnsiConsole.WriteLine("Thank you. No Changed will be made. Exiting to Menu.");
                    Thread.Sleep(IMenu.SLEEP);
                    return;
                }
                AnsiConsole.WriteLine();
                AnsiConsole.WriteLine($"Select FlashCard from {choice.Name} stack to edit");
                var modify = MenuHelper.GetCardNamesPrompt(choice.ID);
                if (modify.StackID == 0)
                {
                    AnsiConsole.WriteLine("No Flash Card was selected.");
                    Thread.Sleep(IMenu.SLEEP);
                    break;
                }
                AnsiConsole.WriteLine("The Flash Card Selected TO BE DELETED");
                AnsiConsole.WriteLine($"Prompt => {modify.Prompt}");
                AnsiConsole.WriteLine($"Answer => {modify.Answer}");
                AnsiConsole.WriteLine($"StackName = {choice.Name}");
                // CONFIRMATION
                var confirm = AnsiConsole.Confirm("Do you wish to delete this Flash Card? IT CAN NOT BE UNDONE!. (Y/N)");
                if (confirm)
                {
                    var (deleted,message) = CardCtr.DeleteCard(modify);
                    if (deleted)
                    {
                        AnsiConsole.WriteLine("Flash Card was deleted!");
                        var another = AnsiConsole.Confirm("Do you wish to delete another Flash Card (Y/N)");
                        if (another)
                        {
                            Thread.Sleep(1000);
                            break;
                        }
                        else
                        {
                            Thread.Sleep(IMenu.SLEEP);
                            return;
                        }
                    }
                    else
                    {
                        AnsiConsole.MarkupLineInterpolated($"[RED]CARD CAN NOT BE DELETED.  {message}[/]");
                        AnsiConsole.MarkupInterpolated($"[RED]Please contact Admin[/]");
                        Thread.Sleep(IMenu.SLEEP);
                        return;
                    }    

                }
                else
                {
                    AnsiConsole.WriteLine("Flash Card Selected will not be deleted.");
                    Thread.Sleep(1000);
                    break;
                }
            }
        }       
    }
}