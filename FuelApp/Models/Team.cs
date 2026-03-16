using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace FuelApp.Models
{
    public class Team
    {
        /// <summary>
        /// Gets or sets the unique identifier for the team.
        /// </summary>
        public int TeamID { get; set; }

        /// <summary>
        /// Gets or sets the name of the team.
        /// </summary>
        [Required]
        [StringLength(100)]
        [RegularExpression(@"^[a-zA-Z0-9 .,'-]+$", ErrorMessage = "Invalid characters in name.")]
        public required string Name { get; set; }
        /// <summary>
        /// Gets or sets the association identifier for the team.
        /// </summary>
        public int? AssociationID { get; set; }

        /// <summary>
        /// Gets or sets the association associated with the team.
        /// </summary>
        [JsonIgnore]
        public Association? Association { get; set; }

        /// <summary>
        /// Gets or sets the list of players assigned to the team.
        /// </summary>
        public List<Player> Players { get; set; } = new List<Player>();
    }
}