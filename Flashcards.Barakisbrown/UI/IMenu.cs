using DataLayer.Controller;

namespace UI;

public interface IMenu
{
    public const int SLEEP = 3000;
    public const string CardMenuTitle = "FlashCard-Menu.txt";
    public const string StackMenuTitle = "Stack-Menu.txt";
    public const string EditMenuTitle = "Edit-FlashCard-Menu.txt";
    public const string StudySessionTItle = "StudySession-Menu.txt";
    public const string MainMenuTitle = "Main-Menu.txt";
    public CardController CardCtr { get; init; }
    public StackController StackCtr { get; init; }
    public void DisplayMenu();

    public void LoadMenu(string menuName);
}