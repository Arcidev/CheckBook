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
        /// <summary>
        /// Creates new payment
        /// </summary>
        /// <param name="payerId">The one who payed</param>
        /// <param name="description">Information about the payment</param>
        /// <param name="debtors">People for whom the payer has paid</param>
        /// <param name="value">Optional value that is added to all debtors</param>
        public static void CreatePayment(int payerId, string description, List<UserPaymentData> debtors, decimal value)
        {
            // Nothing to create
            if (!debtors.Any(x => x.Value > 0) && value <= 0)
                return;

            using (var db = new AppContext())
            {
                var paymentGroup = new PaymentGroup() { Description = description };
                db.PaymentGroups.Add(paymentGroup);

                foreach (var debtor in debtors)
                {
                    var val = debtor.Value + value;
                    if (val > 0)
                    {
                        db.Payments.Add(new Payment()
                        {
                            PayerId = payerId,
                            DebtorId = debtor.UserId,
                            Amount = val,
                            Date = DateTime.Now,
                            Type = PaymentType.Debt,
                            PaymentGroup = paymentGroup
                        });
                    }
                }

                db.SaveChanges();
            }
        }

        
        /// <summary>
        /// Loads debtor and payers related to specified user
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="payersDataSet">Payers will be added here</param>
        /// <param name="debtorsDataSet">Debtors will be added here</param>
        public static void LoadDebtorsAndPayers(int userId, GridViewDataSet<UserPaymentData> payersDataSet, GridViewDataSet<UserPaymentData> debtorsDataSet)
        {
            // Nowhere to load
            if (payersDataSet == null || debtorsDataSet == null)
                return;

            using (var db = new AppContext())
            {
                var debtors = db.Payments.Where(x => x.PayerId == userId && x.DebtorId != userId).GroupBy(x => x.DebtorId).ToList()
                    .Select(x => new { Payment = x.First(), Value = x.Where(y => y.Type == PaymentType.Debt).Sum(y => y.Amount) - x.Where(y => y.Type == PaymentType.Rounding).Sum(y => y.Amount) }).ToList();

                var payers = db.Payments.Where(x => x.DebtorId == userId && x.PayerId != userId).GroupBy(x => x.PayerId).ToList()
                    .Select(x => new { Payment = x.First(), Value = x.Where(y => y.Type == PaymentType.Debt).Sum(y => y.Amount) - x.Where(y => y.Type == PaymentType.Rounding).Sum(y => y.Amount) }).ToList();

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

        /// <summary>
        /// Pays debt
        /// </summary>
        /// <param name="payerId">Original payer</param>
        /// <param name="debtorId">The one who is paying back</param>
        /// <param name="value">Amount to pay</param>
        public static void PayDebt(int payerId, int debtorId, decimal value)
        {
            using (var db = new AppContext())
            {
                db.Payments.Add(new Payment()
                {
                    PayerId = payerId,
                    DebtorId = debtorId,
                    Amount = value,
                    Date = DateTime.Now,
                    Type = PaymentType.Rounding
                });

                db.SaveChanges();
            }
        }

        /// <summary>
        /// Loads history of all payments for specified user
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="gridView">History will be added here</param>
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

        /// <summary>
        /// Loads all user payments/debts as a group payment
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="gridView">Payments will be loaded here as a group</param>
        public static void LoadPaymentGroups(int userId, GridViewDataSet<PaymentGroupData> gridView)
        {
            using (var db = new AppContext())
            {
                var paymentGroups = db.PaymentGroups.Where(x => x.Payments.Any(y => y.PayerId == userId || y.DebtorId == userId))
                    .Select(x => new PaymentGroupData()
                    {
                        Id = x.Id,
                        Description = x.Description,
                        Value = x.Payments.Sum(y => y.Amount),
                        PayerName = x.Payments.FirstOrDefault().Payer.FirstName + " " + x.Payments.FirstOrDefault().Payer.LastName
                    }).OrderBy(x => x.Id);

                gridView.LoadFromQueryable(paymentGroups);
            }
        }
    }
}
