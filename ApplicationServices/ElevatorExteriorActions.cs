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

        public Task PushGoingUpButtoAsync()
        {
            throw new NotImplementedException();
        }

        public Task PushGoingDownButtonAsync()
        {
            throw new NotImplementedException();
        }
    }
}