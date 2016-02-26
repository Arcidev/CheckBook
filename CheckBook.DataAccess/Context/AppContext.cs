﻿using System.Data.Entity;
using CheckBook.DataAccess.Model;

namespace CheckBook.DataAccess.Context
{
    public class AppContext : DbContext
    {

        public AppContext() : base("name=DB")
        {
            
        }

        public DbSet<User> Users { get; set; }

        public DbSet<Group> Groups { get; set; }

        public DbSet<UserGroup> UserGroups { get; set; }

        public DbSet<PaymentGroup> PaymentGroups { get; set; }

        public DbSet<Payment> Payments { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Payment>()
                        .HasRequired(p => p.Debtor)
                        .WithMany()
                        .HasForeignKey(p => p.DebtorId)
                        .WillCascadeOnDelete(false);

            modelBuilder.Entity<Payment>()
                        .HasRequired(p => p.Payer)
                        .WithMany()
                        .HasForeignKey(p => p.PayerId)
                        .WillCascadeOnDelete(false);

            modelBuilder.Entity<Payment>()
                        .HasRequired(p => p.PaymentGroup)
                        .WithMany(p => p.Payments)
                        .HasForeignKey(p => p.PaymentGroupId)
                        .WillCascadeOnDelete(false);
        }
    }
}
