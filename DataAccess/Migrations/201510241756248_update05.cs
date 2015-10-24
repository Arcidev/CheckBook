namespace DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class update05 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Users", "PasswordSalt", c => c.String(nullable: false, maxLength: 100));
            AddColumn("dbo.Users", "PasswordHash", c => c.String(nullable: false, maxLength: 100));
            DropColumn("dbo.Users", "Password");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Users", "Password", c => c.String(nullable: false, maxLength: 100));
            DropColumn("dbo.Users", "PasswordHash");
            DropColumn("dbo.Users", "PasswordSalt");
        }
    }
}
