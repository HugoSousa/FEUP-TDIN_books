using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.ServiceModel;
using System.Threading;
using WarehouseService.StoreService;

namespace WarehouseService
{
    // NOTE: In order to launch WCF Test Client for testing this service, please select WarehouseServ.svc or WarehouseServ.svc.cs at the Solution Explorer and start debugging.
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Reentrant, InstanceContextMode = InstanceContextMode.Single)]
    public class WarehouseServ : IWarehouseService
    {
        //public static IMyServiceCallback Callback;

        private List<IMyServiceCallback> _subscribers = new List<IMyServiceCallback>();
        private readonly object _locker = new object();

        public DataTable GetOpenRequests()
        {
            DataTable result = new DataTable("Request");

            using (SqlConnection c = new SqlConnection(ConfigurationManager.ConnectionStrings["warehouse_db"].ConnectionString))
            {
                try
                {
                    c.Open();
                    string sql = "select * from [Request] where ship_date is null";
                    SqlCommand cmd = new SqlCommand(sql, c);
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(result);
                }
                catch (SqlException e)
                {
                    return null;
                }
                finally
                {
                    c.Close();
                }
            }

            return result;
        }

        public int AddRequest(string title, int quantity, DateTime sent, DateTime received, int orderId)
        {
            using (SqlConnection c = new SqlConnection(ConfigurationManager.ConnectionStrings["warehouse_db"].ConnectionString))
            {
                try
                {
                    c.Open();
                    string sql = "insert into [Request] (title, quantity, request_sent_date, request_received_date, order_id) " +
                                 "values (@title, @quantity, @sent, @received, @order_id)";
                    SqlCommand cmd = new SqlCommand(sql, c);
                    cmd.Parameters.Add("@title", SqlDbType.NVarChar, 50).Value = title;
                    cmd.Parameters.Add("@quantity", SqlDbType.Int).Value = quantity;
                    cmd.Parameters.Add("@sent", SqlDbType.DateTime2, 7).Value = sent;
                    cmd.Parameters.Add("@received", SqlDbType.DateTime2, 7).Value = received;
                    cmd.Parameters.Add("@order_id", SqlDbType.Int).Value = orderId;
                    cmd.ExecuteNonQuery();
                }
                catch (SqlException e)
                {
                    Console.WriteLine(e);
                    return -1;
                }
                finally
                {
                    c.Close();
                }
            }

            foreach (var subscriber in _subscribers)
            {
                var callbackComm = (ICommunicationObject)subscriber;
                if (callbackComm.State == CommunicationState.Opened)
                    subscriber.OnCallback();
            }
            
            return 0;
        }

        public int ShipRequest(int id)
        {
            
            OrderServiceClient serviceStore = new OrderServiceClient();

            using (SqlConnection c = new SqlConnection(ConfigurationManager.ConnectionStrings["warehouse_db"].ConnectionString))
            {
                try
                {
                    c.Open();
                    string sql = "update [Request] set ship_date = @now output inserted.order_id where id = @id";
                    SqlCommand cmd = new SqlCommand(sql, c);
                    cmd.Parameters.Add("@now", SqlDbType.DateTime2, 7).Value = DateTime.Now;
                    cmd.Parameters.Add("@id", SqlDbType.Int).Value = id;
                    int orderId = (Int32)cmd.ExecuteScalar();
                    {
                        serviceStore.ChangeOrderState(orderId, 'S', DateTime.Now.AddDays(2).ToString("yyyy-MM-dd"));
                    }
                }
                catch (SqlException e)
                {
                    Console.WriteLine(e);
                    return -1;
                }
                finally
                {
                    c.Close();
                }
            }
            
            return 0;
             
        }

        public void Subscribe()
        {
            try
            {
                var callback = OperationContext.Current.GetCallbackChannel<IMyServiceCallback>();
                lock (_locker)
                {
                    if (!_subscribers.Contains(callback))
                    {
                        _subscribers.Add(callback);
                    }
                }
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public void Unsubscribe()
        {
            try
            {
                var callback = OperationContext.Current.GetCallbackChannel<IMyServiceCallback>();
                lock (_locker)
                {
                    _subscribers.Remove(callback);
                }

            }
            catch (Exception e)
            {
            }
        }

        public IAsyncResult BeginAsyncTest(string msg, AsyncCallback callback, object asyncState)
        {
            Console.WriteLine("BeginServiceAsyncMethod called with: \"{0}\"", msg);
            Thread.Sleep(5000);
            return new CompletedAsyncResult<string>(msg);
        }

        public string EndAsyncTest(IAsyncResult r)
        {
            CompletedAsyncResult<string> result = r as CompletedAsyncResult<string>;
            Console.WriteLine("EndServiceAsyncMethod called with: \"{0}\"", result.Data);
            return result.Data;
        }
    }

    // Simple async result implementation.
    class CompletedAsyncResult<T> : IAsyncResult
    {
        T data;

        public CompletedAsyncResult(T data)
        { this.data = data; }

        public T Data
        { get { return data; } }

        #region IAsyncResult Members
        public object AsyncState
        { get { return (object)data; } }

        public WaitHandle AsyncWaitHandle
        { get { throw new Exception("The method or operation is not implemented."); } }

        public bool CompletedSynchronously
        { get { return true; } }

        public bool IsCompleted
        { get { return true; } }
        #endregion
    }
}
