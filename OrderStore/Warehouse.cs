using System;
using System.Messaging;

namespace OrderStore 
{

    class Warehouse
    {            
        private readonly MessageQueue _warehouseQueue;
        private readonly string _queueName = "warehouse_books";
        private readonly string _machineName = ".";
        public Warehouse() 
        {
            this._warehouseQueue = new MessageQueue("FormatName:DIRECT=OS:" + _machineName + "\\Private$\\" + _queueName);
            this._warehouseQueue.MessageReadPropertyFilter.LookupId = true;
            this._warehouseQueue.Formatter = new XmlMessageFormatter(new Type[] { typeof(BookOrder) });
            //check if queue exists only works for local queues, not remote ones
            //VerifyQueue();
        }

        private void VerifyQueue()
        {
            if (!MessageQueue.Exists("FormatName:DIRECT=OS:" + _machineName + "\\Private$\\" + _queueName))
            {
                try
                {
                    MessageQueue.Create("FormatName:DIRECT=OS:" + _machineName + "\\Private$\\" + _queueName);
                }
                catch (MessageQueueException ex)
                {
                    Console.WriteLine(ex.Message);
                }
                catch (Exception ex1)
                {
                    Console.WriteLine(ex1.Message);

                }
            }
        }

        public static void SendMessage(BookOrder order)
        {
            Warehouse wh = new Warehouse();
            wh.Send(order);
        }

        public void Send(BookOrder body)
        {
            try
            {
                _warehouseQueue.Send(body, "StoreRequest");
            }
            catch (MessageQueueException e)
            {
                Console.WriteLine(e);
            }
        }
    }
}
