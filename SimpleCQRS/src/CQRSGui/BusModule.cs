using System.Linq;
using System.Reflection;
using Ninject.Modules;
using SimpleCQRS;
using Ninject;
using SimpleCQRS.Ninject;

namespace CQRSGui
{
    public class BusModule : NinjectModule 
    {

        public override void Load()
        {
            Kernel.Bind<ICommandSender>().To<CommandHandler>();
            Kernel.Bind<IEventPublisher>().To<EventPublisher>();
            
            var asm = Assembly.GetAssembly(typeof(InventoryCommandHandlers));
            Kernel.Bind(typeof (IHandles<>))
                .ToMethod(ctx =>
                              {
                                  var service = ctx.Request.Service;
                                  var implementations = from t in asm.GetTypes()
                                                        where t.IsClass && !t.IsAbstract
                                                              && t.IsAssignableFrom(service)
                                                        select t;
                                  foreach (var implementation in implementations)
                                      Kernel.Bind(service)
                                          .To(implementation);
                                  ctx.Request.IsUnique
                                  return Kernel.Get(service);
                              });

        }

    }
}