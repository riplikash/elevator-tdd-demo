using System;
using System.Threading.Tasks;

namespace Domain
{
    public class ElevatorControls : IElevatorControls
    {
        private readonly IElevatorService elevatorService;

        public ElevatorControls(IElevatorService elevatorService)
        {
            this.elevatorService = elevatorService;
        }

        public async Task PushButtonNumberAsync(int floor)
        {
            await SubmitCallForCurrentFloor(floor).ConfigureAwait(false);
        }
        

        private async Task SubmitCallForCurrentFloor(int desiredFloor)
        {
            if (elevatorService.CurrentFloor > desiredFloor)
            {
                await elevatorService.DownCallRequestAsync(desiredFloor).ConfigureAwait(false);
            }
            else
            {
                await elevatorService.UpCallRequestAsync(desiredFloor).ConfigureAwait(false);
            }
        }

        public int TotalFloors => elevatorService.TotalFloors;

        public string FloorDisplay => elevatorService.CurrentFloor.ToString();
    }
}