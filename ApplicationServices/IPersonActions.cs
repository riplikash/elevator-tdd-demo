using System.Threading.Tasks;

namespace ApplicationServices
{
    public interface IPersonActions
    {
        string CheckElevatorPositionAsync();
        string CheckSurroundings();
        Task EnterDoorWhenItOpensAsync();
    }
}