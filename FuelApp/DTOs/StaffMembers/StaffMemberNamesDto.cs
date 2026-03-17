namespace FuelApp.DTOs.StaffMembers
{
    /// <summary>
    /// Staff member first and last names only with ID.
    /// </summary>
    public class StaffMemberNamesDto
    {
        public int StaffMemberId { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
    }
}
