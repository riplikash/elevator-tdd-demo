using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ApplicationServices;

namespace ElevatorConsoleApplication.Engine
{
    public class Engine
    {
        private readonly IElevatorExteriorActions availableActions;
        private readonly IElevatorInteriorActions interiorActions;

        public Engine(IElevatorExteriorActions availableActions, IElevatorInteriorActions interiorActions)
        {
            this.availableActions = availableActions;
            this.interiorActions = interiorActions;
        }

        public async Task ExteriorScenarioRun()
        {
            List<string> options = new List<string> {"Press Up Button", "Press Down Button", "exit"};
            int choice = 0;
            while (choice != 3)
            {
                Console.Clear();
                Console.WriteLine("You are outsideof an elevator. A sign says you are on " + availableActions.CheckSurroundings());
                choice = ConsoleUtilities.GetChoiceFromUser("You see a panel with two buttons", options.ToArray());
                await availableActions.EnterDoorWhenItOpensAsync(CancellationToken.None).ConfigureAwait(false);
                switch (choice)
                {
                    case 1:
                        await availableActions.PushGoingUpButtonAsync().ConfigureAwait(false);
                        await Task.Delay(3000).ConfigureAwait(false);
                        await InteriorScenarioRun().ConfigureAwait(false);
                        break;
                    case 2:
                        await availableActions.PushGoingDownButtonAsync().ConfigureAwait(true);
                        await Task.Delay(3000).ConfigureAwait(false);
                        await InteriorScenarioRun().ConfigureAwait(false);
                        break;
                    case 3:
                        return;
                }
                
                
                
                
            }
        }

        private async Task InteriorScenarioRun()
        {
            Console.Clear();
            Console.WriteLine(
                $"You are inside of an elevator. The panel says you are on {availableActions.CheckSurroundings()}.");
            List<string> options = new List<string> { "1", "2", "3", "4", "5" };
            var choice = ConsoleUtilities.GetChoiceFromUser("You see a panel with two buttons", options.ToArray());
            await interiorActions.PushButtonNumberAsync(choice).ConfigureAwait(false);
            await interiorActions.EnterDoorWhenItOpensAsync(CancellationToken.None).ConfigureAwait(false);
        }

    }
}
