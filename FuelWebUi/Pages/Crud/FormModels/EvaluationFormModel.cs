using System.ComponentModel.DataAnnotations;

namespace FuelWebUi.Pages.Crud.FormModels
{
    public class EvaluationFormModel
    {
        [Required]
        [DataType(DataType.DateTime)]
        public DateTime? Date { get; set; }

        [Required]
        public int? PlayerId { get; set; }

        [Required]
        public int? StaffMemberId { get; set; }
        [StringLength(2000)]
        [RegularExpression(@"^[a-zA-Z0-9 .,'-]+$", ErrorMessage = "Invalid characters in name.")]
        public string? Summary { get; set; }
    }
}