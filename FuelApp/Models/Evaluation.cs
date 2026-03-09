using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace FuelApp.Models
{
    /// <summary>
    /// Represents an evaluation record with performance metrics and assessor information.
    /// </summary>
    /// <remarks>
    /// This class is used to store evaluation data including a summary of the assessment,
    /// the date it was conducted, and references to the player being evaluated and the
    /// staff member who performed the evaluation.
    /// </remarks>
    public class Evaluation
    {
        /// <summary>
        /// Gets or sets the unique identifier for the evaluation.
        /// </summary>
        /// <value>An integer representing the primary key.</value>
        public int EvaluationId { get; set; }

        /// <summary>
        /// Gets or sets a brief summary of the evaluation results and observations.
        /// </summary>
        /// <value>A string containing the evaluation summary.</value>
        [Required]
        [StringLength(2000)]
        public required string Summary { get; set; }

        /// <summary>
        /// Gets or sets the date when the evaluation was conducted.
        /// </summary>
        /// <value>A DateTime object representing the evaluation date.</value>
        [DataType(DataType.Date)]
        [Required]
        public DateTime Date { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the player being evaluated.
        /// </summary>
        /// <value>An integer representing the foreign key to the Player entity.</value>
        [Required]
        public int PlayerId { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the staff member who conducted the evaluation.
        /// </summary>
        /// <value>An integer representing the foreign key to the StaffMember entity.</value>
        public int? StaffMemberId { get; set; }

        /// <summary>
        /// Gets or sets the player associated with this evaluation.
        /// </summary>
        [JsonIgnore]
        public Player? Player { get; set; }

        /// <summary>
        /// Gets or sets the staff member who conducted this evaluation.
        /// </summary>
        [JsonIgnore]
        public StaffMember? StaffMember { get; set; }

    }
}