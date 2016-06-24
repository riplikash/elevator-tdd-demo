using System.Threading.Tasks;

namespace ApplicationServices
{
    public interface IElevatorInteriorActions : IPersonActions
    {
        Task<string> CheckCurrentFloorAsync();


        Task LeaveWhenDoorOpensAsync();
        Task PushButton1();
        Task PushButton2();
        Task PushButton3();
        Task PushButton4();
        Task PushButton5();
    }
}