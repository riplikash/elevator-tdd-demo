using System;
using System.Threading;
using System.Threading.Tasks;
using Domain;

namespace ApplicationServices
{
    public class ElevatorExteriorActions :  IElevatorExteriorActions
    {
        private readonly IElevatorService elevatorService;
        private readonly ICallPanel panel;
        private readonly IPersonActions personActions;

        public ElevatorExteriorActions(IElevatorService elevatorService, IPersonActions personActions)
        {
            this.elevatorService = elevatorService;
            this.personActions = personActions;
            panel = this.elevatorService.GetCallPanelForFloor(1);
        }

        public async Task PushGoingUpButtonAsync()
        {
            if (panel.Floor == elevatorService.TotalFloors) throw new Exception("No up button on top floor");
            if (InElevator() || panel == null) throw new Exception("You are not in an elevator");
            await elevatorService.UpCallRequestAsync(panel.Floor).ConfigureAwait(false);
        }

        private bool InElevator()
        {
            return !personActions.CheckSurroundings().StartsWith("F");
        }

        public async Task PushGoingDownButtonAsync()
        {
            if (panel.Floor == 1) throw new Exception("No down button on first floor");
            if (InElevator() || panel == null) throw new Exception("You are not in an elevator");
            await elevatorService.DownCallRequestAsync(panel.Floor).ConfigureAwait(false);
        }

        public string CheckElevatorPosition()
        {
            return personActions.CheckSurroundings();
        }

        public string CheckSurroundings()
        {
            return personActions.CheckSurroundings();
        }

        public Task EnterDoorWhenItOpensAsync(CancellationToken token)
        {
            return personActions.EnterDoorWhenItOpensAsync(token);
        }

        public ICallPanel CallPanel
        {
            get { return personActions.CallPanel; }
            set { personActions.CallPanel = value; }
        }
    }
}