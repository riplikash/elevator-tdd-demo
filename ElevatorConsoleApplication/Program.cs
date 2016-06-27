using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using ElevatorConsoleApplication.Startup;
using Nito.AsyncEx;

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
            finally
            {
                Console.ReadKey();
            }
        }

        static async Task<int> MainAsync(string[] args)
        {
            try
            {
                var app = new AppStart();
                await app.Run().ConfigureAwait(false);
                
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return await Task.FromResult(0).ConfigureAwait(false);
        }

    }
}
