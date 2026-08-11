namespace DataLayer.Models
{
    /// <summary>
    /// Represents a stack entity with a unique identifier and an optional name.
    /// </summary>
    /// <param name="ID">The unique identifier for the stack. Must be a non-negative integer.</param>
    /// <param name="Name">The name of the stack, or <see langword="null"/> if unnamed.</param>
    public class Stack()
    {
        public int ID { get; set; }
        public required string Name { get; set; } = string.Empty;
    }
}
