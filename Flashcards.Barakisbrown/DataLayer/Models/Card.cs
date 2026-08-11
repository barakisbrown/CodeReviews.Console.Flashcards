namespace DataLayer.Models;

/// <summary>
/// FlashCard Entity
/// </summary>
/// <param name="ID">ID OF THE CARD</param>
/// <param name="Prompt">The question this flashcard asks</param>
/// <param name="Answer">Answer the Prompt is asking for</param>
/// <param name="StackID">ID to the stack this card belongs too</param>
public class Card()
{
    public int ID { get; set; }
    public string Prompt { get; set; } = string.Empty;
    public string Answer { get; set; } = string.Empty;
    public int StackID { get; set; }
}
