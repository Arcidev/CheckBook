using System.ComponentModel.DataAnnotations;

namespace DataAccess.Model
{
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
    }
}
