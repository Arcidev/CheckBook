using DataAccess.Enums;
using System;

namespace DataAccess.Data
{
    public class PaymentData
    {
        public int Id { get; set; }

        public int PayerId { get; set; }

        public int DebtorId { get; set; }

        public decimal Value { get; set; }

        public DateTime Date { get; set; }

        public PaymentHistoryType Type { get; set; }

        public int UserId { get; set; }

        public UserInfoData Payer { get; set; }

        public UserInfoData Debtor { get; set; }

        public string DateString { get { return Date.ToString("hh:mm:ss dd-MM-yyyy"); } }

        public string Action
        {
            get
            {
                if (PayerId == DebtorId)
                    return string.Format("You have payed {0} CZK for Yourself", Value);

                if (PayerId == UserId)
                    return Type == PaymentHistoryType.Debt ?
                        string.Format("You have payed {0} CZK for {1}", Value, Debtor.FullName) :
                        string.Format("{0} has payed You back {1} CZK", Debtor.FullName, Value);

                return Type == PaymentHistoryType.Debt ?
                    string.Format("{0} has payed {1} CZK for You", Payer.FullName, Value) :
                    string.Format("You have payed back {0} CZK to {1}", Value, Payer.FullName);
            }
        }
    }
}
