using System.Web.Hosting;
using Microsoft.Owin;
using Microsoft.Owin.FileSystems;
using Microsoft.Owin.StaticFiles;
using Owin;
using DotVVM.Framework;
using DotVVM.Framework.Configuration;
using Microsoft.Owin.Security.Cookies;
using Microsoft.AspNet.Identity;

[assembly: OwinStartup(typeof(CheckBook.Startup))]
namespace CheckBook
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.UseCookieAuthentication(new CookieAuthenticationOptions()
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie
            });

            var applicationPhysicalPath = HostingEnvironment.ApplicationPhysicalPath;

            // use DotVVM
            DotvvmConfiguration dotvvmConfiguration = app.UseDotVVM(applicationPhysicalPath);
            dotvvmConfiguration.RouteTable.Add("Login", "", "Views/login.dothtml", null);
            dotvvmConfiguration.RouteTable.Add("Home", "Home", "Views/home.dothtml", null);
            dotvvmConfiguration.RouteTable.Add("Payment", "Payment", "Views/payment.dothtml", null);
            dotvvmConfiguration.RouteTable.Add("Manager", "Manager", "Views/manager.dothtml", null);
            dotvvmConfiguration.RouteTable.Add("Settings", "Settings", "Views/settings.dothtml", null);

            // use static files
            app.UseStaticFiles(new StaticFileOptions()
            {
                FileSystem = new PhysicalFileSystem(applicationPhysicalPath)
            });
        }
    }
}
