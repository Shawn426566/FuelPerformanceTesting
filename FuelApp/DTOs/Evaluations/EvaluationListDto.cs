namespace FuelApp.DTOs.Evaluations
{
    /// <summary>
    /// Lightweight evaluation row returned by list endpoints.
    /// </summary>
    public class EvaluationListDto
    {
        public int EvaluationId { get; set; }
        public DateTime Date { get; set; }
        public required string Summary { get; set; }
        public int PlayerId { get; set; }
        public required string PlayerName { get; set; }
        public int? StaffMemberId { get; set; }
        public string? StaffMemberName { get; set; }
    }
}
