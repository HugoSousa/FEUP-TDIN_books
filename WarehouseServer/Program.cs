using System;
using System.Collections.Generic;
using System.Linq;
using System.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace WarehouseServer
{
    class Program
    {
        static void Main(string[] args)
        {
            Server server = new Server();
            Console.WriteLine("Reading messages from warehouse_books queue. Ctrl + C to stop updating.");
            while (true)
            {   
                server.ReadMessage();
            }
        }
    }
}
