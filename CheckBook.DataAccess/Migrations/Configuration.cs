using System.Data.Entity.Migrations;
using System.Linq;
using CheckBook.DataAccess.Enums;
using CheckBook.DataAccess.Model;
using CheckBook.DataAccess.Security;

namespace CheckBook.DataAccess.Migrations
{
    public sealed class Configuration : DbMigrationsConfiguration<DataAccess.Context.AppContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(DataAccess.Context.AppContext context)
        {
            var prague = context.Groups.FirstOrDefault(x => x.Name == "Lunches - Riganti Office");
            if (prague == null)
            {
                prague = new Group { Name = "Riganti - Prague" };
                context.Groups.Add(prague);
            }

            var brno = context.Groups.FirstOrDefault(x => x.Name == "Sport Activities");
            if (brno == null)
            {
                brno = new Group { Name = "Riganti - Brno" };
                context.Groups.Add(brno);
            }

            if (!context.Users.Any(x => x.Email == "newman@test.com"))
            {
                var password = PasswordHelper.CreateHash("Pa$$w0rd");
                var user = new User
                {
                    FirstName = "John",
                    LastName = "Newman",
                    Email = "newman@test.com",
                    UserRole = UserRole.User,
                    PasswordHash = password.PasswordHash,
                    PasswordSalt = password.PasswordSalt
                };

                context.Users.Add(user);
                context.UserGroups.Add(new UserGroup() { User = user, Group = prague });
            }

            if (!context.Users.Any(x => x.Email == "smith@test.com"))
            {
                var password = PasswordHelper.CreateHash("Pa$$w0rd");
                var user = new User
                {
                    FirstName = "John",
                    LastName = "Smith",
                    Email = "smith@test.com",
                    UserRole = UserRole.Admin,
                    PasswordHash = password.PasswordHash,
                    PasswordSalt = password.PasswordSalt
                };

                context.Users.Add(user);
                context.UserGroups.Add(new UserGroup() { User = user, Group = prague });
                context.UserGroups.Add(new UserGroup() { User = user, Group = brno });
            }

            if (!context.Users.Any(x => x.Email == "anderson@test.com"))
            {
                var password = PasswordHelper.CreateHash("Pa$$w0rd");
                var user = new User
                {
                    FirstName = "David",
                    LastName = "Anderson",
                    Email = "anderson@test.com",
                    UserRole = UserRole.User,
                    PasswordHash = password.PasswordHash,
                    PasswordSalt = password.PasswordSalt
                };

                context.Users.Add(user);
                context.UserGroups.Add(new UserGroup() { User = user, Group = prague });
                context.UserGroups.Add(new UserGroup() { User = user, Group = brno });
            }
        }
    }
}
