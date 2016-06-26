using System.Threading.Tasks;

namespace Domain
{
    public interface IElevatorControls
    {
        Task PushFloorButtonAsync(int floor);
        Task FloorUpdateEventHandlerAsync(int newFloor);
        string FloorDisplay { get; }

        // TODO: internal display
        // TODO: internal doors (?)
    }
}
