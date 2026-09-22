namespace StudySession;

using DataLayer.Controller;
using DataLayer.Models;
using DataLayer.Models.DTO;
using Spectre.Console;

public class Sessions(CardController _card,StackController _stack)
{
    private readonly SessionController _sessionCtr = new(_stack);
    private const string StudySessionTItle = "StudySession-Menu.txt";
    private string[] _menu;

    private void LoadMenu()
    {
        var path = AppDomain.CurrentDomain.BaseDirectory;
        path += "//MENUS//";

        _menu = File.ReadAllLines(Path.Combine(path, StudySessionTItle));
    }

    public void DisplayMenu()
    {
        LoadMenu();
        while(true)
        {
            AnsiConsole.Clear();
            AnsiConsole.Write(new FigletText("Study Session Menu").Color(Color.Green));

            var choice = AnsiConsole.Prompt(new SelectionPrompt<string>()
                .Title("Select an option").AddChoices(_menu));
            switch(choice)
            {
                case "NEW":
                    NewSession();
                    break;
                case "HISTORY":
                    DisplayUserStats();
                    break;
                case "EXIT":
                    return;
            }
        }
    }
    
    private Dictionary<int,CardDTO> DisplayCardList(List<CardDTO> _cards)
    {
        var dict = new Dictionary<int, CardDTO>();
        int counter = 1;
        foreach(var single in _cards)
        {
            dict.Add(counter++, single);
        }
        return dict;
    }

    private void DisplayTable(Dictionary<int, CardDTO> Cards,string StackName)
    {
        var table = new Table();
        table.Title(StackName);
        table.AddColumn("Question #");
        table.AddColumn("Prompt");
        table.AddColumn("Answer");
        foreach (var (key, value) in Cards)
        {
            table.AddRow(key.ToString(), value.Front, value.Back);
        }
        // CREATE AN EXIT POINT TO BE USED        
        AnsiConsole.Write(table);
    }


    private void DisplaySessionStats(Dictionary<int,CardDTO> Cards,string StackName,int Qasked, int Qcorrect)
    {
        AnsiConsole.Clear();
        var table = new Table();
        table.Title($"Study Session Results for Stack {StackName}");
        table.AddColumn("Question #");
        table.AddColumn("Prompt");
        table.AddColumn("Answer");

        foreach (var (key,value) in Cards)
        {
            table.AddRow(key.ToString(), value.Front, value.Back);
        }
        AnsiConsole.Write(table);
        AnsiConsole.WriteLine();
        AnsiConsole.WriteLine($"Number of Questions Asked => {Qasked}");
        AnsiConsole.WriteLine($"Number of Questions Correct => {Qcorrect}");

        double score = (double)Qcorrect / Qasked;
        double percentage = score * 100.0;
        AnsiConsole.WriteLine($"The percentage for this session => {percentage}% ");
        AnsiConsole.WriteLine("Press any key to write this to the backend");
        Console.ReadKey(true);
    }



    private void NewSession()
    {
        while(true)
        {
            AnsiConsole.Clear();
            AnsiConsole.WriteLine("Lets start a new study session");            

            var stackList = _stack.GetStackForDisplay(true);
            var choice = AnsiConsole.Prompt(Helper.GetStackNameList(stackList));
                        
            if (choice.ID != 0)
            {
                var cardDTO = _card.DisplayCardsByStack(choice.ID);
                var stackName = choice.Name;
                var cards = DisplayCardList(cardDTO);
                int totalCards = cards.Count;
                int totalCorrect = 0;
                int totalAsked = 0;
                while (true)
                {
                    AnsiConsole.Clear();                    
                    DisplayTable(cards,stackName);
                    AnsiConsole.WriteLine();
                    // NOTE : THIS NEEDS TO BE LOOPED
                    var studyID = new TextPrompt<int>($"Enter the # of Flash Card(1 - {cards.Count}) to Study OR -1 for a random flash card to study 0 to EXIT")
                        .Validate(input =>
                         {
                             if (input == 0) return ValidationResult.Success();
                             else if (input >= 1 && input <= cards.Count) return ValidationResult.Success();
                             else if (input == -1) return ValidationResult.Success();
                             else
                                 return ValidationResult.Error($"[red]{input} does not exist.  Must be between 1 and {cards.Count}[/]");
                         });

                    var prompt = AnsiConsole.Prompt(studyID);
                    // RANDOM NUMBER TEST BEGIN                    
                    if (prompt == -1)
                        prompt = Helper.GetRandomFlashCardFromList(cards.Count + 1);
                    // END
                    if (prompt != 0)
                    {
                        (int asked, int correct) = TestFlashCard(stackName, cards[prompt].Front, cards[prompt].Back);                        
                        totalAsked += asked;
                        totalCorrect += correct;
                    }
                    else if (prompt == 0)
                    {
                        if (totalAsked + totalCorrect == 0)
                            break;
                        // DISPLAY STATS FOR THIS SESSION ONLY
                        DisplaySessionStats(cards, stackName, totalAsked, totalCorrect);
                        SaveSessionData(stackName, totalAsked, totalCorrect,choice.ID);
                        break;
                    }
                    else
                        break;
                } 
                
            }
            break;
        }
    }

    private void SaveSessionData(string stackName, int totalAsked, int totalCorrect,int StackID)
    {
        SessionDTO session = new()
        {
            StackName = stackName,
            Score = totalCorrect,
            TotalQuestions = totalAsked,
            Completed = DateTime.Now
        };
        // WRITE TO DB -- NOT IMPLEMENTED
        var (success, message) = _sessionCtr.AddSession(session, StackID);
        if (success)
        {
            AnsiConsole.WriteLine("Session has been saved.");
        }
        else
        {
            AnsiConsole.WriteLine($"Session was not saved.  Error = ${message}");            
        }

        AnsiConsole.WriteLine("Press any key to return back to main menu");
        Console.ReadKey(true);            
    }

    private (int asked,int correct) TestFlashCard(string StackName,string Front,string Back) 
    {
        int asked = 0, correct = 0;
        while (true)
        {
            AnsiConsole.Clear();
            var table = new Table();
            table.Title(StackName);
            table.AddColumn("FRONT");
            table.AddRow(Front);
            AnsiConsole.Write(table);
            AnsiConsole.WriteLine();
            AnsiConsole.WriteLine();
            var guess = AnsiConsole.Ask<string>("Enter your answer or enter to exist", "");
            if (guess == String.Empty)
                return (asked, correct);
            asked++;
            AnsiConsole.Write($"You guessed ${guess} and it was ");
            if (guess.Equals(Back))
            {
                AnsiConsole.WriteLine("Correct.");
                correct++;
            }
            else
            {
                AnsiConsole.WriteLine("Wrong.");
                AnsiConsole.WriteLine($"The correct answer was {Back}");
            }

            var confirm = AnsiConsole.Confirm("Try Again (Y/N)?");
            if (!confirm)
                break;
        }
        return (asked, correct);
    }

    private void DisplayUserStats()
    {
        AnsiConsole.Clear();
        var sessionData = _sessionCtr.GetUserSessionData();
        // USE THIS TO GET DAILY TOTAL --> IT WILL NOT PASS THE CHALLENGE
        // BUT HAPPY I CAN DO IT ON MY OWN
        var totals = sessionData.Aggregate(
            new { TotalScore = 0, TotalAsked = 0 },
            (acc, item) => new
            {
                TotalScore = acc.TotalScore + item.Score,
                TotalAsked = acc.TotalAsked + item.TotalQuestions,
                
            }
        );

        var totalPerecentage = (double)totals.TotalScore / totals.TotalAsked * 100;
        string percent = $"{totalPerecentage:F2}%";

        if (sessionData.Count == 0)
        {
            AnsiConsole.MarkupLineInterpolated($"[RED]NO USER DATA EXIST.  USE THE NEW MENU OPTION INSTEAD.[/]");
        }
        else
        {
            AnsiConsole.WriteLine();
            var title = $"[Bold]USER STATS SO FAR[/]";
            var table = new Table();
            
            table.Title(title);
            table.AddColumn("Stack Name");
            table.AddColumn("Question Asked");
            table.AddColumn("Question Answered");
            table.AddColumn("Score Percentage");
            table.AddColumn("Session Completed");


            foreach (var single in sessionData)
            {
                table.AddRow(
                    single.StackName, single.TotalQuestions.ToString(), single.Score.ToString(),
                    single.percentage.ToString() + "%", single.Completed.ToShortDateString()
                    );
            }

            var align = Align.Center(table);
            
            AnsiConsole.Write(align);
            AnsiConsole.WriteLine();
            table = new Table();
            table.Title("Daily Totals");
            table.Rows.Clear();
            table.AddColumn("Total Score");
            table.AddColumn("Total Questions");
            table.AddColumn("Total Percentage");            
            table.AddRow(totals.TotalScore.ToString(), totals.TotalAsked.ToString(), percent);
            align = Align.Center(table);
            AnsiConsole.Write(align);
        }
        AnsiConsole.WriteLine("Press any key to exit.");
        Console.ReadKey(true);
        Thread.Sleep(2000);
    }
}
