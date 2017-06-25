using EPiServer.Commerce.Internal.Migration.Steps;
using EPiServer.ServiceLocation;
using Mediachase.Commerce.Shared;
using StructureMap;

namespace Star.Epi.CMS.Business.Migrations
{
    [ServiceConfiguration(typeof(IMigrationStep))]
    public class SetupDataMigrationStep : IMigrationStep
    {
        private readonly IContainer _container;

        public SetupDataMigrationStep(IContainer container)
        {
            _container = container;
        }

        public int Order => 1000;
        public string Name => "Migrate";
        public string Description => "Dump stuff";

        public bool Execute(IProgressMessenger processMessenger)
        {
            var setup = new SetupData(_container);
            setup.Dump();
            return true;
        }
    }
}