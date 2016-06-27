using System;
using System.Threading.Tasks;
using Domain;
using ElevatorConsoleApplication.Engine;
using Ninject;

namespace ElevatorConsoleApplication.Startup
{
    public class AppStart
    {
        public async Task Run()
        {
            IKernel kernel = new StandardKernel(new DependencyInjectionConfig());
            IElevatorService service = kernel.Get<IElevatorService>();
            for (int i = 0; i < 5; i++)
            {
                kernel.Get<ICallPanel>();
            }
            await service.StartAsync().ConfigureAwait(false);
            var engine = kernel.Get<Engine.Engine>();
            await engine.MainLoop().ConfigureAwait(false);
        }
    }
}
