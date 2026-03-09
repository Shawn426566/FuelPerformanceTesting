namespace FuelApp.DTOs.Teams
{
    /// <summary>
    /// Lightweight team row returned by list endpoints.
    /// </summary>
    public class TeamListDto
    {
        public int TeamId { get; set; }
        public required string Name { get; set; }
        public int? AssociationId { get; set; }
        public string? AssociationName { get; set; }
        public int PlayerCount { get; set; }
    }
}
