using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public interface IElevatorService
    {
        HashSet<int> UpCalls { get; }
        HashSet<int> DownCalls { get; }
        int CurrentFloor { get; }
        int TotalFloors { get; }
        Task UpCallRequestAsync(int floor);
        Task DownCallRequestAsync(int floor);
        ICallPanel GetCallPanelForFloor(int floor);
        Task StopAsync();
        Task StartAsync();
        void RegisterCallPanel(ICallPanel newPanel);
    }
}
