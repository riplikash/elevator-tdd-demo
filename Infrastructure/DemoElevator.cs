using System.Threading;
using System.Threading.Tasks;
using Domain;

namespace Infrastructure
{
    public class DemoElevator : IElevator
    {
        public Task MoveUpAsync()
        {
            return Task.Delay(3000);
        }

        public Task MoveDownAsync()
        {
            return Task.Delay(3000);
        }
    }
}