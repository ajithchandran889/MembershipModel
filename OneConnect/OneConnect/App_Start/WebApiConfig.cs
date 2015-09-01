using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Formatting;
using System.Web.Http;

namespace OneConnect
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            config.MapHttpAttributeRoutes();

            
            config.Routes.MapHttpRoute(
                name: "Account",
                routeTemplate: "api/{controller}/{action}/{token}",
                defaults: new { id = RouteParameter.Optional }
            );
            
            config.Routes.MapHttpRoute(
                name: "GetUsers",
                routeTemplate: "api/{controller}/{action}"
                
            );
            config.Routes.MapHttpRoute(
                name: "GetAccountInfo",
                routeTemplate: "api/{controller}/{action}"
            );
            config.Routes.MapHttpRoute(
                name: "Group",
                routeTemplate: "api/{controller}/{action}/{token}",
                defaults: new { id = RouteParameter.Optional }
            );
            config.Routes.MapHttpRoute(
               name: "GetGroupInfo",
               routeTemplate: "api/{controller}/{action}/{id}",
               defaults: new { id = RouteParameter.Optional }
           );

            config.Routes.MapHttpRoute(
               name: "GetGroupsWithStatus",
               routeTemplate: "api/{controller}/{action}/{isActiveOnly}"
           );

            config.Routes.MapHttpRoute(
               name: "GetGroups",
               routeTemplate: "api/{controller}/{action}"
           );
            config.Routes.MapHttpRoute(
               name: "GetGroupProductDetails",
               routeTemplate: "api/{controller}/{action}/{id}",
               defaults: new { id = RouteParameter.Optional }
           );

            config.Routes.MapHttpRoute(
               name: "GetGroupMemberDetails",
               routeTemplate: "api/{controller}/{action}/{id}",
               defaults: new { id = RouteParameter.Optional }
           );

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
            
            
            var jsonFormatter = config.Formatters.OfType<JsonMediaTypeFormatter>().First();
            jsonFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
        }
    }
}
