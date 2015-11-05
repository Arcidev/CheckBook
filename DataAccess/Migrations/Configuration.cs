namespace DataAccess.Migrations
{
    using Security;
    using Enums;
    using Model;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<DataAccess.Context.AppContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            ContextKey = "DataAccess.Context.AppContext";
        }

        protected override void Seed(DataAccess.Context.AppContext context)
        {
            context.Groups.AddOrUpdate(
                new Group { Name = "Test group" },
                new Group { Name = "Test group 2" });

            var password = PasswordHelper.CreateHash("Pa$$w0rd");
            if (!context.Users.Any(x => x.Email == "admin@admin.com"))
            {
                context.Users.Add(
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
}
