using EPiServer.Framework;
using EPiServer.Framework.Initialization;
using EPiServer.ServiceLocation;
using Mediachase.Commerce.Core.Features;
using Star.Epi.CMS.Business.Benchmarks;

namespace Star.Epi.CMS.Infrastructure.Initialization
{
    [ModuleDependency(typeof(SiteInitializationModule))]
    public class BenchmarkInitializationModule : IConfigurableModule
    {
        public void Initialize(InitializationEngine context)
        {
        }

        public void Uninitialize(InitializationEngine context)
        {
        }

        public void ConfigureContainer(ServiceConfigurationContext context)
        {
            // also switch the SerializedCarts feature in ecf.app.config
            context.Services.AddTransient<IBenchmarks, CartHelperBenchmarks>();
        }
    }
}