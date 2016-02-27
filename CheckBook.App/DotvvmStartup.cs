using DotVVM.Framework.Configuration;
using DotVVM.Framework.Routing;

namespace CheckBook.App
{
    public class DotvvmStartup : IDotvvmStartup
    {
        public void Configure(DotvvmConfiguration config, string applicationPath)
        {
            RegisterRoutes(config);
            RegisterMarkupControls(config);
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
    }
}