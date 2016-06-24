using System;
using System.Threading.Tasks;

namespace Domain
{
    public class ElevatorInteriorInterface : IElevatorInteriorInterface
    {
        private IElevatorService elevatorService;
        private int currentFloor;
        public ElevatorInteriorInterface(int currentFloor, IElevatorService elevatorService, int totalFloors)
        {
            this.currentFloor = currentFloor;
            this.elevatorService = elevatorService;
            TotalFloors = totalFloors;
        }
        // TODO: make more generic interface
        public async Task PushFloor1ButtonAsync()
        {
            if (currentFloor != 1)
                await elevatorService.DownCallRequestAsync(1);
        }

        public async Task PushFloor2ButtonAsync()
        {
            await SubmitCallForCurrentFloor(2);
        }

        private async Task SubmitCallForCurrentFloor(int buttonPushed)
        {
            if (currentFloor > buttonPushed) await elevatorService.DownCallRequestAsync(buttonPushed);
            else if (currentFloor < buttonPushed) await elevatorService.UpCallRequestAsync(buttonPushed);
        }

        public async Task PushFloor3ButtonAsync()
        {
            await SubmitCallForCurrentFloor(3);
        }

        public async Task PushFloor4ButtonAsync()
        {
            await SubmitCallForCurrentFloor(4);
        }

        public async Task PushFloor5ButtonAsync()
        {
            await SubmitCallForCurrentFloor(5);
        }

        public Task FloorUpdateEventHandlerAsync(int newFloor)
        {
            if (newFloor > TotalFloors) throw new ArgumentOutOfRangeException(
                $"newFloor must be between 1 and {TotalFloors}");
            currentFloor = newFloor;
            return Task.FromResult(0);
        }

        public int TotalFloors { get;  }

        public string FloorDisplay => currentFloor.ToString();
    }
}