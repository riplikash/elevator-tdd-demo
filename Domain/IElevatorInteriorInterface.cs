using System.Threading.Tasks;

namespace Domain
{
    public interface IElevatorInteriorInterface
    {
        Task PushFloor1ButtonAsync();
        Task PushFloor2ButtonAsync();
        Task PushFloor3ButtonAsync();
        Task PushFloor4ButtonAsync();
        Task PushFloor5ButtonAsync();
        Task FloorUpdateEventHandlerAsync(int newFloor);
        string FloorDisplay { get; }

        // TODO: internal display
        // TODO: internal doors (?)
    }
}
