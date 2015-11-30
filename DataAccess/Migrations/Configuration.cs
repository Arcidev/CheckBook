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
            Group prague = context.Groups.FirstOrDefault(x => x.Name == "Riganti - Prague");
            if (prague == null)
            {
                prague = new Group { Name = "Riganti - Prague" };
                context.Groups.Add(prague);
            }
            Group brno = context.Groups.FirstOrDefault(x => x.Name == "Riganti - Brno");
            if (brno == null)
            {
                brno = new Group { Name = "Riganti - Brno" };
                context.Groups.Add(brno);
            }

            var password = PasswordHelper.CreateHash("Pa$$w0rd");
            if (!context.Users.Any(x => x.Email == "newman@admin.com"))
            {
                var user = new User
                {
                    FirstName = "John",
                    LastName = "Newman",
                    Email = "newman@admin.com",
                    UserRole = UserRoles.Admin,
                    PasswordHash = password.PasswordHash,
                    PasswordSalt = password.PasswordSalt
                };

                context.Users.Add(user);
                context.UsersGroups.Add(new UserGroups() { User = user, Group = prague });
            }
            if (!context.Users.Any(x => x.Email == "smith@admin.com"))
            {
                var user = new User
                {
                    FirstName = "John",
                    LastName = "Smith",
                    Email = "smith@admin.com",
                    UserRole = UserRoles.Admin,
                    PasswordHash = password.PasswordHash,
                    PasswordSalt = password.PasswordSalt
                };

                context.Users.Add(user);
                context.UsersGroups.Add(new UserGroups() { User = user, Group = brno });
            }
            if (!context.Users.Any(x => x.Email == "anderson@user.com"))
            {
                var user = new User
                {
                    FirstName = "David",
                    LastName = "Anderson",
                    Email = "anderson@user.com",
                    UserRole = UserRoles.User,
                    PasswordHash = password.PasswordHash,
                    PasswordSalt = password.PasswordSalt
                };

                context.Users.Add(user);
                context.UsersGroups.Add(new UserGroups() { User = user, Group = prague });
            }
        }
    }
}
