namespace DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class update03 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Payments", "Value", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Payments", "Value");
        }
    }
}
