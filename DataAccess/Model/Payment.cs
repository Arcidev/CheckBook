using DataAccess.Enums;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccess.Model
{
    public class Payment
    {
        public int Id { get; set; }
        
        public decimal Amount { get; set; }
        
        public PaymentType Type { get; set; }

        [ForeignKey(nameof(Payer))]
        public int PayerId { get; set; }

        public virtual User Payer { get; set; }

        [ForeignKey(nameof(Debtor))]
        public int DebtorId { get; set; }

        public virtual User Debtor { get; set; }

        public int PaymentGroupId { get; set; }

        public virtual PaymentGroup PaymentGroup { get; set; }
    }
}
