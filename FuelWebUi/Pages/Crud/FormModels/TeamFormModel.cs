using System.ComponentModel.DataAnnotations;

namespace FuelWebUi.Pages.Crud.FormModels
{
    public class TeamFormModel
    {
        [Required]
        [StringLength(100)]
        [RegularExpression(@"^[a-zA-Z0-9 .,'-]+$", ErrorMessage = "Invalid characters in name.")]
        public string Name { get; set; } = string.Empty;

        [Required]
        public int? AssociationID { get; set; }
    }
}