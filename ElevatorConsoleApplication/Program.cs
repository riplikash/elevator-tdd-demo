using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using ElevatorConsoleApplication.Startup;

namespace ElevatorConsoleApplication
{
    public static class Program
    {
        static int Main(string[] args)
        {
            try
            {
                return AsyncContext.Run(() => MainAsync(args));
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex);
                return -1;
            }
        }

        static async Task<int> MainAsync(string[] args)
        {
        
      }

    }
}
