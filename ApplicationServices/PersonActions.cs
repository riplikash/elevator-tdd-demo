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

        public void EnterDoor()
        {
            if (inElevator)
            {
                CallPanel = elevatorService.GetCallPanelForFloor(elevatorService.CurrentFloor);
                if (CallPanel.IsDoorOpen)
                {
                    Console.WriteLine($"Door is open. Exiting onto floor {CallPanel.Floor}");
                    inElevator = false;
                    CallPanel = elevatorService.GetCallPanelForFloor(elevatorService.CurrentFloor);
                }
                else
                {
                    Console.WriteLine("Door is not open");
                }
            }
            else
            {
                if (CallPanel.IsDoorOpen)
                {
                    Console.WriteLine($"Door is open. Entering door on floor {CallPanel.Floor}");
                    inElevator = true;
                    CallPanel = null;
                }
                else
                {
                    Console.WriteLine("Door is not open");
                }
            }
        }

        public ICallPanel CallPanel
        {
            get { return Volatile.Read(ref callPanel); }
            set { Volatile.Write(ref callPanel, value); }
        }

    }
}