using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public interface IElevatorService
    {
        HashSet<int> UpQueue { get; }
        HashSet<int> DownQueue { get; }
        int CurrentFloor { get; }
        int TotalFloors { get; }
        Task UpCallRequestAsync(int floor);
        Task DownCallRequestAsync(int floor);
        ICallPanel GetExternalCallInterfaceForFloor(int i);
        Task StopAsync();
        Task StartAsync();
    }
}
