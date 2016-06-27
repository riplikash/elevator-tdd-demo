using System.Threading;
using System.Threading.Tasks;
using Domain;

namespace ApplicationServices
{
    public interface IPersonActions
    {
        string CheckElevatorPosition();
        string CheckSurroundings();
        Task EnterDoorWhenItOpensAsync(CancellationToken token);
        ICallPanel CallPanel { get; set; }
    }
}