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
        public Task EnterDoorWhenItOpensAsync()
        {
            var cts = new CancellationTokenSource();
            cts.CancelAfter(11000); // TODO: make configurable. This is based on the curret setup of 5 floors
                // TODO: handle cancellation and timeout
                // TODO: eventually the cancellation token should be passed in here, but for now I'm just going to configure it on a timeout
            return EnterDoorWaitModeAsync(cts.Token); 
        }

        private Task EnterDoorWaitModeAsync(CancellationToken token)
        {
            // TODO: Really should be timer based, but this will work for now
            while (!token.IsCancellationRequested)
            {
                if (inElevator && callPanel != null)
                {
                    if (callPanel.IsDoorOpen)
                    {
                        inElevator = false;
                        return Task.CompletedTask;
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
                        return Task.CompletedTask;
                    }
                }
                else
                {
                    throw new Exception("Invalid state");
                }
                Thread.Sleep(200);
            }

            throw new OperationCanceledException();
        }
    }
}