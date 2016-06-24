using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Domain
{
    public interface IElevator
    {
        Task MoveUpAsync();
        Task MoveDownAsync();
    }

    public class DemoElevator : IElevator
    {
        public Task MoveUpAsync()
        {
            Thread.Sleep(3000);
            return Task.FromResult(0);
        }

        public Task MoveDownAsync()
        {
            Thread.Sleep(3000);
            return Task.FromResult(0);
        }
    }
}
