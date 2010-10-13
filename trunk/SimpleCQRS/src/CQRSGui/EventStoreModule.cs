using Ninject.Modules;
using SimpleCQRS.EventStore;
using SimpleCQRS.EventStore.Memory;

namespace CQRSGui
{
    public class EventStoreModule : NinjectModule 
    {

        public override void Load()
        {
            Kernel.Bind<IEventStore>().To<MemoryEventStore>();
        }

    }
}