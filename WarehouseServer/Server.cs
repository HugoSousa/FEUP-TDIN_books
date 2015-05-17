using System;
using System.Messaging;
using OrderStore;
using WarehouseServer.WarehouseService;

namespace WarehouseServer
{
    public class Server
    {
        public event EventHandler NewRequest;

        private readonly MessageQueue _warehouseQueue;
        private readonly string _queueName = "warehouse_books";
        private readonly string _machineName = ".";

        private WarehouseServiceClient _proxy;

        public Server()
        {
            _warehouseQueue = new MessageQueue("FormatName:DIRECT=OS:" + _machineName + "\\Private$\\" + _queueName);
            _warehouseQueue.Formatter = new XmlMessageFormatter(new Type[] { typeof(BookOrder) });
            _warehouseQueue.MessageReadPropertyFilter.SetAll();
            _proxy = new WarehouseServiceClient();
        }

        public void ReadMessage()
        {
            try
            {
                Message message = _warehouseQueue.Receive();
                
                BookOrder body = (BookOrder) message.Body;

                if (message.Label.Equals("StoreRequest"))
                {
                    if(_proxy.AddRequest(body.Title, body.Quantity, message.SentTime, message.ArrivedTime, body.OrderId) == 0)
                        OnNewRequest(null);
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

        protected virtual void OnNewRequest(EventArgs e)
        {
            EventHandler handler = NewRequest;
            if (handler != null)
            {
                handler(this, e);
            }
        }
    }
}
