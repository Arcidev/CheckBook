using System.Data.Entity.Migrations;

namespace CheckBook.DataAccess.Migrations
{
    public partial class InitialDatabaseSchema : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Groups",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 100),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.PaymentGroups",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CreatedDate = c.DateTime(nullable: false),
                        Description = c.String(nullable: false, maxLength: 100),
                        GroupId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Groups", t => t.GroupId, cascadeDelete: true)
                .Index(t => t.GroupId);
            
            CreateTable(
                "dbo.Payments",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Type = c.Int(nullable: false),
                        PayerId = c.Int(nullable: false),
                        DebtorId = c.Int(nullable: false),
                        PaymentGroupId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.DebtorId)
                .ForeignKey("dbo.Users", t => t.PayerId)
                .ForeignKey("dbo.PaymentGroups", t => t.PaymentGroupId)
                .Index(t => t.PayerId)
                .Index(t => t.DebtorId)
                .Index(t => t.PaymentGroupId);
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        FirstName = c.String(nullable: false, maxLength: 100),
                        LastName = c.String(nullable: false, maxLength: 100),
                        Email = c.String(nullable: false, maxLength: 100),
                        PasswordSalt = c.String(nullable: false, maxLength: 100),
                        PasswordHash = c.String(nullable: false, maxLength: 100),
                        UserRole = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.UserGroups",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.Int(nullable: false),
                        GroupId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Groups", t => t.GroupId, cascadeDelete: true)
                .ForeignKey("dbo.Users", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.GroupId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.UserGroups", "UserId", "dbo.Users");
            DropForeignKey("dbo.UserGroups", "GroupId", "dbo.Groups");
            DropForeignKey("dbo.Payments", "PaymentGroupId", "dbo.PaymentGroups");
            DropForeignKey("dbo.Payments", "PayerId", "dbo.Users");
            DropForeignKey("dbo.Payments", "DebtorId", "dbo.Users");
            DropForeignKey("dbo.PaymentGroups", "GroupId", "dbo.Groups");
            DropIndex("dbo.UserGroups", new[] { "GroupId" });
            DropIndex("dbo.UserGroups", new[] { "UserId" });
            DropIndex("dbo.Payments", new[] { "PaymentGroupId" });
            DropIndex("dbo.Payments", new[] { "DebtorId" });
            DropIndex("dbo.Payments", new[] { "PayerId" });
            DropIndex("dbo.PaymentGroups", new[] { "GroupId" });
            DropTable("dbo.UserGroups");
            DropTable("dbo.Users");
            DropTable("dbo.Payments");
            DropTable("dbo.PaymentGroups");
            DropTable("dbo.Groups");
        }
    }
}
