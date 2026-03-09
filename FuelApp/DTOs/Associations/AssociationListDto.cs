namespace FuelApp.DTOs.Associations
{
    /// <summary>
    /// Lightweight association row returned by list endpoints.
    /// </summary>
    public class AssociationListDto
    {
        public int AssociationId { get; set; }
        public required string Name { get; set; }
        public int TeamCount { get; set; }
    }
}
