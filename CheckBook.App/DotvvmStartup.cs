using DotVVM.Framework.Configuration;
using DotVVM.Framework.ResourceManagement;
using DotVVM.Framework.Routing;

namespace CheckBook.App
{
    public class DotvvmStartup : IDotvvmStartup
    {
        public void Configure(DotvvmConfiguration config, string applicationPath)
        {
            RegisterRoutes(config);
            RegisterMarkupControls(config);
            RegisterResources(config);
        }
        
        private void RegisterRoutes(DotvvmConfiguration config)
        {
            // configure a default route
            config.RouteTable.Add("default", "", "Views/login.dothtml");

            // configure routes with parameters
            config.RouteTable.Add("group", "group/{Id}", "Views/group.dothtml", new { Id = (int?) null });
            config.RouteTable.Add("payment", "payment/{GroupId}/{Id}", "Views/payment.dothtml", new { Id = (int?) null });

            // configure customer presenters
            config.RouteTable.Add("identicon", "identicon/{Identicon}", null, null, () => new IdenticonPresenter());

            // auto-discover all missing parameterless routes
            config.RouteTable.AutoDiscoverRoutes(new DefaultRouteStrategy(config));
        }

        private void RegisterMarkupControls(DotvvmConfiguration config)
        {
            // register markup control
            config.Markup.Controls.Add(new DotvvmControlConfiguration()
            {
                Src = "Controls/UserAvatar.dotcontrol",
                TagPrefix = "cc",
                TagName = "UserAvatar"
            });
        }

        private void RegisterResources(DotvvmConfiguration config)
        {
            // register custom script
            config.Resources.Register("autoHideAlert", new ScriptResource()
            {
                Url = "/Scripts/autoHideAlert.js",
                Dependencies = new [] { "jquery" }
            });
            
            // Note that the 'jquery' resource is registered in DotVVM and points to official jQuery CDN.
            // If you need to use different version, or you want to embed jQuery in your app, use this code:
            // config.Resources.FindResource("jquery").Url = "your URL";
        }
    }
}