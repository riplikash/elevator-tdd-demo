using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using Timer = System.Timers.Timer;

namespace Domain
{
    public class ElevatorService : IElevatorService
    {
        // TODO: Change to ConcurrentDictionary
        public HashSet<int> UpCalls { get; } = new HashSet<int>();
        public HashSet<int> DownCalls { get; } = new HashSet<int>();

        private ConcurrentDictionary<int, ICallPanel> ExteriorCallPanels { get; }

        private DirectionEnum currentDirection = DirectionEnum.Stationary; // Doesn't need to be syncronized. Only used by the single update thread

        public int CurrentFloor
        {
            get { return Volatile.Read(ref currentFloor); }
            private set { Volatile.Write(ref currentFloor, value);  }
        }

        public int TotalFloors => ExteriorCallPanels.Count;

        private readonly Timer timer = new Timer();
        private readonly ElevatorServiceUtilities elevatorServiceUtilities;
        private bool requestStop;
        private readonly IElevator elevator;
        private int currentFloor;

        public ElevatorService(IElevator elevator)
        {
            CurrentFloor = 1;
            this.elevator = elevator;
            SetupTimer();
            elevatorServiceUtilities = new ElevatorServiceUtilities(this);
            ExteriorCallPanels = new ConcurrentDictionary<int, ICallPanel>();
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

            if (!Volatile.Read(ref requestStop))
            {
                timer.Start(); //restart the timer
            }
        }

        private void DetermineNewDirection()
        {
            if (DownCalls.Contains(CurrentFloor))
            {
                currentDirection = DirectionEnum.Down;
            } else if (UpCalls.Contains(CurrentFloor))
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
            if (currentDirection.Equals(DirectionEnum.Up) && UpCalls.Contains(CurrentFloor))
            {
                while (UpCalls.Contains(CurrentFloor)) // allows user(s) to re-open door if they hit the button again before door operation completes
                {
                    UpCalls.Remove(CurrentFloor);
                    Console.WriteLine($"Opening door on level {CurrentFloor}");
                    var callPanel = GetCallPanelForFloor(CurrentFloor);
                    await callPanel.DoorOpenEventHandlerAsync().ConfigureAwait(false);
//                    await Task.Delay(3000).ConfigureAwait(false); // TODO: Get this configurable
                    await callPanel.DoorCloseEventHandlerAsync().ConfigureAwait(false);
                    Console.WriteLine($"Closing door on level {CurrentFloor}");

                }

            } else if (currentDirection.Equals(DirectionEnum.Down) && DownCalls.Contains(CurrentFloor))
            {
                while (DownCalls.Contains(CurrentFloor))
                {
                    DownCalls.Remove(CurrentFloor);
                    Console.WriteLine($"Opening door on level {CurrentFloor}");
                    await GetCallPanelForFloor(CurrentFloor).DoorOpenEventHandlerAsync().ConfigureAwait(false);
                    Console.WriteLine($"Closing door on level {CurrentFloor}");
                    await Task.Delay(3000).ConfigureAwait(false); // TODO: this is a magic value. Should be set in configuration somewhere
                    await GetCallPanelForFloor(CurrentFloor).DoorCloseEventHandlerAsync().ConfigureAwait(false);
                }
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
                    Interlocked.Increment(ref currentFloor);
                    Console.WriteLine($"Crrived at {CurrentFloor}");
//                    await UpdateFloorDisplays().ConfigureAwait(false); // TODO: use .net messaging
                    break;
                default:
                    Console.WriteLine($"Moving from floor {CurrentFloor} to {CurrentFloor - 1}");
                    await elevator.MoveDownAsync().ConfigureAwait(false);
                    Interlocked.Decrement(ref currentFloor);
//                    await UpdateFloorDisplays().ConfigureAwait(false);
                    break;
            }
        }
        
        public Task UpCallRequestAsync(int floor)
        {
            UpCalls.Add(floor);
            return Task.CompletedTask;

        }

        public Task DownCallRequestAsync(int floor)
        {
            DownCalls.Add(floor);
            return Task.CompletedTask;
        }

        public ICallPanel GetCallPanelForFloor(int floor)
        {
            return ExteriorCallPanels[floor];
        }

        public Task StopAsync()
        {
            Volatile.Write(ref requestStop, true);
            timer.Stop();
            return Task.CompletedTask;
        }

        public Task StartAsync()
        {
            Volatile.Write(ref requestStop, false);
            timer.Start();
            return Task.CompletedTask;
        }

        public void RegisterCallPanel(ICallPanel newPanel)
        {
            ExteriorCallPanels.TryAdd(newPanel.Floor, newPanel);
        }
    }

    public enum DirectionEnum
    {
        Up, Down, Stationary
    }
}