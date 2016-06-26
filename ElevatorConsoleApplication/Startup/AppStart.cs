using System;
using ApplicationServices;
using Domain;
using Infrastructure;
using Ninject;
using Ninject.Modules;

namespace ElevatorConsoleApplication.Startup
{
    public class AppStart
    {
        public static void Run()
        {
            IKernel kernel = new StandardKernel(new DIConfig());
            IElevatorService service = kernel.Get<IElevatorService>();
            Console.WriteLine("hey");

        }
    }

    public class DIConfig : NinjectModule
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
