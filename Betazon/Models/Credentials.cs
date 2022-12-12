using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Betazon.Models
{
    public class Credentials
    {
        #region Proprietà
        /// <summary>
        /// Primary key con identity
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Username utente
        /// </summary>
        [Required]
        [StringLength(20, ErrorMessage = "Inserire uno username di massimo 20 caratteri")]
        public string Username { get; set; }

        /// <summary>
        /// Ruolo utente il 2(admin non è selezionabile alla registrazione ma solo assegnabile da un admin)
        /// </summary>
        [Required]
        [Range(0, 2)]
        public int Role { get; set; }

        /// <summary>
        /// Foreign key alla tabella customer
        /// </summary>
        [Required]
        public int CustomerId { get; set; }

        /// <summary>
        /// Record customer collegato alla tabella user
        /// </summary>
        [ForeignKey("CustomerId")]
        public virtual Customer customer { get; set; } = null!;

        public virtual ICollection<UserLog> UserLogs { get; } = new List<UserLog>();

        #endregion
    }
}
