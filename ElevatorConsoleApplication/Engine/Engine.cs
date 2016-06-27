using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ApplicationServices;

namespace ElevatorConsoleApplication.Engine
{
    public class Engine
    {
        private readonly IElevatorExteriorActions exteriorActions;
        private readonly IElevatorInteriorActions interiorActions;

        public Engine(IElevatorExteriorActions exteriorActions, IElevatorInteriorActions interiorActions)
        {
            this.exteriorActions = exteriorActions;
            this.interiorActions = interiorActions;
        }

        public async Task MainLoop()
        {
            while (true)
            {
                Console.WriteLine(exteriorActions.CheckSurroundings());
                if (exteriorActions.CheckSurroundings().Equals("In elevator"))
                {
                    await RunInteriorScenario().ConfigureAwait(false);
                } else
                {
                    
                    if (exteriorActions.CheckSurroundings().EndsWith("5"))
                    {
                        await RunTopFloorScenario().ConfigureAwait(false);
                    } else if (exteriorActions.CheckSurroundings().EndsWith("1"))
                    {
                        await RunFirstFloorScenario().ConfigureAwait(false);
                    }
                    else
                    {
                        await RunExteriorScenario().ConfigureAwait(false);
                        
                    }
                        

                }
            }

            
        }

        private async Task RunInteriorScenario()
        {
            int choice = 0;
            List<string> options = new List<string> {"1", "2", "3", "4", "5"};
            if (interiorActions.CallPanel != null) options.Add("Exit elevator");
            choice = ConsoleUtilities.GetChoiceFromUser("You see a panel with two buttons", options.ToArray());
            Console.WriteLine("");
            if (choice < 6) await interiorActions.PushButtonNumberAsync(choice).ConfigureAwait(false);
            else interiorActions.EnterDoor();
        }


        private async Task RunTopFloorScenario() 
        {
            List<string> options = new List<string> { "Press Down Button" };
            if (interiorActions.CallPanel != null) options.Add("Enter elevator");
            var choice = ConsoleUtilities.GetChoiceFromUser("You see a panel with one button", options.ToArray());
            Console.WriteLine("");
            switch (choice)
            {
                case 1:
                    await exteriorActions.PushGoingDownButtonAsync().ConfigureAwait(false);
                    break;
                case 2:
                    exteriorActions.EnterDoor();
                    break;
            }
        }

        private async Task RunFirstFloorScenario()
        {
            List<string> options = new List<string> { "Press Up Button" };
            if (interiorActions.CallPanel != null) options.Add("Enter elevator");
            var choice = ConsoleUtilities.GetChoiceFromUser("You see a panel with one button", options.ToArray());
            Console.WriteLine("");
            switch (choice)
            {
                case 1:
                    await exteriorActions.PushGoingUpButtonAsync().ConfigureAwait(false);
                    break;
                case 2:
                    exteriorActions.EnterDoor();
                    break;
            }
            
        }

        private async Task RunExteriorScenario()
        {
            List<string> options = new List<string> {"Press Up Button", "Press Down Button"};
            if (interiorActions.CallPanel != null) options.Add("Enter elevator");
            var choice = ConsoleUtilities.GetChoiceFromUser("You see a panel with two buttons", options.ToArray());
            Console.WriteLine("");
            switch (choice)
            {
                case 1:
                    await exteriorActions.PushGoingUpButtonAsync().ConfigureAwait(false);
                    break;
                case 2:
                    await exteriorActions.PushGoingDownButtonAsync().ConfigureAwait(false);
                    break;
                case 3:
                    exteriorActions.EnterDoor();
                    break;
            }
        }
    }
}
