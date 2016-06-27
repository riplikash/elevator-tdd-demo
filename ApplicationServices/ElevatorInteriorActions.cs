using System;
using System.Threading;
using System.Threading.Tasks;
using Domain;

namespace ApplicationServices
{
    public class ElevatorInteriorActions : IPersonActions, IElevatorInteriorActions
    {
        private readonly IElevatorService elevatorService;
        private readonly IElevatorControls controls;
        private readonly IPersonActions personActions;

        public ElevatorInteriorActions(
            IElevatorService elevatorService,
            IElevatorControls controls,
            IPersonActions personActions)
        {
            if (controls == null) throw new ArgumentNullException(nameof(controls));
            this.elevatorService = elevatorService;
            this.controls = controls;
            this.personActions = personActions;
        }

        public string CheckCurrentFloorAsync()
        {
            return elevatorService.CurrentFloor.ToString();
        }

       
        public async Task PushButtonNumberAsync(int desiredFloor)
        {
            if (InElevator() == false) throw new Exception("You are not in an elevator");
            if (elevatorService.CurrentFloor > desiredFloor)
            {
                await elevatorService.DownCallRequestAsync(desiredFloor).ConfigureAwait(false);
            }
            else
            {
                await elevatorService.UpCallRequestAsync(desiredFloor).ConfigureAwait(false);
            }
            personActions.CallPanel = elevatorService.GetCallPanelForFloor(desiredFloor);
        }

        private bool InElevator()
        {
            return !personActions.CheckSurroundings().StartsWith("F");
        }

        public string CheckElevatorPosition()
        {
            return personActions.CheckElevatorPosition();
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