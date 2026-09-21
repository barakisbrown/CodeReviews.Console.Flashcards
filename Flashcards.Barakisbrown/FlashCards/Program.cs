using DataLayer;
using DataLayer.Controller;
using Spectre.Console;
using UI;

DbSetup setup = new();

var cardTable = new CardController();
var stackTable = new StackController();

var menu = new MainMenu(cardTable, stackTable);
menu.DisplayMenu();

AnsiConsole.WriteLine("Thank you for using the FlashCards App.  Have a good day.");
Console.ReadKey();