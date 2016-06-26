using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class CallPanel : ICallPanel
    {
        private static int _floorCounter;
        private readonly IElevatorService elevatorService;
        public string ElevatorFloorDisplay { get; private set; }
        public CallPanel(IElevatorService elevatorService)
        {
            this.elevatorService = elevatorService;
            Floor = System.Threading.Interlocked.Increment(ref _floorCounter); ;
            IsDoorOpen = false;
            ElevatorFloorDisplay = "";
        }

        public int Floor { get; }
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

        public Task FloorChangeEventHandlerAsync(int newFloor)
        {
            ElevatorFloorDisplay = newFloor.ToString();
            return Task.FromResult(0);
        }

        public Task DoorCloseEventHandlerAsync()
        {
            IsDoorOpen = false;
            return Task.FromResult(0);
        }

        public Task DoorOpenEventHandlerAsync()
        {
            IsDoorOpen = true;
            return Task.FromResult(0);
        }
    }
}
