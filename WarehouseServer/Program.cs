using System;
using System.Runtime.InteropServices;
using System.Threading;

namespace WarehouseServer
{
    public class Program
    {
        private static Server _server = new Server();

        static void Main(string[] args)
        {
            Program p = new Program();
            p.Run();
        }

        private void Run()
        {
            Console.WriteLine("Server running. Reading MSMQ messages.");
            _server.Run();
        }
    }
}
