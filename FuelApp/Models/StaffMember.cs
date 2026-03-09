using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace FuelApp.Models
{
    /// <summary>
    /// Represents a staff member (coach, manager, or assistant) in the sports management system.
    /// </summary>
    public class StaffMember
    {
        /// <summary>
        /// Gets or sets the unique identifier for the staff member.
        /// </summary>
        public int StaffMemberID { get; set; }

        /// <summary>
        /// Gets or sets the staff member's first name.
        /// </summary>
        [Required]
        [StringLength(50)]
        public required string FirstName { get; set; }

        /// <summary>
        /// Gets or sets the staff member's last name.
        /// </summary>
        [Required]
        [StringLength(50)]
        public required string LastName { get; set; }

        /// <summary>
        /// Gets or sets the staff member's role (Coach, Manager, Evaluator or Assistant).
        /// </summary>
        [Required]
        public StaffRole Role { get; set; }

        /// <summary>
        /// Gets or sets the staff member's email address.
        /// </summary>
        public string? Email { get; set; }

        /// <summary>
        /// Gets or sets the team identifier for the staff member.
        /// </summary>
        public int? TeamID { get; set; }

        /// <summary>
        /// Gets or sets the team associated with the staff member.
        /// </summary>
        [JsonIgnore]
        public Team? Team { get; set; }

        /// <summary>
        /// Gets or sets the list of evaluations conducted by the staff member.
        /// </summary>
        [JsonIgnore]
        public List<Evaluation> Evaluations { get; set; } = new();
    }
}
