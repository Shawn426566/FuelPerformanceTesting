namespace FuelApp.DTOs.StaffMembers
{
    /// <summary>
    /// Lightweight staff member row returned by list endpoints.
    /// </summary>
    public class StaffMemberListDto
    {
        public int StaffMemberId { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string Role { get; set; }
        public string? Email { get; set; }
        public int? TeamId { get; set; }
        public string? TeamName { get; set; }
        public int EvaluationCount { get; set; }
    }
}
