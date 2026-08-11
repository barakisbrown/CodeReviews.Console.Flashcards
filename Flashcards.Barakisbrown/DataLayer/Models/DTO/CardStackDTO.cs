namespace DataLayer.Models.DTO
{
    /// <summary>
    /// Represents a data transfer object for a card stack, containing identifiers and card face text.
    /// </summary>
    /// <param name="DtoId">The unique identifier for the card stack.</param>
    /// <param name="Front">The text displayed on the front side of the card. Can be null if not specified.</param>
    /// <param name="Back">The text displayed on the back side of the card. Can be null if not specified.</param>
    public record class CardStackDTO(int DtoId, string? Front, string? Back);
}
