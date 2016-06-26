using System;
using System.Threading.Tasks;
using Domain;

namespace ApplicationServices
{
    public class ElevatorExteriorActions : PersonActions, IElevatorExteriorActions
    {
        public ElevatorExteriorActions(IElevatorService elevatorService, ICallPanel callPanel, IElevatorControls controls) : base(elevatorService, callPanel, controls)
        {
        }

        public async Task PushGoingUpButtonAsync()
        {
            if (callPanel.Floor == ElevatorService.TotalFloors) throw new Exception("No up button on top floor");
            if (inElevator || callPanel == null) throw new Exception("You are not in an elevator");
            await ElevatorService.UpCallRequestAsync(callPanel.Floor).ConfigureAwait(false);
        }

        public async Task PushGoingDownButtonAsync()
        {
            if (callPanel.Floor == 1) throw new Exception("No down button on first floor");
            if (inElevator || callPanel == null) throw new Exception("You are not in an elevator");
            await ElevatorService.DownCallRequestAsync(callPanel.Floor).ConfigureAwait(false);
        }
    }
}