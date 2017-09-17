using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Routing;

namespace HyperStoreServiceAPP
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultAPIGet",
                routeTemplate: "api/{userId}/{controller}",
                defaults: new { action = "Get" },
                constraints: new
                {
                    httpMethod = new HttpMethodConstraint(HttpMethod.Get),
                    userId = @"[0-9A-Fa-f]{8}[-][0-9A-Fa-f]{4}[-][0-9A-Fa-f]{4}[-][0-9A-Fa-f]{4}[-][0-9A-Fa-f]{12}"
                }
              );

            config.Routes.MapHttpRoute(
                name: "DefaultAPIPost",
                routeTemplate: "api/{userId}/{controller}",
                defaults: new { action = "Post" },
                constraints: new
                {
                    httpMethod = new HttpMethodConstraint(HttpMethod.Post),
                    userId = @"[0-9A-Fa-f]{8}[-][0-9A-Fa-f]{4}[-][0-9A-Fa-f]{4}[-][0-9A-Fa-f]{4}[-][0-9A-Fa-f]{12}"
                }
                );

            config.Routes.MapHttpRoute(
                name: "DefaultAPIPut",
                routeTemplate: "api/{userId}/{controller}",
                defaults: new { action = "Put" },
                constraints: new
                {
                    httpMethod = new HttpMethodConstraint(HttpMethod.Put),
                    userId = @"[0-9A-Fa-f]{8}[-][0-9A-Fa-f]{4}[-][0-9A-Fa-f]{4}[-][0-9A-Fa-f]{4}[-][0-9A-Fa-f]{12}"
                }
                );

            config.Routes.MapHttpRoute(
             name: "DefaultApi",
             routeTemplate: "api/{userId}/{controller}/{id}",
             defaults: new { id = RouteParameter.Optional },
             constraints: new
             {
                 id = @"[0-9A-Fa-f]{8}[-][0-9A-Fa-f]{4}[-][0-9A-Fa-f]{4}[-][0-9A-Fa-f]{4}[-][0-9A-Fa-f]{12}",
                 userId = @"[0-9A-Fa-f]{8}[-][0-9A-Fa-f]{4}[-][0-9A-Fa-f]{4}[-][0-9A-Fa-f]{4}[-][0-9A-Fa-f]{12}"
             }
         );

            config.Routes.MapHttpRoute(
             name: "DefaultAPIWithAction",
             routeTemplate: "api/{userId}/{controller}/{action}",
             defaults: new { },
             constraints: new
             {
                 userId = @"[0-9A-Fa-f]{8}[-][0-9A-Fa-f]{4}[-][0-9A-Fa-f]{4}[-][0-9A-Fa-f]{4}[-][0-9A-Fa-f]{12}"
             }
             );
        }
    }
}
