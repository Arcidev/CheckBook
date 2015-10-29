namespace DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class update07 : DbMigration
    {
        public override void Up()
        {
            DropPrimaryKey("dbo.PaymentHistories");
            AddColumn("dbo.PaymentHistories", "Id", c => c.Int(nullable: false, identity: true));
            AddPrimaryKey("dbo.PaymentHistories", "Id");
        }
        
        public override void Down()
        {
            DropPrimaryKey("dbo.PaymentHistories");
            DropColumn("dbo.PaymentHistories", "Id");
            AddPrimaryKey("dbo.PaymentHistories", new[] { "PayerId", "DebtorId" });
        }
    }
}
