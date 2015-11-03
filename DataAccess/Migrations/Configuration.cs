namespace DataAccess.Migrations
{
    using Security;
    using Enums;
    using Model;
    using System.Data.Entity.Migrations;

    internal sealed class Configuration : DbMigrationsConfiguration<DataAccess.Context.AppContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            ContextKey = "DataAccess.Context.AppContext";
        }

        protected override void Seed(DataAccess.Context.AppContext context)
        {
            context.Groups.AddOrUpdate(
                new Group { Name = "Test group" },
                new Group { Name = "Test group 2" });

            var password = PasswordHelper.CreateHash("Pa$$w0rd");
            context.Users.AddOrUpdate(
                new User
                {
                    FirstName = "Admin",
                    LastName = "Admin",
                    Email = "admin@admin.com",
                    UserRole = UserRoles.Admin,
                    PasswordHash = password.PasswordHash,
                    PasswordSalt = password.PasswordSalt
                });
        }
    }
}
