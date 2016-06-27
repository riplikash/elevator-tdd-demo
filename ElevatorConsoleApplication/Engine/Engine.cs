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

        public async Task MainLoop()
        {
            int choice = 0;
            while (choice != 99)
            {
                Console.WriteLine("DOIT" + availableActions.CheckSurroundings());
                if (availableActions.CheckSurroundings().Equals("In elevator"))
                {
                    List<string> interiorButtons = new List<string> { "1", "2", "3", "4", "5" };
                    choice = ConsoleUtilities.GetChoiceFromUser("You see a panel with two buttons", interiorButtons.ToArray());
                    Console.WriteLine("");
                    await interiorActions.PushButtonNumberAsync(choice).ConfigureAwait(false);
                    await interiorActions.EnterDoorWhenItOpensAsync(CancellationToken.None).ConfigureAwait(true);
                    while (interiorActions.CheckElevatorPositionAsync() != choice.ToString())
                    {
                        await Task.Delay(500).ConfigureAwait(false);
                    }
                } else
                {
                    List<string> exteriorOptions = new List<string> { "Press Up Button", "Press Down Button" };

                        choice = ConsoleUtilities.GetChoiceFromUser("You see a panel with two buttons", exteriorOptions.ToArray());
                        Console.WriteLine("");
                        await availableActions.EnterDoorWhenItOpensAsync(CancellationToken.None).ConfigureAwait(false);
                        switch (choice)
                        {
                            case 1:
                                await availableActions.PushGoingUpButtonAsync().ConfigureAwait(false);
                                await Task.Delay(3000).ConfigureAwait(false);
                                break;
                            case 2:
                                await availableActions.PushGoingDownButtonAsync().ConfigureAwait(true);
                                await Task.Delay(3000).ConfigureAwait(false);
                                break;
                            case 3:
                                return;
                        }

                }
                await Task.Delay(3000).ConfigureAwait(false);
            }

            
        }

    }
}
