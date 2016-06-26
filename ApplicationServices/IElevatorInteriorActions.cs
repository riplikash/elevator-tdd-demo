using System.Threading.Tasks;

namespace ApplicationServices
{
    public interface IElevatorInteriorActions : IPersonActions
    {
        string CheckCurrentFloorAsync();

        Task PushButtonNumberAsync(int desiredFloor);
    }
}