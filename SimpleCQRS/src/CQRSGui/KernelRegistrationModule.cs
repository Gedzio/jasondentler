using Ninject;
using Ninject.Modules;

namespace CQRSGui
{
    public class KernelRegistrationModule : NinjectModule 
    {

        public override void Load()
        {
            Kernel.Bind<IKernel>()
                .ToConstant(Kernel);
        }

    }
}