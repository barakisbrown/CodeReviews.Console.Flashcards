using DataLayer;
using DataLayer.Controller;
using Spectre.Console;
using UI;

var db = new DbSetup();

var cardTable = new CardController();
var stackTable = new StackController();

AnsiConsole.WriteLine("Begin UI Application Here");
var menu = new MainMenu(cardTable, stackTable);
menu.DisplayMenu();

AnsiConsole.WriteLine("Thank you for using the FlashCards App.  Have a good day.");
Console.ReadKey();