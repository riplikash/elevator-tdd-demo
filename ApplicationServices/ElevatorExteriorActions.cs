using System;
using System.Threading.Tasks;
using Domain;

namespace ApplicationServices
{
    public class ElevatorExteriorActions : PersonActions, IElevatorExteriorActions
    {
        public ElevatorExteriorActions(IElevatorService elevatorService,  IElevatorControls controls) : base(elevatorService, elevatorService.GetCallPanelForFloor(1), controls)
        {
        }

        public async Task PushGoingUpButtonAsync()
        {
            if (CallPanel.Floor == ElevatorService.TotalFloors) throw new Exception("No up button on top floor");
            if (InElevator || CallPanel == null) throw new Exception("You are not in an elevator");
            await ElevatorService.UpCallRequestAsync(CallPanel.Floor).ConfigureAwait(false);
        }

        public async Task PushGoingDownButtonAsync()
        {
            if (CallPanel.Floor == 1) throw new Exception("No down button on first floor");
            if (InElevator || CallPanel == null) throw new Exception("You are not in an elevator");
            await ElevatorService.DownCallRequestAsync(CallPanel.Floor).ConfigureAwait(false);
        }
    }
}