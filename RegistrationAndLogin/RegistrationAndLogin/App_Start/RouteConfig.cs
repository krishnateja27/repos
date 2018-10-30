using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace RegistrationAndLogin
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default1",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Register", action = "Index", id = UrlParameter.Optional }
            );
            routes.MapRoute(
                name: "Default2",
                url: "{controller}/{action}/{firstName}/{lastName}/{age}/{userName}/{password}",
                defaults: new { controller = "Home", action = "Index"}
            );
        }
    }
}
