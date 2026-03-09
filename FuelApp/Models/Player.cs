using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace FuelApp.Models
{
    /// <summary>
    /// Represents a player in the sports management system.
    /// </summary>
    /// <remarks>
    /// Players are evaluated at the association level and may not be assigned to a team initially.
    /// A player must be part of an association but team assignment is optional during the evaluation phase.
    /// </remarks>
    public class Player
    {
        /// <summary>
        /// Gets or sets the unique identifier for the player.
        /// </summary>
        public int PlayerID { get; set; }

        /// <summary>
        /// Gets or sets the player's first name.
        /// </summary>
        [Required]
        [StringLength(50)]
        public required string FirstName { get; set; }

        /// <summary>
        /// Gets or sets the player's last name.
        /// </summary>
        [Required]
        [StringLength(50)]
        public required string LastName { get; set; }

        /// <summary>
        /// Gets or sets the list of evaluations associated with the player.
        /// </summary>
        public List<Evaluation> Evaluations { get; set; } = new();

        /// <summary>
        /// Gets or sets the team identifier for the player.
        /// </summary>
        /// <remarks>
        /// This value is optional and may be null during player evaluation phase.
        /// A player is assigned to a team only after evaluation is complete.
        /// </remarks>
        public int? TeamID { get; set; }

        /// <summary>
        /// Gets or sets the team associated with the player.
        /// </summary>
        /// <remarks>
        /// This navigation property is optional and may be null if the player has not yet been assigned to a team.
        /// </remarks>
        [JsonIgnore]
        public Team? Team { get; set; }

        /// <summary>
        /// Gets or sets the association identifier for the player.
        /// </summary>
        /// <remarks>
        /// Optional. A player may not yet be assigned to an association.
        /// </remarks>
        public int? AssociationID { get; set; }

        /// <summary>
        /// Gets or sets the association associated with the player.
        /// </summary>
        /// <remarks>
        /// Optional navigation property. Players may exist without an association.
        /// </remarks>
        [JsonIgnore]
        public Association? Association { get; set; }

        /// <summary>
        /// Gets or sets the player's birth date.
        /// </summary>
        [Required]
        public DateTime BirthDate { get; set; }

        /// <summary>
        /// Gets or sets the player's position on the field/court.
        /// </summary>
        public Position? Position { get; set; }

        /// <summary>
        /// Gets or sets the player's jersey number.
        /// </summary>
        public int JerseyNumber { get; set; }

        /// <summary>
        /// Gets or sets the player's email address.
        /// </summary>
        public string? Email { get; set; }
    }
}