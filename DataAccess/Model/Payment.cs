using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccess.Model
{
    public class Payment
    {
        [Key]
        [ForeignKey("User")]
        [Column(Order = 1)]
        public int UserId { get; set; }

        [Key]
        [ForeignKey("Debtor")]
        [Column(Order = 2)]
        public int DebtorId { get; set; }

        public int Value { get; set; }

        public virtual User User { get; set; }

        public virtual User Debtor { get; set; }
    }
}
