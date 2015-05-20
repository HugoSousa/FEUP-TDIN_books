using System;
using System.Runtime.InteropServices;
using System.Threading;

namespace WarehouseServer
{
    public class Program
    {
        static bool exitSystem = false;
        private static Server _server = new Server();
        
        #region Trap application termination
        [DllImport("Kernel32")]
        private static extern bool SetConsoleCtrlHandler(EventHandler handler, bool add);

        private delegate bool EventHandler(CtrlType sig);
        static EventHandler _handler;

        enum CtrlType
        {
            CTRL_C_EVENT = 0,
            CTRL_BREAK_EVENT = 1,
            CTRL_CLOSE_EVENT = 2,
            CTRL_LOGOFF_EVENT = 5,
            CTRL_SHUTDOWN_EVENT = 6
        }

        private static bool Handler(CtrlType sig)
        {
            Console.WriteLine("Exiting system due to external CTRL-C, or process kill, or shutdown");

            _server.Unsubscribe();

            Console.WriteLine("Cleanup complete");

            //allow main to run off
            exitSystem = true;

            //shutdown right away so there are no lingering threads
            Environment.Exit(-1);

            return true;
        }
        #endregion
        

        static void Main(string[] args)
        {
            _handler += new EventHandler(Handler);
            SetConsoleCtrlHandler(_handler, true);

            Program p = new Program();
            p.Run();
        }

        private void Run()
        {
            Console.WriteLine("Server running. Reading MSMQ messages.");
            _server.Run();

            while (!exitSystem)
            {
                Thread.Sleep(500);
            }
        }
    }
}
