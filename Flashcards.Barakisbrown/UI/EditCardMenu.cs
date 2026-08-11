using DataLayer.Controller;
using DataLayer.Models;
using Spectre.Console;

namespace UI;

public class EditCards(CardController cardController, StackController stackController)
{
    private const int SLEEP = 4000;
    private readonly CardController? _cardController = cardController;
    private readonly StackController _stackController = stackController;

    private CardController? CardCtr { get => _cardController; init => _cardController = value; }
    

    private StackController? StackCtr { get => _stackController; init => _stackController = value; }

    private enum Option { PROMPT = 'p', ANSWER = 'a', STACK = 's', WRITE = 'w', EXIT = 'x',NONE };

    public void Begin()
    {
        while (true)
        {
            AnsiConsole.Clear();
            AnsiConsole.WriteLine("EDITING FLASHCARD INFORMATION");
            // DISPLAY STACK TO GET FLASHCARD
            // DISPLAY LIST OF CARDS FROM STACK TO EDIT
            // ASK USER WHICH OF THE PARTS TO EDIT
            var stackList = StackCtr?.GetStackNames();
            AnsiConsole.WriteLine("Please Select Stack of Cards to find the Flash Card you would like to modify.");
            AnsiConsole.WriteLine();
            if (stackList != null)
            {
                var choice = MenuHelper.GetStackNamePrompt(stackList);
                if (choice.ID == 0)
                {
                    AnsiConsole.WriteLine("Thank you. No Changed will be made. Exiting to Menu.");
                    Thread.Sleep(SLEEP);
                    break;
                }
                EditLoop(choice);
            }
            // Will Add More
            AnsiConsole.WriteLine();
            AnsiConsole.Write("Press Any Key to Exit.");
            Console.ReadKey(true);
            break;
        }

    }

    private void EditLoop(Stack whichStack)
    {
        AnsiConsole.WriteLine();
        AnsiConsole.WriteLine($"Select Flash Card from Stack {whichStack.Name} that you would like to edit");
        var modify = MenuHelper.GetCardNamesPrompt(whichStack.ID);
        if (modify.ID == 0)
        {
            AnsiConsole.WriteLine("Thank you. No changed will be made. Exiting to Menu.");
            Thread.Sleep(SLEEP);
            return;
        }        
        while (true)
        {
            AnsiConsole.Clear();
            AnsiConsole.WriteLine("The Flash Card Selected");
            AnsiConsole.WriteLine($"Prompt => {modify.Prompt}");
            AnsiConsole.WriteLine($"Answer => {modify.Answer}");
            AnsiConsole.WriteLine($"StackName = {whichStack.Name}");
            // ASK FOR WHICH PART OF THE CARD TO BE MODIFIED 
            // OPTIONS ARE (P)rompt, (A)nswer, (S)tackname, (x)exit(no change made), (W)rite Changes
            AnsiConsole.WriteLine("Options are P)rompt / A)nswer / S)tack name / W)rite Changes / X)exit");
            AnsiConsole.MarkupLineInterpolated($"[bold]Make sure you write your chnages for it to be saved to disk.[/]");

            var nodifyOption = new TextPrompt<Char>("Which option do you want to modify")
                .AddChoice('p')
                .AddChoice('a')
                .AddChoice('s')
                .AddChoice('w')
                .AddChoice('x');

            var selection = AnsiConsole.Prompt(nodifyOption);
            switch (selection)
            {
                case (char)Option.PROMPT:
                    modify = Modify(modify, Option.PROMPT);
                    break;
                case (char)Option.ANSWER:
                    modify = Modify(modify, Option.ANSWER);
                    break;
                case (char)Option.STACK:
                    modify = Modify(modify, Option.STACK);
                    break;
                case (char)Option.WRITE:
                    var writeConfirm = AnsiConsole.Confirm("Saving Changes Warning..Do you wish to save these changes.");
                    if (writeConfirm)
                    {
                        AnsiConsole.WriteLine("Changes has been saved.");
                        Write(modify);                      
                    }
                    else
                    {
                        AnsiConsole.WriteLine("Changes have not been saved to disk.");
                    }
                    Thread.Sleep(SLEEP);
                    break;
                case (char)Option.EXIT:
                    AnsiConsole.WriteLine("No changes had been made. Exiting to Menu.");
                    return;
            }
        }
    }

    private Card Modify(Card _card, Option which)
    {
        var whichChange = which;
        // Modify function shoud not have these options selected.
        if (which == Option.WRITE || which == Option.EXIT)
        {
            AnsiConsole.WriteLine("Opps. Options selected values wrongly,");
            return _card;
        }
        // SHOW WHICH OPTION CAN BE MODIFIED
        AnsiConsole.WriteLine($"The Card {which} is being modified");
        if (which == Option.PROMPT)
        {
            AnsiConsole.WriteLine($"Prompt => {_card.Prompt}");
            var cancel = new TextPrompt<string>("Prompt => (No Changes)").AllowEmpty();
            var prompt = AnsiConsole.Prompt(cancel);
            if (prompt == String.Empty)
            {
                AnsiConsole.WriteLine("No change made for the Prompt.");
                whichChange = Option.NONE;
            }
            else
            {
                if (ConfirmChanges(Option.PROMPT, _card.Prompt, prompt))
                {
                    _card.Prompt = prompt;
                }
                else
                    whichChange = Option.NONE;
            }
                
        }
        else if (which == Option.ANSWER)
        {
            AnsiConsole.WriteLine($"Answer => {_card.Answer}");
            var cancel = new TextPrompt<string>("Answer => (No Changes)").AllowEmpty();
            var answer = AnsiConsole.Prompt(cancel);
            if (answer == String.Empty)
            {
                AnsiConsole.WriteLine("No change made for the Answer.");
                whichChange = Option.NONE;
            }
            else
            {
                if (ConfirmChanges(Option.ANSWER, _card.Answer, answer))
                {
                    _card.Answer = answer;                    
                }
                else
                    whichChange = Option.NONE;
            }
        }
        else if (which == Option.STACK)
        {
            var oldStackName = StackCtr?.GetStackNameById(_card.StackID);
            AnsiConsole.WriteLine($"Stack Name => {oldStackName}");
            var stackList = StackCtr?.GetStackNames();
            if (stackList != null)
            {
                var choice = MenuHelper.GetStackNamePrompt(stackList);
                if (choice.ID == 0)
                {
                    AnsiConsole.WriteLine("No change made for the Stack Name.");
                    whichChange = Option.NONE;
                }
                else
                {
                    var newStackName = StackCtr?.GetStackNameById(choice.ID);
                    AnsiConsole.WriteLine($"Stack Name is now => {newStackName}");
                    if (ConfirmChanges(Option.STACK, oldStackName, newStackName))
                    {                        
                        _card.StackID = choice.ID;
                    }
                }
            }
        }


        if (whichChange != Option.NONE)
            AnsiConsole.WriteLine($"Thank you. The Flsh Card {whichChange} was modified.");
        Thread.Sleep(SLEEP);
        return _card;
    }

    /// <summary>
    /// This actually does the Updat in the backend.
    /// </summary>
    /// <param name="_card"></param>
    /// <returns></returns>
    private bool Write(Card _card)
    {
        (bool edit, string message) = CardCtr.EditCard(_card);
        if (edit)
        {
            AnsiConsole.WriteLine("Congrats. Flash Card has been updated.");            
        }
        else
        {
            AnsiConsole.WriteLine($"Error has happened => {message}");
        }
        return edit;
    }

    private bool ConfirmChanges(Option which, string orig, string changed)
    {
        // Modify function shoud not have these options selected.
        if (which == Option.WRITE || which == Option.EXIT)
        {
            AnsiConsole.WriteLine("Opps. Options selected values wrongly,");
            return false;
        }

        AnsiConsole.WriteLine($"Flash Card Orignal {which} => {orig}\nFlash Card Changed {which} is {changed} ");
        var confirm = AnsiConsole.Confirm("Is this correct? (Y/N)");
        return confirm;
    }
}
