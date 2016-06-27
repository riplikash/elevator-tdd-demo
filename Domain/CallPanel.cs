using System.Threading.Tasks;

namespace Domain
{
    public class CallPanel : ICallPanel
    {
        // TODO: Make threadsafe
        private readonly IElevatorService elevatorService;
        public string ElevatorFloorDisplay { get; private set; }
        private static int _floorClounter = 1;
        public CallPanel(IElevatorService elevatorService)
        {
            this.elevatorService = elevatorService;
            Floor = _floorClounter;
            System.Threading.Interlocked.Increment(ref _floorClounter);
            elevatorService.RegisterCallPanel(this);
            IsDoorOpen = false;
            ElevatorFloorDisplay = "";
        }

        public int Floor { get; set; }

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
