using System;
using System.Threading.Tasks;
using Domain;

namespace ApplicationServices
{
    public class ElevatorInteriorActions : PersonActions, IElevatorInteriorActions
    {
        public ElevatorInteriorActions(
            IElevatorService elevatorService,
            ICallPanel callPanel,
            IElevatorControls controls) : base(elevatorService, callPanel, controls)
        {
        }

        public string CheckCurrentFloorAsync()
        {
            return base.CheckElevatorPositionAsync();
        }

        public async Task PushButton1()
        {
            await PushButtonNumber(1).ConfigureAwait(false);
        }

        public async Task PushButton2()
        {
            await PushButtonNumber(2).ConfigureAwait(false);
        }

        public async Task PushButton3()
        {
            await PushButtonNumber(3).ConfigureAwait(false);
        }

        public async Task PushButton4()
        {
            await PushButtonNumber(4).ConfigureAwait(false);
        }

        public async Task PushButton5()
        {
            await PushButtonNumber(5).ConfigureAwait(false);
        }

        private async Task PushButtonNumber(int floor)
        {
            if (inElevator == false) throw new Exception("You are not in an elevator");
            if (ElevatorService.CurrentFloor > floor)
            {
                await ElevatorService.DownCallRequestAsync(floor).ConfigureAwait(false);
            }
            else
            {
                await ElevatorService.UpCallRequestAsync(floor).ConfigureAwait(false);
            }
            callPanel = ElevatorService.GetCallPanelForFloor(floor);
        }
    }
}