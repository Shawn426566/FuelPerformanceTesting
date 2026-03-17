namespace FuelApp.DTOs.Players
{
    /// <summary>
    /// Player first and last names only with ID.
    /// </summary>
    public class PlayerNamesDto
    {
        public int PlayerId { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
    }
}