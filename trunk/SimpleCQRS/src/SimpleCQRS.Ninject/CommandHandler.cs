using Ninject;

namespace SimpleCQRS.Ninject
{
    public class CommandHandler : ICommandSender
    {
        private readonly IKernel _kernel;
        
        public CommandHandler(IKernel kernel)
        {
            _kernel = kernel;
        }

        public void Send<T>(T command) where T : Command
        {
            var handler = _kernel.Get<IHandles<T>>();
            handler.Handle(command);
        }

    }
}
