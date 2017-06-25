using EPiServer.Framework;
using EPiServer.Framework.Initialization;
using Mediachase.Commerce.Orders;
using Mediachase.MetaDataPlus.Configurator;

namespace Star.Epi.CMS.Infrastructure.Initialization
{
    [ModuleDependency(typeof(FrameworkInitialization))]
    public class SetupMetaField : IInitializableModule
    {
        public void Initialize(InitializationEngine context)
        {
            var lineItemMetaClass = OrderContext.Current.LineItemMetaClass;
            var metaDataContext = OrderContext.MetaDataContext;
            if (lineItemMetaClass.MetaFields["Custom"] == null)
            {
                var mf = MetaField.Create(metaDataContext, string.Empty, "Custom", "Custom", string.Empty, MetaDataType.Boolean, 0, true, false, false, false);
                lineItemMetaClass.AddField(mf);
            }

        }

        public void Uninitialize(InitializationEngine context)
        {
        }
    }
}