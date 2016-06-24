using System.Threading.Tasks;

namespace ApplicationServices
{
    public interface IPersonActions
    {
        Task<string> CheckElevatorPositionAsync();
        Task<string> CheckSurroundings();
        Task<string> LookAtDoorAsync();
    }
}