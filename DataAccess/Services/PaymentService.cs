using DataAccess.Context;
using DataAccess.Data;
using DataAccess.Enums;
using DataAccess.Model;
using DotVVM.Framework.Controls;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DataAccess.Services
{
    public static class PaymentService
    {
        public static void CreatePayment(int payerId, string description, List<UserPaymentData> debtors, decimal value)
        {
            // Nothing to create
            if (!debtors.Any(x => x.Value > 0) && value <= 0)
                return;

            using (var db = new AppContext())
            {
                var paymentGroup = new PaymentGroup() { Description = description };
                db.PaymentGroups.Add(paymentGroup);
                db.SaveChanges();

                foreach (var debtor in debtors)
                {
                    var val = debtor.Value + value;
                    if (val > 0)
                    {
                        db.Payments.Add(new Payment()
                        {
                            PayerId = payerId,
                            DebtorId = debtor.UserId,
                            Value = val,
                            Date = DateTime.Now,
                            Type = PaymentHistoryType.Debt,
                            PaymentGroupId = paymentGroup.Id
                        });
                    }
                }

                db.SaveChanges();
            }
        }

        // TODO: Simplify this shit
        public static void LoadDebtorsAndPayers(int userId, GridViewDataSet<UserPaymentData> payersDataSet, GridViewDataSet<UserPaymentData> debtorsDataSet)
        {
            using (var db = new AppContext())
            {
                var debtors = db.Payments.Where(x => x.PayerId == userId && x.DebtorId != userId).GroupBy(x => x.DebtorId).ToList()
                    .Select(x => new { Payment = x.First(), Value = x.Where(y => y.Type == PaymentHistoryType.Debt).Sum(y => y.Value) - x.Where(y => y.Type == PaymentHistoryType.Payment).Sum(y => y.Value) }).ToList();

                var payers = db.Payments.Where(x => x.DebtorId == userId && x.PayerId != userId).GroupBy(x => x.PayerId).ToList()
                    .Select(x => new { Payment = x.First(), Value = x.Where(y => y.Type == PaymentHistoryType.Debt).Sum(y => y.Value) - x.Where(y => y.Type == PaymentHistoryType.Payment).Sum(y => y.Value) }).ToList();

                var debtorsSet = new List<UserPaymentData>();
                var payersSet = new List<UserPaymentData>();

                foreach (var debtor in debtors)
                {
                    var payer = payers.FirstOrDefault(x => x.Payment.PayerId == debtor.Payment.DebtorId);
                    if (payer == null)
                        continue;

                    var user = debtor.Payment.Debtor;
                    var userPayment = new UserPaymentData()
                    {
                        Name = string.Format("{0} {1}", user.FirstName, user.LastName),
                        UserId = user.Id,
                        Value = debtor.Value - payer.Value
                    };

                    if (userPayment.Value > 0)
                    {
                        debtorsSet.Add(userPayment);
                    }
                    else
                    {
                        userPayment.Value *= -1;
                        payersSet.Add(userPayment);
                    }
                }

                var debtorIds = debtors.Select(x => x.Payment.DebtorId);
                var payerIds = payers.Select(x => x.Payment.PayerId);
                debtorsSet.AddRange(debtors.Where(x => !payerIds.Contains(x.Payment.DebtorId)).Select(x => new UserPaymentData()
                {
                    Name = string.Format("{0} {1}", x.Payment.Debtor.FirstName, x.Payment.Debtor.LastName),
                    UserId = x.Payment.DebtorId,
                    Value = x.Value
                }));

                payersSet.AddRange(payers.Where(x => !debtorIds.Contains(x.Payment.PayerId)).Select(x => new UserPaymentData()
                {
                    Name = string.Format("{0} {1}", x.Payment.Payer.FirstName, x.Payment.Payer.LastName),
                    UserId = x.Payment.PayerId,
                    Value = x.Value
                }));

                payersDataSet.LoadFromQueryable(payersSet.AsQueryable());
                debtorsDataSet.LoadFromQueryable(debtorsSet.AsQueryable());
            }
        }

        public static void PayDebt(int payerId, int debtorId, decimal value)
        {
            using (var db = new AppContext())
            {
                db.Payments.Add(new Payment()
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

        public static void LoadPaymentHistory(int userId, GridViewDataSet<PaymentData> gridView)
        {
            using (var db = new AppContext())
            {
                var paymentHistory = db.Payments.Where(x => x.DebtorId == userId || x.PayerId == userId).OrderByDescending(x => x.Date)
                    .Select(x => new PaymentData()
                    {
                        Id = x.Id,
                        PayerId = x.PayerId,
                        DebtorId = x.DebtorId,
                        UserId = userId,
                        Value = x.Value,
                        Date = x.Date,
                        Type = x.Type,
                        Debtor = new UserInfoData()
                        {
                            Id = x.Debtor.Id,
                            Email = x.Debtor.Email,
                            FirstName = x.Debtor.FirstName,
                            LastName = x.Debtor.LastName
                        },
                        Payer = new UserInfoData()
                        {
                            Id = x.Payer.Id,
                            Email = x.Payer.Email,
                            FirstName = x.Payer.FirstName,
                            LastName = x.Payer.LastName
                        }
                    });

                gridView.LoadFromQueryable(paymentHistory);
            }
        }
    }
}
