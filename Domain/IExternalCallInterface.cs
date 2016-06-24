using System.Threading.Tasks;

namespace Domain
{
    public interface IExternalCallInterface
    {
        string ElevatorFloorDisplay { get; }
        int Floor { get; }
        bool IsDoorOpen { get; }
        int TotalFloors { get; }

        Task DoorCloseEventHandlerAsync();
        Task DoorOpenEventHandlerAsync();
        Task FloorChangeEventHandlerAsync(int newFloor);
        Task PushDownCallAsync();
        Task PushUpCallAsync();
    }
}