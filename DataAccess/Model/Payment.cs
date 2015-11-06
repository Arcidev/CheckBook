using DataAccess.Enums;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccess.Model
{
    public class Payment
    {
        [Key]
        public int Id { get; set; }
        
        [ForeignKey("Payer")]
        public int PayerId { get; set; }

        [ForeignKey("Debtor")]
        public int DebtorId { get; set; }

        [ForeignKey("PaymentGroup")]
        public int? PaymentGroupId { get; set; }

        public decimal Value { get; set; }

        public DateTime Date { get; set; }

        public PaymentHistoryType Type { get; set; }

        public virtual User Payer { get; set; }

        public virtual User Debtor { get; set; }

        public virtual PaymentGroup PaymentGroup { get; set; }
    }
}
