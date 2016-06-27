using ApplicationServices;
using Domain;
using Infrastructure;
using Ninject.Modules;

namespace ElevatorConsoleApplication.Startup
{
    public class DependencyInjectionConfig : NinjectModule
    {
        public override void Load()
        {
            Bind<IElevatorService>().To<ElevatorService>().InSingletonScope();
            Bind<ICallPanel>().To<CallPanel>();
            Bind<IElevator>().To<DemoElevator>().InSingletonScope();
            Bind<IElevatorControls>().To<ElevatorControls>().InSingletonScope();
            Bind<IElevatorExteriorActions>().To<ElevatorExteriorActions>().InSingletonScope();
            Bind<IElevatorInteriorActions>().To<ElevatorInteriorActions>().InSingletonScope();

        }
    }
}