using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Betazon.Models
{
    public class UserLog
    {

        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Descrizione log
        /// </summary>
        public string Description { get; set; }

        [Required]
        public int UserId { get; set; }


        [ForeignKey("UserId")]

        /// <summary>
        /// Record User collegato alla tabella userLog
        /// </summary>
        public virtual Credentials user { get; set; } = null!;
    }
}
