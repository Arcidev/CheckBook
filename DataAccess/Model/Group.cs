using System.ComponentModel.DataAnnotations;

namespace DataAccess.Model
{
    public class Group
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }
    }
}
