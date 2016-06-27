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
                        return;
                    }
                        

                }
                await Task.Delay(3000).ConfigureAwait(false);
                Console.Clear();
            }

            
        }

        private async Task RunInteriorScenario()
        {
            int choice = 0;
            List<string> interiorButtons = new List<string> {"1", "2", "3", "4", "5"};
            choice = ConsoleUtilities.GetChoiceFromUser("You see a panel with two buttons", interiorButtons.ToArray());
            Console.WriteLine("");
            await interiorActions.EnterDoorWhenItOpensAsync(CancellationToken.None).ConfigureAwait(true);
            await interiorActions.PushButtonNumberAsync(choice).ConfigureAwait(false);
            while (interiorActions.CheckElevatorPosition() != choice.ToString())
            {
                await Task.Delay(500).ConfigureAwait(false);
            }
        }

        private async Task RunTopFloorScenario()
        {
            List<string> exteriorOptions = new List<string> { "Press Down Button" };
            ConsoleUtilities.GetChoiceFromUser("You see a panel with one button", exteriorOptions.ToArray());
            Console.WriteLine("");
            await exteriorActions.EnterDoorWhenItOpensAsync(CancellationToken.None).ConfigureAwait(false);
            await exteriorActions.PushGoingDownButtonAsync().ConfigureAwait(false);
            await Task.Delay(3000).ConfigureAwait(false);
        }

        private async Task RunFirstFloorScenario()
        {
            List<string> exteriorOptions = new List<string> { "Press Up Button" };
            ConsoleUtilities.GetChoiceFromUser("You see a panel with one button", exteriorOptions.ToArray());
            Console.WriteLine("");
            await exteriorActions.EnterDoorWhenItOpensAsync(CancellationToken.None).ConfigureAwait(false);
            await exteriorActions.PushGoingUpButtonAsync().ConfigureAwait(false);
            await Task.Delay(3000).ConfigureAwait(false);
        }

        private async Task RunExteriorScenario()
        {
            int choice = 0;
            List<string> exteriorOptions = new List<string> {"Press Up Button", "Press Down Button"};
            choice = ConsoleUtilities.GetChoiceFromUser("You see a panel with two buttons", exteriorOptions.ToArray());
            Console.WriteLine("");
            await exteriorActions.EnterDoorWhenItOpensAsync(CancellationToken.None).ConfigureAwait(false);
            switch (choice)
            {
                case 1:
                    await exteriorActions.PushGoingUpButtonAsync().ConfigureAwait(false);
                    await Task.Delay(3000).ConfigureAwait(false);
                    break;
                case 2:
                    await exteriorActions.PushGoingDownButtonAsync().ConfigureAwait(true);
                    await Task.Delay(3000).ConfigureAwait(false);
                    break;
                case 3:
                    return;
            }
            return;
        }
    }
}
