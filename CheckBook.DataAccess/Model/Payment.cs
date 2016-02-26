using System.ComponentModel.DataAnnotations.Schema;
using CheckBook.DataAccess.Enums;

namespace CheckBook.DataAccess.Model
{
    public class Payment
    {
        public int Id { get; set; }
        
        public decimal Amount { get; set; }
        
        public PaymentType Type { get; set; }

        public int UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public virtual User User { get; set; }

        public int PaymentGroupId { get; set; }

        public virtual PaymentGroup PaymentGroup { get; set; }
    }
}
