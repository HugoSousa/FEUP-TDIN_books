using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Messaging;
using System.Runtime.InteropServices;

namespace OrderStore 
{

    class Warehouse: System.ComponentModel.ISynchronizeInvoke
    {            
        private MessageQueue WarehouseQueue;
        public string QueueName = "warehouse_books";
        public Warehouse() 
        {
            this.WarehouseQueue = new MessageQueue();
            this.WarehouseQueue.MessageReadPropertyFilter.LookupId = true;
            this.WarehouseQueue.SynchronizingObject = this;
            VerifyQueue();
        }

        private void VerifyQueue()
        {
            if (!MessageQueue.Exists(".\\private$\\" + QueueName))
            {
                Console.WriteLine("does not exist");
                try
                {
                    MessageQueue.Create(".\\private$\\" + QueueName);
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


        public IAsyncResult BeginInvoke(Delegate method, object[] args)
        {
            throw new NotImplementedException();
        }

        public object EndInvoke(IAsyncResult result)
        {
            throw new NotImplementedException();
        }

        public object Invoke(Delegate method, object[] args)
        {
            throw new NotImplementedException();
        }

        public bool InvokeRequired { get; private set; }
    }
}
