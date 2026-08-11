using Dapper;
using DataLayer.Models;
using DataLayer.Models.DTO;
using Microsoft.Data.SqlClient;
using Spectre.Console;

namespace DataLayer.Controller;

public class CardController
{
    private readonly static DbUser DataSource = Configuration.GetUserSecretsConnStrings();
    private const string SelectAllSql = "SELECT * FROM dbo.Cards";
    private const string InsertSql = "INSERT INTO dbo.Cards(Prompt, Answer, StackID) VALUES(@PROMPT, @ANSWER, @STACKID)";
    private const string UpdateSql = "Update dbo.cards SET Prompt = @Prompt, Answer = @Answer, StackID = @StackID" + " WHERE dbo.cards.ID = @ID";
    private const string DeleteSql = "DELETE FROM dbo.Cards WHERE dbo.Cards.ID = @ID";
    private const string CardsPerStackSql = "SELECT COUNT(*) FROM Cards WHERE Cards.StackID = @fkey";

    public CardController()
    {
        
    }

    private SqlConnection MakeConnection
    {
        get
        {
            var conn = new SqlConnection(DataSource.Main);
            if (conn.State != System.Data.ConnectionState.Open)
            {
                try
                {
                    conn.Open();
                }
                catch(SqlException e)
                {
                    AnsiConsole.MarkupLineInterpolated($"[blink red]Error problem opening connection to the database engine. CHeck to see if it running.[/]");
                    throw;
                }
            }
            return conn;
        }
    }

    /*
     * ADDCARD / EDIT CARD / GET CARD / UPDATE CARD / DELETE CARD
     * 
     * CREATE COMPLETED
     * READ COMPLETED
     * UPDATE COMPLETED
     * DELETE COMPLETED
     */
    public (bool,string) AddCard(string Prompt, string Answer, int fkey = 1)
    {
        bool success;
        string message = string.Empty;
        // CHECK STRINGS FIRST
        if (string.IsNullOrEmpty(Prompt) || string.IsNullOrEmpty(Answer))
            return (false, "Prompt or Answer is either null or Empty!");
        // CREATE CONNECTION
        using var conn = MakeConnection;
        object[] param = { new { Prompt, Answer, @STACKID = fkey } };        
        try
        {
            var added = conn.Execute(InsertSql, param) == 1;
            success = true;
        }
        catch(Exception e)
        {
            success = false;
            message = "Not Added";
        }

        return (success, message);
    }

    public (bool,string) AddCard(Card newCard)
    {
        return AddCard(newCard.Prompt, newCard.Answer, newCard.StackID);

    }

    // READ SECTION BEGIN

    public List<Card> GetAllCards()
    {
        using var conn = MakeConnection;
        return conn.Query<Card>(SelectAllSql).ToList();
    }

    public int Count => GetAllCards().Count;

    public Card GetCardByID(int ID) => GetAllCards().FirstOrDefault(x => x.ID == ID);

    public List<Card> GetAllCardsByStack(int stackID) => GetAllCards().Where(x => x.StackID == stackID).ToList();

    public List<CardDTO> DisplayCardsByStack(int stackID)
    {
        var list = new List<CardDTO>();
        var cards = GetAllCards().Where(x => x.StackID == stackID).ToList();
        foreach(var single in cards)
        {
            var one = new CardDTO
            {
                Front = single.Prompt,
                Back = single.Answer
            };
            list.Add(one);
        }

        return list;
    }

    public int GetNumberCardsInStack(int fkey) => GetAllCardsByStack(fkey).Count;

    public bool CardExist(Card selCard)
    {
        var list = GetAllCardsByStack(selCard.StackID);
        var exist = list.Any(x => x.Prompt.Equals(selCard.Prompt) && (x.Answer.Equals(selCard.Answer)));
        return exist;
    }

    // READ SECTION END
    // UPDATE SECTION BEGIN
    public (bool,string) EditCard(Card updatedCard)
    {
        bool success = false;
        string message = string.Empty;
        using var conn = MakeConnection;
        object[] param = { new { updatedCard.Prompt, updatedCard.Answer, updatedCard.StackID, updatedCard.ID} };

        try
        {
            var updated = conn.Execute(UpdateSql, param) == 1;
            success = true;
        }
        catch (Exception e)
        {
            success = false;
            message = e.Message;
        }       
        return (success, message); 
    }
    
    // UPDATE SECTION END
    // DELETE SECTION BEGIN
    public (bool,string) DeleteCard(Card deletedCard)
    {
        bool success = false;
        string message = string.Empty;
        using var conn = MakeConnection;
        object[] param = { new { deletedCard.ID } };
        try {
            var deleted = conn.Execute(DeleteSql, param) == 1;
            success = deleted;
        } catch (Exception e) 
        {
            success = false;
            message = e.Message;
        }
        return (success, message);
    }
}