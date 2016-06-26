using System.Threading.Tasks;

namespace Domain
{
    public interface IElevatorControls
    {
        Task PushButtonNumberAsync(int floor);
        string FloorDisplay { get; }

    }
}
