namespace DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class update06 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.PaymentHistories",
                c => new
                    {
                        PayerId = c.Int(nullable: false),
                        DebtorId = c.Int(nullable: false),
                        Value = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Date = c.DateTime(nullable: false),
                        Type = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.PayerId, t.DebtorId })
                .ForeignKey("dbo.Users", t => t.DebtorId)
                .ForeignKey("dbo.Users", t => t.PayerId)
                .Index(t => t.PayerId)
                .Index(t => t.DebtorId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.PaymentHistories", "PayerId", "dbo.Users");
            DropForeignKey("dbo.PaymentHistories", "DebtorId", "dbo.Users");
            DropIndex("dbo.PaymentHistories", new[] { "DebtorId" });
            DropIndex("dbo.PaymentHistories", new[] { "PayerId" });
            DropTable("dbo.PaymentHistories");
        }
    }
}
