using System;
using System.Messaging;
using System.ServiceModel;
using OrderStore;
using WarehouseServer.WarehouseService;

namespace WarehouseServer
{
    public class Server
    {
        private readonly MessageQueue _warehouseQueue;
        private readonly string _queueName = "warehouse_books";
        private readonly string _machineName = ".";

        private WarehouseServiceClient _proxy;

        public Server()
        {
            _warehouseQueue = new MessageQueue("FormatName:DIRECT=OS:" + _machineName + "\\Private$\\" + _queueName);
            _warehouseQueue.Formatter = new XmlMessageFormatter(new Type[] { typeof(BookOrder) });
            _warehouseQueue.MessageReadPropertyFilter.SetAll();
            _proxy = new WarehouseServiceClient(new InstanceContext(new MyServiceCallback()));
            _proxy.Subscribe();
        }

        public void ReadMessage()
        {
            try
            {
                Message message = _warehouseQueue.Receive();

                Console.WriteLine("Got message");

                if (message != null)
                {
                    BookOrder body = (BookOrder) message.Body;

                    if (message.Label.Equals("StoreRequest"))
                    {
                        if (_proxy.AddRequest(body.Title, body.Quantity, message.SentTime, message.ArrivedTime, body.OrderId) == 0)
                        {
                            Console.WriteLine("Added Request");
                        }
                    }
                }
            }
            catch (MessageQueueException e)
            {
                Console.WriteLine(e);
                return;
            }
            catch (InvalidCastException e)
            {
                Console.WriteLine("Message with invalid format was consumed.");
                return;
            }
        }

        public void Run()
        {
            while (true)
            {
                ReadMessage();
            }
        }
    }

    [CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, UseSynchronizationContext = false)]
    public class MyServiceCallback : IWarehouseServiceCallback
    {
        public void OnCallback()
        {
        }
    }
}
