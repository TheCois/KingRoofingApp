﻿using System.Web.Mvc;
using System.Web.Routing;

namespace KRF.Web
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Inventory", action = "Index", id = UrlParameter.Optional },
                namespaces: new[] { "KRF.Web.Controllers" });

            routes.MapRoute("GetAddressListByCustomerID",
                            "Client/GetAddressListByCustomerID/customerID",
                            new { controller = "Client", action = "GetAddressListByCustomerID" },
                            new[] { "KRF.Web.Controllers" });
        }
    }
}