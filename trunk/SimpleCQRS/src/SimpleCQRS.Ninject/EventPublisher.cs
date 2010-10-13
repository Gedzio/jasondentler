using Ninject;

namespace SimpleCQRS.Ninject
{
    public class EventPublisher : IEventPublisher
    {
        private readonly IKernel _kernel;

        public EventPublisher(IKernel kernel)
        {
            _kernel = kernel;
        }

        public void Publish<T>(T @event) where T : Event
        {
            var handlers = _kernel.GetAll<IHandles<T>>();
            foreach (var handler in handlers)
                handler.Handle(@event);
        }
    }
}
