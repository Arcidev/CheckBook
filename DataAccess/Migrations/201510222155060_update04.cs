namespace DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class update04 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Users", "FirstName", c => c.String(nullable: false, maxLength: 100));
            AddColumn("dbo.Users", "LastName", c => c.String(nullable: false, maxLength: 100));
            AlterColumn("dbo.Groups", "Name", c => c.String(nullable: false, maxLength: 100));
            AlterColumn("dbo.Payments", "Value", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("dbo.Users", "Email", c => c.String(nullable: false, maxLength: 100));
            AlterColumn("dbo.Users", "Password", c => c.String(nullable: false, maxLength: 100));
            DropColumn("dbo.Users", "Name");
            DropColumn("dbo.Users", "Surname");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Users", "Surname", c => c.String(nullable: false));
            AddColumn("dbo.Users", "Name", c => c.String(nullable: false));
            AlterColumn("dbo.Users", "Password", c => c.String(nullable: false));
            AlterColumn("dbo.Users", "Email", c => c.String(nullable: false));
            AlterColumn("dbo.Payments", "Value", c => c.Int(nullable: false));
            AlterColumn("dbo.Groups", "Name", c => c.String(nullable: false));
            DropColumn("dbo.Users", "LastName");
            DropColumn("dbo.Users", "FirstName");
        }
    }
}
