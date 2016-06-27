using System;
using System.Threading;
using System.Threading.Tasks;
using Domain;

namespace ApplicationServices
{
    public class ElevatorExteriorActions :  IElevatorExteriorActions
    {
        private readonly IElevatorService elevatorService;
        private readonly IPersonActions personActions;

        public ElevatorExteriorActions(IElevatorService elevatorService, IPersonActions personActions)
        {
            this.elevatorService = elevatorService;
            this.personActions = personActions;
        }

        public async Task PushGoingUpButtonAsync()
        {
            if (personActions.CallPanel.Floor == elevatorService.TotalFloors) throw new Exception("No up button on top floor");
            if (InElevator() || personActions.CallPanel == null) throw new Exception("You are not in an elevator");
            await elevatorService.UpCallRequestAsync(personActions.CallPanel.Floor).ConfigureAwait(false);
        }

        private bool InElevator()
        {
            return !personActions.CheckSurroundings().StartsWith("F");
        }

        public async Task PushGoingDownButtonAsync()
        {
            if (personActions.CallPanel.Floor == 1) throw new Exception("No down button on first floor");
            if (InElevator() || personActions.CallPanel == null) throw new Exception("You are not in an elevator");
            await elevatorService.DownCallRequestAsync(personActions.CallPanel.Floor).ConfigureAwait(false);
        }

        public string CheckElevatorPosition()
        {
            return personActions.CheckSurroundings();
        }

        public string CheckSurroundings()
        {
            return personActions.CheckSurroundings();
        }

        public void EnterDoor()
        {
            personActions.EnterDoor();
        }

        public ICallPanel CallPanel
        {
            get { return personActions.CallPanel; }
            set { personActions.CallPanel = value; }
        }
    }
}