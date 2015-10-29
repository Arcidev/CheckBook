using DataAccess.Context;
using DataAccess.Data;
using DataAccess.Enums;
using DataAccess.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DataAccess.Services
{
    public static class PaymentService
    {
        public static void CreatePayment(int payerId, List<UserPaymentData> debtors, decimal value)
        {
            using (var db = new AppContext())
            {
                var debtorIds = debtors.Select(y => y.UserId);
                var duplicateDebtors = db.Payments.Where(x => x.UserId == payerId && debtorIds.Contains(x.DebtorId)).ToList();
                foreach (var duplicateDebtor in duplicateDebtors)
                {
                    duplicateDebtor.Value += debtors.First(x => x.UserId == duplicateDebtor.DebtorId).Value + value;

                    db.PaymentHistories.Add(new PaymentHistory()
                    {
                        PayerId = payerId,
                        DebtorId = duplicateDebtor.DebtorId,
                        Value = value,
                        Date = DateTime.Now,
                        Type = PaymentHistoryType.Debt
                    });
                }

                var duplicateIds = duplicateDebtors.Select(x => x.DebtorId);
                foreach (var debtorId in debtorIds.Where(x => !duplicateIds.Contains(x)))
                {
                    var val = debtors.First(x => x.UserId == debtorId).Value + value;
                    if (val == 0)
                        continue;

                    db.Payments.Add(new Payment()
                    {
                        UserId = payerId,
                        DebtorId = debtorId,
                        Value = val
                    });

                    db.PaymentHistories.Add(new PaymentHistory()
                    {
                        PayerId = payerId,
                        DebtorId = debtorId,
                        Value = value,
                        Date = DateTime.Now,
                        Type = PaymentHistoryType.Debt
                    });
                }

                db.SaveChanges();
            }
        }

        public static List<UserPaymentData> GetDebtors(int payerId)
        {
            using (var db = new AppContext())
            {
                var payments = db.Payments.Where(x => x.UserId == payerId).ToList();
                return payments.Select(x => ToUserPaymentData(x, true)).ToList();
            }
        }

        public static List<UserPaymentData> GetPayers(int debtorId)
        {
            using (var db = new AppContext())
            {
                var payments = db.Payments.Where(x => x.DebtorId == debtorId).ToList();
                return payments.Select(x => ToUserPaymentData(x, false)).ToList();
            }
        }

        public static void PayDebt(int payerId, int debtorId, decimal value)
        {
            using (var db = new AppContext())
            {
                var payment = db.Payments.FirstOrDefault(x => x.DebtorId == debtorId && payerId == x.UserId);
                if (payment == null)
                    return;

                if (payment.Value <= value)
                    db.Payments.Remove(payment);
                else
                    payment.Value -= value;

                db.PaymentHistories.Add(new PaymentHistory()
                {
                    PayerId = payerId,
                    DebtorId = debtorId,
                    Value = value,
                    Date = DateTime.Now,
                    Type = PaymentHistoryType.Payment
                });

                db.SaveChanges();
            }
        }

        public static List<PaymentHistoryData> GetPaymentHistory(int userId)
        {
            using (var db = new AppContext())
            {
                var paymentHistory = db.PaymentHistories.Where(x => x.DebtorId == userId || x.PayerId == userId).ToList();
                return paymentHistory.Select(x => ToPaymentHistoryData(x, userId)).ToList();
            }
        }

        public static UserPaymentData ToUserPaymentData(Payment payment, bool debtor)
        {
            var user = debtor ? payment.Debtor : payment.User;
            return new UserPaymentData()
            {
                Name = string.Format("{0} {1}", user.FirstName, user.LastName),
                Value = payment.Value,
                UserId = user.Id
            };
        }

        public static PaymentHistoryData ToPaymentHistoryData(PaymentHistory history, int userId)
        {
            return new PaymentHistoryData()
            {
                Id = history.Id,
                PayerId = history.PayerId,
                DebtorId = history.DebtorId,
                UserId = userId,
                Value = history.Value,
                Date = history.Date,
                Type = history.Type,
                Debtor = UserService.ToUserInfoData(history.Debtor),
                Payer = UserService.ToUserInfoData(history.Payer)
            };
        }
    }
}
