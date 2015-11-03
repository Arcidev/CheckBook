namespace DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class update08 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Users", "UserRole", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Users", "UserRole");
        }
    }
}
