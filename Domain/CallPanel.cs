using System.Threading.Tasks;

namespace Domain
{
    public class CallPanel : ICallPanel
    {
        // TODO: Make threadsafe
        private readonly IElevatorService elevatorService;
        public string ElevatorFloorDisplay { get; private set; }
        public CallPanel(IElevatorService elevatorService)
        {
            this.elevatorService = elevatorService;
            IsDoorOpen = false;
            ElevatorFloorDisplay = "";
        }

        public int Floor => elevatorService.CurrentFloor;
        public int TotalFloors => elevatorService.TotalFloors;

        public bool IsDoorOpen { get; private set; }


        public async Task PushUpCallAsync()
        {
            await elevatorService.UpCallRequestAsync(Floor).ConfigureAwait(false);
        }

        public async Task PushDownCallAsync()
        {
            await elevatorService.DownCallRequestAsync(Floor).ConfigureAwait(false);
        }

        // Should this be event handler?
        public Task FloorChangeEventHandlerAsync(int newFloor)
        {
            ElevatorFloorDisplay = newFloor.ToString();
            return Task.CompletedTask;
        }

        public Task DoorCloseEventHandlerAsync()
        {
            IsDoorOpen = false;
            return Task.CompletedTask;
        }

        public Task DoorOpenEventHandlerAsync()
        {
            IsDoorOpen = true;
            return Task.CompletedTask;
        }
    }
}
