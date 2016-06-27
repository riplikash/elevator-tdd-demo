using System.Threading;
using System.Threading.Tasks;
using Domain;

namespace ApplicationServices
{
    public interface IPersonActions
    {
        string CheckElevatorPosition();
        string CheckSurroundings();
        void EnterDoor();
        ICallPanel CallPanel { get; set; }
    }
}