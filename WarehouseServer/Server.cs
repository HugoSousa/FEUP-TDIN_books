using System;
using System.Messaging;
using OrderStore;
using WarehouseServer.WarehouseService;

namespace WarehouseServer
{
    class Server
    {
        private readonly MessageQueue _warehouseQueue;
        private readonly string _queueName = "warehouse_books";
        private readonly string _machineName = ".";

        private WarehouseServiceClient _proxy;

        public Server()
        {
            _warehouseQueue = new MessageQueue("FormatName:DIRECT=OS:" + _machineName + "\\Private$\\" + _queueName);
            _warehouseQueue.Formatter = new XmlMessageFormatter(new Type[] { typeof(BookOrder) });
        }

        public void ReadMessage()
        {
            try
            {
                Message message = _warehouseQueue.Receive();
                BookOrder body = (BookOrder) message.Body;

                if (message.Label.Equals("StoreRequest"))
                    _proxy.AddRequest(body.Title, body.Quantity, message.SentTime, message.ArrivedTime);
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
    }
}
