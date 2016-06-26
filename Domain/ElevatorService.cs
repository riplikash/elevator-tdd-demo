using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using Timer = System.Timers.Timer;

namespace Domain
{
    // TODO: get timer stuff into infrastructure
    public class ElevatorService : IElevatorService
    {
        public HashSet<int> UpQueue { get; } = new HashSet<int>();
        public HashSet<int> DownQueue { get; } = new HashSet<int>();
        private readonly List<ICallPanel> externalCallInterfaces;
        private DirectionEnum currentDirection = DirectionEnum.Stationary;
        public int CurrentFloor { get; private set; }
        public int TotalFloors { get;  }

        private readonly Timer timer = new Timer();
        private readonly ElevatorServiceUtilities elevatorServiceUtilities;
        private volatile bool requestStop;
        private readonly IElevator elevator;
        private readonly IElevatorControls controls;

        // TODO: total floors can be inferred from ICallPanel list
        public ElevatorService(int totalFloors, List<ICallPanel> externalCallInterfaces, IElevator elevator, IElevatorControls controls)
        {
            CurrentFloor = 1;
            TotalFloors = totalFloors;
            this.externalCallInterfaces = externalCallInterfaces;
            this.elevator = elevator;
            this.controls = controls;
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
            if (currentDirection != DirectionEnum.Stationary)
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
            if (DownQueue.Contains(CurrentFloor))
            {
                currentDirection = DirectionEnum.Down;
            } else if (UpQueue.Contains(CurrentFloor))
            {
                currentDirection = DirectionEnum.Up;
            } else
            {
                currentDirection = elevatorServiceUtilities.SelectMostAppropriateDirectionBasedOnHeuristic();
            }
        }

        private void ClearDirectionIfNecessary()
        {
          
            if (currentDirection.Equals(DirectionEnum.Up))
            {
                if (elevatorServiceUtilities.IsAtApex())
                {
                    currentDirection = DirectionEnum.Stationary;
                }
            } else if (currentDirection.Equals(DirectionEnum.Down))
            {
                if (elevatorServiceUtilities.IsAtNadir())
                {
                    currentDirection = DirectionEnum.Stationary;
                }
            }
        }

        // TODO: This grammer is bugging me. Why does an feel right in one situation and a in the other?

        private async Task PerformNecessaryDoorOperations()
        {
            if (currentDirection.Equals(DirectionEnum.Up) && UpQueue.Contains(CurrentFloor))
            {
                while (UpQueue.Contains(CurrentFloor)) // allows user(s) to re-open door if they hit the button again before door operation completes
                {
                    UpQueue.Remove(CurrentFloor);
                    Console.WriteLine($"Opening door on level {CurrentFloor}");
                    var callPanel = GetCallPanelForFloor(CurrentFloor);
                    await callPanel.DoorOpenEventHandlerAsync().ConfigureAwait(false);
//                    Thread.Sleep(3000); // TODO: Get this configurable
                    await callPanel.DoorCloseEventHandlerAsync().ConfigureAwait(false);
                    Console.WriteLine($"Closing door on level {CurrentFloor}");

                }

            } else if (currentDirection.Equals(DirectionEnum.Down) && DownQueue.Contains(CurrentFloor))
            {
                while (DownQueue.Contains(CurrentFloor))
                {
                    DownQueue.Remove(CurrentFloor);
                    Console.WriteLine($"Opening door on level {CurrentFloor}");
                    await GetCallPanelForFloor(CurrentFloor).DoorOpenEventHandlerAsync().ConfigureAwait(false);
                    Console.WriteLine($"Closing door on level {CurrentFloor}");
                    Thread.Sleep(3000); // TODO: this is a magic value. Should be set in configuration somewhere
                    await GetCallPanelForFloor(CurrentFloor).DoorCloseEventHandlerAsync().ConfigureAwait(false);
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
            // TODO: Get proper logging in instead of console write lines
            switch (currentDirection)
            {
                case DirectionEnum.Stationary:
                    return;
                case DirectionEnum.Up:
                    Console.WriteLine($"Moving up from floor {CurrentFloor}");
                    await elevator.MoveUpAsync().ConfigureAwait(false);
                    CurrentFloor++;
                    Console.WriteLine($"Crrived at {CurrentFloor}");
                    await UpdateFloorDisplays().ConfigureAwait(false);
                    break;
                default:
                    Console.WriteLine($"Moving from floor {CurrentFloor} to {CurrentFloor - 1}");
                    await elevator.MoveDownAsync().ConfigureAwait(false);
                    CurrentFloor--;
                    await UpdateFloorDisplays().ConfigureAwait(false);
                    break;
            }
        }
        
        public Task UpCallRequestAsync(int floor)
        {
            UpQueue.Add(floor);
            return Task.FromResult(0);

        }

        public Task DownCallRequestAsync(int floor)
        {
            DownQueue.Add(floor);
            return Task.FromResult(0);
        }

        public ICallPanel GetCallPanelForFloor(int i)
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

    public enum DirectionEnum
    {
        Up, Down, Stationary
    }
}