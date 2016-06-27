using System.Threading;
using System.Threading.Tasks;

namespace Domain
{
    public class CallPanel : ICallPanel
    {
        private readonly IElevatorService elevatorService;
        private int floor;
        private bool isDoorOpen;
        private string elevatorFloorDisplay;

        public string ElevatorFloorDisplay
        {
            get { return Volatile.Read(ref elevatorFloorDisplay); }
            private set { Volatile.Write(ref elevatorFloorDisplay, value); }
        }

        private static int _floorClounter = 1;
        public CallPanel(IElevatorService elevatorService)
        {
            this.elevatorService = elevatorService;
            Floor = _floorClounter;
            Interlocked.Increment(ref _floorClounter);
            elevatorService.RegisterCallPanel(this);
            IsDoorOpen = false;
            ElevatorFloorDisplay = "";
        }

        public int Floor
        {
            get { return Volatile.Read(ref floor); }
            private set { Volatile.Write(ref floor, value); }
        }

        public int TotalFloors => elevatorService.TotalFloors;

        public bool IsDoorOpen
        {
            get { return Volatile.Read(ref isDoorOpen); }
            private set { Volatile.Write(ref isDoorOpen, value); }
        }


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
