using System.ComponentModel.DataAnnotations;

namespace DataAccess.Model
{
    public class Group
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }
    }
}
