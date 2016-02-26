using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using CheckBook.DataAccess.Context;
using CheckBook.DataAccess.Data;
using CheckBook.DataAccess.Enums;
using CheckBook.DataAccess.Model;
using DotVVM.Framework.Controls;

namespace CheckBook.DataAccess.Services
{
    public static class PaymentService
    {
        
        /// <summary>
        /// Loads all payment groups in the specified group.
        /// </summary>
        public static void LoadPaymentGroups(int groupId, GridViewDataSet<PaymentGroupData> dataSet)
        {
            using (var db = new AppContext())
            {
                var paymentGroups = db.PaymentGroups
                    .Where(pg => pg.GroupId == groupId)
                    .Select(ToPaymentGroupData);

                dataSet.LoadFromQueryable(paymentGroups);
            }
        }

        /// <summary>
        /// Gets the payment group by ID.
        /// </summary>
        public static PaymentGroupData GetPaymentGroup(int paymentGroupId)
        {
            using (var db = new AppContext())
            {
                return db.PaymentGroups
                    .Where(pg => pg.Id == paymentGroupId)
                    .Select(ToPaymentGroupData)
                    .Single();
            }
        }

        /// <summary>
        /// Gets a list of all users in the specified group with paid amounts from the specified payment group.
        /// </summary>
        public static List<PaymentData> GetPayers(int groupId, int paymentGroupId)
        {
            using (var db = new AppContext())
            {
                return db.Users
                    .Where(u => u.UserGroups.Any(ug => ug.GroupId == groupId))
                    .Select(u => new
                    {
                        User = u,
                        Payment = u.Payments.FirstOrDefault(p => p.PaymentGroupId == paymentGroupId && p.Type == PaymentType.Payment && p.Amount >= 0)
                    })
                    .Select(u => new PaymentData()
                    {
                        Id = u.Payment.Id,
                        Name = u.User.FirstName + " " + u.User.LastName,
                        Amount = u.Payment.Amount,
                        UserId = u.User.Id
                    })
                    .OrderBy(u => u.Name)
                    .ToList();
            }
        }

        /// <summary>
        /// Gets a list of all users in the specified group with debt amounts from the specified payment group.
        /// </summary>
        public static List<PaymentData> GetDebtors(int groupId, int paymentGroupId)
        {
            using (var db = new AppContext())
            {
                return db.Users
                    .Where(u => u.UserGroups.Any(ug => ug.GroupId == groupId))
                    .Select(u => new
                    {
                        User = u,
                        Payment = u.Payments.FirstOrDefault(p => p.PaymentGroupId == paymentGroupId && p.Type == PaymentType.Payment && p.Amount < 0)
                    })
                    .Select(u => new PaymentData()
                    {
                        Id = u.Payment.Id,
                        Name = u.User.FirstName + " " + u.User.LastName,
                        Amount = -u.Payment.Amount,
                        UserId = u.User.Id
                    })
                    .OrderBy(u => u.Name)
                    .ToList();
            }
        }

        public static void SavePaymentGroup(PaymentGroupData data, List<PaymentData> payers, List<PaymentData> debtors)
        {
            using (var db = new AppContext())
            {
                // get or create the payment group
                var paymentGroup = db.PaymentGroups.Find(data.Id);
                if (paymentGroup == null)
                {
                    paymentGroup = new PaymentGroup()
                    {
                        GroupId = data.GroupId
                    };
                    db.PaymentGroups.Add(paymentGroup);
                }
                paymentGroup.CreatedDate = data.CreatedDate;
                paymentGroup.Description = data.Description;

                // delete all current payments
                foreach (var payment in paymentGroup.Payments.ToList())
                {
                    db.Payments.Remove(payment);
                }

                // generate new payments
                var involvedUsers = new HashSet<int>();
                foreach (var payer in payers.Where(p => p.Amount != null && p.Amount != 0))
                {
                    paymentGroup.Payments.Add(new Payment()
                    {
                        Amount = payer.Amount.Value,
                        UserId = payer.UserId,
                        Type = PaymentType.Payment
                    });
                    involvedUsers.Add(payer.UserId);
                }
                foreach (var debtor in debtors.Where(p => p.Amount != null && p.Amount != 0))
                {
                    paymentGroup.Payments.Add(new Payment()
                    {
                        Amount = -debtor.Amount.Value,
                        UserId = debtor.UserId,
                        Type = PaymentType.Payment
                    });
                    involvedUsers.Add(debtor.UserId);
                }

                // calculate rounding
                var difference = (payers.Sum(p => p.Amount) ?? 0) - (debtors.Sum(p => p.Amount) ?? 0);
                if (difference != 0 && involvedUsers.Any())
                {
                    foreach (var user in involvedUsers)
                    {
                        paymentGroup.Payments.Add(new Payment()
                        {
                            Amount = -difference / involvedUsers.Count,
                            UserId = user,
                            Type = PaymentType.AutoGeneratedRounding
                        });
                    }
                }

                if (involvedUsers.Count < 2)
                {
                    // no data was entered
                    throw new Exception("You have to fill the amount for at least two users!");
                }

                db.SaveChanges();
            }
        }

        public static void DeletePaymentGroup(PaymentGroupData data, List<PaymentData> payers, List<PaymentData> debtors)
        {
            using (var db = new AppContext())
            {
                // get or create the payment group
                var paymentGroup = db.PaymentGroups.Find(data.Id);
                db.PaymentGroups.Remove(paymentGroup);
                db.SaveChanges();
            }
        }


        private static Expression<Func<PaymentGroup, PaymentGroupData>> ToPaymentGroupData
        {
            get
            {
                return pg => new PaymentGroupData()
                {
                    Id = pg.Id,
                    Description = pg.Description,
                    TotalAmount = pg.Payments.Where(p => p.Amount > 0).Sum(p => (decimal?)p.Amount) ?? 0,
                    CreatedDate = pg.CreatedDate,
                    Currency = pg.Group.Currency,
                    GroupId = pg.GroupId
                };
            }
        }

        public static bool IsPaymentGroupEditable(int userId, int paymentGroupId)
        {
            using (var db = new AppContext())
            {
                var user = db.Users.Find(userId);
                return user.UserRole == UserRole.Admin 
                    || db.PaymentGroups.Any(pg => pg.Id == paymentGroupId && pg.Group.UserGroups.Any(ug => ug.UserId == userId));
            }
        }

    }
}
