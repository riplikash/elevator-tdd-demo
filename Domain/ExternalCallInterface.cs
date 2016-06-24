using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class ExternalCallInterface : IExternalCallInterface
    {
        private readonly IElevatorService _elevatorService;
        public string ElevatorFloorDisplay { get; private set; }
        public ExternalCallInterface(IElevatorService elevatorService, int floor, int totalFloors)
        {
            _elevatorService = elevatorService;
            Floor = floor;
            TotalFloors = totalFloors;
            IsDoorOpen = false;
            ElevatorFloorDisplay = "";
        }

        public int Floor { get; }
        public int TotalFloors { get; }

        public bool IsDoorOpen { get; private set; }


        public async Task PushUpCallAsync()
        {
            await _elevatorService.UpCallRequestAsync(Floor).ConfigureAwait(false);
        }

        public async Task PushDownCallAsync()
        {
            await _elevatorService.DownCallRequestAsync(Floor).ConfigureAwait(false);
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
