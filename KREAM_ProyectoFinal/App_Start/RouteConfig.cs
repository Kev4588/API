using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace PURIS_FLASH
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}", 
                // En la siguiente linea es donde se define quien sera la pagina que vaya a cargar desde el inicio
                defaults: new { controller = "Login", action = "Login", id = UrlParameter.Optional }
            );

            
        }

    }
}
