using System;
using System.Threading;
using System.Threading.Tasks;
using Domain;

namespace ApplicationServices
{
    public class PersonActions : IPersonActions
    {
        private readonly IElevatorService elevatorService;
        private bool inElevator;
        private ICallPanel callPanel;

        public PersonActions(IElevatorService elevatorService)
        {
            this.elevatorService = elevatorService;
            this.CallPanel = elevatorService.GetCallPanelForFloor(1);
        }


        public string CheckElevatorPosition()
        {
            return elevatorService.CurrentFloor.ToString();
        }

        public string CheckSurroundings()
        {
            if (inElevator)
            {
                return "In elevator";
            } 
            return $"Floor {CallPanel.Floor}";
        }

        public ICallPanel CallPanel
        {
            get { return Volatile.Read(ref callPanel); }
            set { Volatile.Write(ref callPanel, value); }
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
                if (inElevator && CallPanel != null)
                {
                    if (CallPanel.IsDoorOpen)
                    {
                        inElevator = false;
                        return;
                    }
                }
                else if (inElevator & CallPanel == null)
                {
                    throw new ArgumentException("You haven't selected a floor");
                }
                else if (!inElevator && CallPanel != null)
                {
                    if (CallPanel.IsDoorOpen)
                    {
                        Console.WriteLine($"Door is open. Entering door on floor {CallPanel.Floor}");
                        inElevator = true;
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