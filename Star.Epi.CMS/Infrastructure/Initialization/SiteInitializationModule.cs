using System.Web.Http;
using System.Web.Mvc;
using EPiServer.Framework;
using EPiServer.Framework.Initialization;
using EPiServer.ServiceLocation;
using KW.Dawero.Web.Infrastructure.WebApi;
using Newtonsoft.Json;
using Star.Epi.CMS.Infrastructure.DependencyResolvers;

namespace Star.Epi.CMS.Infrastructure.Initialization
{
    [ModuleDependency(typeof(EPiServer.Commerce.Initialization.InitializationModule))]
    public class SiteInitializationModule : IConfigurableModule
    {
        public void Initialize(InitializationEngine context)
        {
            GlobalFilters.Filters.Add(new HandleErrorAttribute());
        }

        public void ConfigureContainer(ServiceConfigurationContext context)
        {
            DependencyResolver.SetResolver(new WebStructureMapDependencyResolver(context.StructureMap()));

            GlobalConfiguration.Configure(config =>
            {
                config.IncludeErrorDetailPolicy = IncludeErrorDetailPolicy.LocalOnly;
                config.Formatters.JsonFormatter.SerializerSettings = new JsonSerializerSettings();
                config.Formatters.XmlFormatter.UseXmlSerializer = false;
                config.DependencyResolver = new StructureMapResolver(context.StructureMap());
                config.MapHttpAttributeRoutes();
                config.Routes.MapHttpRoute("API Default", "api/{controller}/{id}", new { id = RouteParameter.Optional });
            });
        }

        public void Uninitialize(InitializationEngine context) { }
    }
}