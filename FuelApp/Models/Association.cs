using System.ComponentModel.DataAnnotations;

namespace FuelApp.Models
{
    public class Association
    {
        /// <summary>
        /// The unique identifier for the association.
        /// </summary>
        [Key]
        public int AssociationID { get; set; }

        /// <summary>
        /// The name of the association.
        /// </summary>
        [Required]
        [StringLength(100)]
        public required string Name { get; set; }

        /// <summary>
        /// The list of teams that belong to this association.
        /// </summary>
        public List<Team> Teams { get; set; } = new List<Team>();

    }
}