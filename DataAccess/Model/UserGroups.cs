using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccess.Model
{
    public class UserGroups
    {
        [Key]
        [ForeignKey("User")]
        [Column(Order = 1)]
        public int UserId { get; set; }

        [Key]
        [ForeignKey("Group")]
        [Column(Order = 2)]
        public int GroupId { get; set; }

        public virtual User User { get; set; }

        public virtual Group Group { get; set; }
    }
}
