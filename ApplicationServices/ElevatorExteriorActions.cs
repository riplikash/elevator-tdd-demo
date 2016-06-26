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
            // TODO: exception checks for if someone ties to push an upCall on top floor
            if (inElevator || callPanel == null) throw new Exception("You are not in an elevator");
            await ElevatorService.UpCallRequestAsync(callPanel.Floor).ConfigureAwait(false);
        }

        public async Task PushGoingDownButtonAsync()
        {
            // TODO: exception if someone tries to ppush downCall on bottom floor
            if (inElevator || callPanel == null) throw new Exception("You are not in an elevator");
            await ElevatorService.DownCallRequestAsync(callPanel.Floor).ConfigureAwait(false);
        }
    }
}