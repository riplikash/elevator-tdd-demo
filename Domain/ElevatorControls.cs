using System;
using System.Threading.Tasks;

namespace Domain
{
    public class ElevatorControls : IElevatorControls
    {
        private readonly IElevatorService elevatorService;
        private int currentFloor;
        public ElevatorControls(int currentFloor, IElevatorService elevatorService, int totalFloors)
        {
            this.currentFloor = currentFloor;
            this.elevatorService = elevatorService;
            TotalFloors = totalFloors;
        }
        // TODO: make more generic interface
        public async Task PushFloorButtonAsync(int floor)
        {
            await SubmitCallForCurrentFloor(floor).ConfigureAwait(false);
        }
        

        private async Task SubmitCallForCurrentFloor(int buttonPushed)
        {
            if (currentFloor > buttonPushed) await elevatorService.DownCallRequestAsync(buttonPushed).ConfigureAwait(false);
            else if (currentFloor < buttonPushed) await elevatorService.UpCallRequestAsync(buttonPushed).ConfigureAwait(false);
        }

       

        public Task FloorUpdateEventHandlerAsync(int newFloor)
        {
            if (newFloor > TotalFloors || newFloor < 1) throw new ArgumentOutOfRangeException(
                $"newFloor must be between 1 and {TotalFloors}");
            currentFloor = newFloor;
            return Task.FromResult(0);
        }

        public int TotalFloors { get;  }

        public string FloorDisplay => currentFloor.ToString();
    }
}