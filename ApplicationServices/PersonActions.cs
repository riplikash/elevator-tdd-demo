using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using Domain;
using Timer = System.Timers.Timer;

namespace ApplicationServices
{
    public class PersonActions : IPersonActions
    {
        internal readonly IElevatorService ElevatorService;
        internal  ICallPanel callPanel;
        internal readonly IElevatorControls controls;
        internal bool inElevator = false;

        public PersonActions(IElevatorService elevatorService, ICallPanel callPanel, IElevatorControls controls)
        {
            this.ElevatorService = elevatorService;
            this.callPanel = callPanel;
            this.controls = controls;
        }

        public string CheckElevatorPositionAsync()
        {
            return ElevatorService.CurrentFloor.ToString();
        }

        public string CheckSurroundings()
        {
            if (inElevator)
            {
                return "In elevator";
            } 
            return $"Floor {callPanel.Floor}";
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
                if (inElevator && callPanel != null)
                {
                    if (callPanel.IsDoorOpen)
                    {
                        inElevator = false;
                        return;
                    }
                }
                else if (inElevator & callPanel == null)
                {
                    throw new ArgumentException("You haven't selected a floor");
                }
                else if (!inElevator && callPanel != null)
                {
                    if (callPanel.IsDoorOpen)
                    {
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