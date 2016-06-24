using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using Timer = System.Timers.Timer;

namespace Domain
{
    public class ElevatorService : IElevatorService
    {
        private Timer timer = new Timer();
        private volatile bool requestStop;
        private IElevator elevator;
        private readonly IElevatorInteriorInterface interiorInterface;
        public readonly HashSet<int> upQueue = new HashSet<int>();
        public readonly HashSet<int> downQueue = new HashSet<int>();
        private readonly List<IExternalCallInterface> externalCallInterfaces;
        private string currentDirection = "";
        private readonly ElevatorServiceUtilities elevatorServiceUtilities;

        public int CurrentFloor { get; set; }
        public int TotalFloors { get; private set; }

        // TODO: move all utilities into single class and make them public so I can test them properlly.
        public ElevatorService(int totalFloors, List<IExternalCallInterface> externalCallInterfaces, IElevator elevator, IElevatorInteriorInterface interiorInterface)
        {
            CurrentFloor = 1;
            TotalFloors = totalFloors;
            this.externalCallInterfaces = externalCallInterfaces;
            this.elevator = elevator;
            this.interiorInterface = interiorInterface;
            SetupTimer();
            elevatorServiceUtilities = new ElevatorServiceUtilities(this);
            // TODO: need better way to get floor interfaces while still being able to mock them
        }

        private void SetupTimer()
        {
            timer.Interval = 100;
            timer.Elapsed += PerformElevatorActionIteration;
            timer.AutoReset = false;
        }

        private async void PerformElevatorActionIteration(object sender, ElapsedEventArgs e)
        {
            // TODO: In elevator design like this it can be tricky to make sure that a customer will never actually get isolated, no matter how busy it is. 
            // Better do some reasearch and have someone else double check this to make sure I've completely avoided that scenario
            // Note: shouldn't need a lock due to this restarting its own timer, but I need to have someone cdouble check this
            if (currentDirection != "")
            {
                await PerformNecessaryDoorOperations().ConfigureAwait(false);
                ClearDirectionIfNecessary();
                await PerformNecessaryMovement().ConfigureAwait(false);
            }
            DetermineNewDirection();

            if (!requestStop)
            {
                timer.Start(); //restart the timer
            }
        }

        private void DetermineNewDirection()
        {
            if (downQueue.Contains(CurrentFloor))
            {
                currentDirection = "down";
            } else if (upQueue.Contains(CurrentFloor))
            {
                currentDirection = "up";
            } else
            {
                currentDirection = elevatorServiceUtilities.SelectMostAppropriateDirectionBasedOnHeuristic();
            }
        }

        private void ClearDirectionIfNecessary()
        {
          
            if (currentDirection.Equals("up"))
            {
                if (elevatorServiceUtilities.IsAtApex())
                {
                    currentDirection = "";
                }
            } else if (currentDirection.Equals("down"))
            {
                if (elevatorServiceUtilities.IsAtNadir())
                {
                    currentDirection = "";
                }
            }
        }

        // TODO: This grammer is bugging me. Why does an feel right in one situation and a in the other?

        private async Task PerformNecessaryDoorOperations()
        {
            if (currentDirection.Equals("up") && upQueue.Contains(CurrentFloor))
            {
                while (upQueue.Contains(CurrentFloor)) // allows user(s) to re-open door if they hit the button again before door operation completes
                {
                    upQueue.Remove(CurrentFloor);
                    await GetExternalCallInterfaceForFloor(CurrentFloor).DoorOpenEventHandlerAsync().ConfigureAwait(false);
                    Thread.Sleep(3000); // TODO: this is a magic value. Should be set in configuration somewhere
                    await GetExternalCallInterfaceForFloor(CurrentFloor).DoorCloseEventHandlerAsync().ConfigureAwait(false);
                    
                }

            } else if (currentDirection.Equals("down") && downQueue.Contains(CurrentFloor))
            {
                while (downQueue.Contains(CurrentFloor))
                {
                    downQueue.Remove(CurrentFloor);
                    await GetExternalCallInterfaceForFloor(CurrentFloor).DoorOpenEventHandlerAsync().ConfigureAwait(false);
                    Thread.Sleep(3000); // TODO: this is a magic value. Should be set in configuration somewhere
                    await GetExternalCallInterfaceForFloor(CurrentFloor).DoorCloseEventHandlerAsync().ConfigureAwait(false);
                }
            }
        }

        private async Task UpdateFloorDisplays()
        {
            foreach (var floorInterface in externalCallInterfaces)
            {
                await floorInterface.FloorChangeEventHandlerAsync(CurrentFloor).ConfigureAwait(false);
            }
        }

        private async Task PerformNecessaryMovement()
        {
            switch (currentDirection)
            {
                case "":
                    return;
                case "up":
                    await elevator.MoveUpAsync().ConfigureAwait(false);
                    CurrentFloor++;
                    await UpdateFloorDisplays().ConfigureAwait(false);
                    break;
                default:
                    await elevator.MoveDownAsync().ConfigureAwait(false);
                    CurrentFloor--;
                    await UpdateFloorDisplays().ConfigureAwait(false);
                    break;
            }
        }
        
        public Task UpCallRequestAsync(int floor)
        {
            upQueue.Add(floor);
            return Task.FromResult(0);

        }

        public Task DownCallRequestAsync(int floor)
        {
            downQueue.Add(floor);
            return Task.FromResult(0);
        }

        public IExternalCallInterface GetExternalCallInterfaceForFloor(int i)
        {
            return externalCallInterfaces[i - 1];
        }

        public Task StopAsync()
        {
            requestStop = true;
            timer.Stop();
            return Task.FromResult(0);
        }

        public Task StartAsync()
        {
            requestStop = false;
            timer.Start();
            return Task.FromResult(0);
        }

    }

}