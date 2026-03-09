namespace FuelApp.DTOs.Players
{
    /// <summary>
    /// Lightweight player row returned by list endpoints.
    /// </summary>
    public class PlayerListDto
    {
        public int PlayerId { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public int? TeamId { get; set; }
        public string? TeamName { get; set; }
        public int? AssociationId { get; set; }
        public string? AssociationName { get; set; }
        public string? Position { get; set; }
        public int JerseyNumber { get; set; }
    }
}
