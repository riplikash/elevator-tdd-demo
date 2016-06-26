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


        public async Task EnterDoorWhenItOpensAsync()
        {
            var cts = new CancellationTokenSource();
            cts.CancelAfter(11000); // TODO: make configurable. This is based on the curret setup of 5 floors
                // TODO: handle cancellation and timeout
            await EnterDoorWaitMode(cts.Token).ConfigureAwait(false);
        }

        private readonly Timer timer = new Timer();

        private Task EnterDoorWaitMode(CancellationToken token)
        {
            // TODO: Really should be timer based, but this will work for now
            while (!token.IsCancellationRequested)
            {
                if (inElevator && callPanel != null)
                {
                    if (callPanel.IsDoorOpen)
                    {
                        inElevator = false;
                        return Task.FromResult(0);
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
                        return Task.FromResult(0);
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