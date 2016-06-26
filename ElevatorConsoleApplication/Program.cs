using System;
using ElevatorConsoleApplication.Startup;

namespace ElevatorConsoleApplication
{
    public static class Program
    {
        static void Main(string[] args)
        {
            AppStart.Run();
            var choice = ConsoleUtilities.GetChoiceFromUser("Choose up or down", "Up", "Down");
            Console.WriteLine();
            Console.WriteLine("The Value You entered is : " + choice);
            Console.ReadKey();
        }

    }
}
