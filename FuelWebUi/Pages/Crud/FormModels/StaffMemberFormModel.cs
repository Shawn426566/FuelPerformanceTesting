using System.ComponentModel.DataAnnotations;
using FuelWebUi.ApiClient.Models;

namespace FuelWebUi.Pages.Crud.FormModels
{
    public class StaffMemberFormModel
    {
        [Required]
        [StringLength(50)]
        [RegularExpression(@"^[a-zA-Z0-9 .,'-]+$", ErrorMessage = "Invalid characters in name.")]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        [RegularExpression(@"^[a-zA-Z0-9 .,'-]+$", ErrorMessage = "Invalid characters in name.")]

        public string LastName { get; set; } = string.Empty;

        [StringLength(100)]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", ErrorMessage = "Invalid email address.")]
        public string? Email { get; set; }

        [Required]
        public StaffRole? Role { get; set; }

        public int? TeamID { get; set; }
    }
}