using DataLayer.Controller;
using DataLayer.Models;
using Microsoft.SqlServer.Management.XEvent;
using Spectre.Console;

namespace UI
{
    public class MainMenu : IMenu
    {
        private readonly string _appName = "Flashcards App";
        private readonly string _appNote = "Table DEFAULT can not be deleted since it is a system table.";
        private readonly StackController _stack;
        private readonly CardController _card;
        private string[] _menu = [];

        public MainMenu(CardController card, StackController stack)
        {
            _card = card;
            _stack = stack;
            LoadMenu(IMenu.MainMenuTitle);

        }

        public CardController CardCtr { get => _card; init => _card = value; }
        public StackController StackCtr { get => _stack; init  => _stack = value; }
        public void DisplayMenu()
        {
            while (true)
            {
                AnsiConsole.Clear();
                AnsiConsole.Write(new FigletText(_appName).Color(Color.Green));
                AnsiConsole.WriteLine();

                var choice = AnsiConsole.Prompt(new SelectionPrompt<string>()
                        .Title("Select an option:").AddChoices(_menu));

                 switch (choice)
                {
                    case "Study Sessions":
                        var session = new StudySession.Sessions(_card, _stack);
                        session.DisplayMenu();
                        continue;
                    case "FlashCards":
                        new CardMenu(_card,_stack).DisplayMenu();
                        continue;
                    case "Stacks":
                        new StackMenu(_card,_stack).DisplayMenu();
                        continue;
                    case "Exit":
                        break;
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
    }
}
