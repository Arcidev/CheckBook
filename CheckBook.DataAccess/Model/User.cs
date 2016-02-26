using System.ComponentModel.DataAnnotations;
using CheckBook.DataAccess.Enums;

namespace CheckBook.DataAccess.Model
{

    /// <summary>
    /// A user who can login into the application.
    /// </summary>
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(100)]
        public string LastName { get; set; }

        [Required]
        [StringLength(100)]
        public string Email { get; set; }

        [Required]
        [StringLength(100)]
        public string PasswordSalt { get; set; }

        [Required]
        [StringLength(100)]
        public string PasswordHash { get; set; }

        public UserRole UserRole { get; set; }
    }
}
