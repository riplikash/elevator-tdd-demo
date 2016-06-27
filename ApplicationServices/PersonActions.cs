using System;
using System.Threading;
using System.Threading.Tasks;
using Domain;

namespace ApplicationServices
{
    public class PersonActions : IPersonActions
    {
        internal readonly IElevatorService ElevatorService;
        internal static ICallPanel CallPanel;
        private readonly IElevatorControls controls;
        internal static bool InElevator;

        public PersonActions(IElevatorService elevatorService, ICallPanel callPanel, IElevatorControls controls)
        {
            this.ElevatorService = elevatorService;
            PersonActions.CallPanel = callPanel;
            this.controls = controls;
        }


        public string CheckElevatorPositionAsync()
        {
            return ElevatorService.CurrentFloor.ToString();
        }

        public string CheckSurroundings()
        {
            if (InElevator)
            {
                return "In elevator";
            } 
            return $"Floor {CallPanel.Floor}";
        }



        // TODO : Put cancellation token here and let UI handle the cancellation
        public Task EnterDoorWhenItOpensAsync(CancellationToken token)
        {
            return Task.Run(() => EnterDoorWaitModeAsync(token), token); 
        }

        private async void EnterDoorWaitModeAsync(CancellationToken token)
        {
            // TODO: Really should be timer based, but this will work for now 
            while (!token.IsCancellationRequested)
            {
                if (InElevator && CallPanel != null)
                {
                    if (CallPanel.IsDoorOpen)
                    {
                        InElevator = false;
                        return;
                    }
                }
                else if (InElevator & CallPanel == null)
                {
                    throw new ArgumentException("You haven't selected a floor");
                }
                else if (!InElevator && CallPanel != null)
                {
                    Console.WriteLine("Attempting to enter door at floor" + CallPanel.Floor);
                    if (CallPanel.IsDoorOpen)
                    {
                        Console.WriteLine("Door is open. Entering door.");
                        InElevator = true;
                        return;
                    }
                }
                else
                {
                    throw new Exception("Invalid state");
                }
                await Task.Delay(300, token).ConfigureAwait(false);
            }
            throw new OperationCanceledException();
        }
    }
}