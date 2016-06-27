using System;
using System.Threading.Tasks;
using Domain;

namespace ApplicationServices
{
    public class ElevatorInteriorActions : PersonActions, IElevatorInteriorActions
    {
        public ElevatorInteriorActions(
            IElevatorService elevatorService,
            IElevatorControls controls) : base(elevatorService, elevatorService.GetCallPanelForFloor(1), controls)
        {

        }

        public string CheckCurrentFloorAsync()
        {
            return base.CheckElevatorPositionAsync();
        }

       
        public async Task PushButtonNumberAsync(int desiredFloor)
        {
            if (InElevator == false) throw new Exception("You are not in an elevator");
            if (ElevatorService.CurrentFloor > desiredFloor)
            {
                await ElevatorService.DownCallRequestAsync(desiredFloor).ConfigureAwait(false);
            }
            else
            {
                await ElevatorService.UpCallRequestAsync(desiredFloor).ConfigureAwait(false);
            }
            CallPanel = ElevatorService.GetCallPanelForFloor(desiredFloor);
        }
    }
}