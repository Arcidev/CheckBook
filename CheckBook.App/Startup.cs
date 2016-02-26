using System.Data.Entity;
using System.Web.Hosting;
using CheckBook.DataAccess.Context;
using Microsoft.Owin;
using Microsoft.Owin.FileSystems;
using Microsoft.Owin.StaticFiles;
using Owin;
using DotVVM.Framework;
using Microsoft.Owin.Security.Cookies;
using Microsoft.AspNet.Identity;

[assembly: OwinStartup(typeof(CheckBook.App.Startup))]
namespace CheckBook.App
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // set up Entity Framework Migrations
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<AppContext, DataAccess.Migrations.Configuration>());

            // use cookie authentication
            app.UseCookieAuthentication(new CookieAuthenticationOptions()
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/")
            });

            // use DotVVM
            var applicationPhysicalPath = HostingEnvironment.ApplicationPhysicalPath;
            var dotvvmConfiguration = app.UseDotVVM<DotvvmStartup>(applicationPhysicalPath);

            // use static files
            app.UseStaticFiles(new StaticFileOptions()
            {
                FileSystem = new PhysicalFileSystem(applicationPhysicalPath)
            });
        }
    }
}
