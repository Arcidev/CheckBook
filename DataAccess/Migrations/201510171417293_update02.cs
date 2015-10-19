namespace DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class update02 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Payments",
                c => new
                    {
                        UserId = c.Int(nullable: false),
                        DebtorId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.UserId, t.DebtorId })
                .ForeignKey("dbo.Users", t => t.DebtorId)
                .ForeignKey("dbo.Users", t => t.UserId)
                .Index(t => t.UserId)
                .Index(t => t.DebtorId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Payments", "UserId", "dbo.Users");
            DropForeignKey("dbo.Payments", "DebtorId", "dbo.Users");
            DropIndex("dbo.Payments", new[] { "DebtorId" });
            DropIndex("dbo.Payments", new[] { "UserId" });
            DropTable("dbo.Payments");
        }
    }
}
