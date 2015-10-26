﻿using DataAccess.Context;
using DataAccess.Data;
using DataAccess.Model;
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
                    duplicateDebtor.Value += debtors.First(x => x.UserId == duplicateDebtor.UserId).Value + value;

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
                }

                db.SaveChanges();
            }
        }

        public static List<PaymentData> GetDebtors(int payerId)
        {
            using (var db = new AppContext())
            {
                return db.Payments.Where(x => x.UserId == payerId).Select(ToPaymentData).ToList();
            }
        }

        public static List<PaymentData> GetPayers(int debtorId)
        {
            using (var db = new AppContext())
            {
                return db.Payments.Where(x => x.DebtorId == debtorId).Select(ToPaymentData).ToList();
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

                db.SaveChanges();
            }
        }

        private static PaymentData ToPaymentData(Payment payment)
        {
            return new PaymentData() { DebtorId = payment.DebtorId, UserId = payment.UserId, Value = payment.Value };
        }
    }
}
