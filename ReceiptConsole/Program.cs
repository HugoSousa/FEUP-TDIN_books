using System;
using System.ServiceModel;
using ReceiptConsole.StoreService;

namespace ReceiptConsole
{
    public class Program
    {
        private static OrderServiceClient _proxy;


        static void Main(string[] args)
        {
            Program p = new Program();
            Console.Write("Name of printer: ");
            string name = Console.ReadLine();

            p.Start(name);
            Console.WriteLine("Type E to turn this printer off and C to clear the log.");
            while (true)
            {
                string input = Console.ReadLine();
                if (StringComparer.CurrentCultureIgnoreCase.Equals(input, "E"))
                {
                    _proxy.Unsubscribe();
                    return;
                }
                else if (StringComparer.CurrentCultureIgnoreCase.Equals(input, "C"))
                {
                    Console.Clear();
                    Console.WriteLine("Name of printer: {0}", name);
                    Console.WriteLine("Type E to turn this printer off and C to clear the log.");
                }
            }
        }

        public void Start(string name)
        {
            OrderCallback callback = new OrderCallback(this);
            InstanceContext instanceContext = new InstanceContext(callback);
            _proxy = new OrderServiceClient(instanceContext);
            _proxy.Subscribe(name);
        }

        public void PrintReceipt(Receipt receipt)
        {
            Console.WriteLine("\nReceipt\n");
            Console.WriteLine("Client: {0}\n", receipt.Client);
            Console.WriteLine("Book: {0}\n", receipt.Title);
            Console.WriteLine("Quantity: {0}\n", receipt.Quantity);
            Console.WriteLine("Price: {0}\n\n", receipt.TotalPrice);
            Console.WriteLine("--------------------------------\n");
        }

    }

    [CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, UseSynchronizationContext = false)]
    public class OrderCallback : IOrderServiceCallback
    {
        private readonly Program _program;

        public OrderCallback(Program program)
        {
            _program = program;
        }

        public void OnSuccessfullSell()
        {}

        public void OnSucessfullStockUpdate()
        {
        }

        public void OnPrint(Receipt receipt)
        {
            _program.PrintReceipt(receipt);
        }
    }
}
