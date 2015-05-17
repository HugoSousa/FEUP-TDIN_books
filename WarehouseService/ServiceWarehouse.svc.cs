using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using WarehouseService.StoreService;

namespace WarehouseService
{
    // NOTE: In order to launch WCF Test Client for testing this service, please select WarehouseServ.svc or WarehouseServ.svc.cs at the Solution Explorer and start debugging.
    public class WarehouseServ : IWarehouseService
    {
        public DataTable GetOpenRequests(int id)
        {
            DataTable result = new DataTable("Request");

            using (SqlConnection c = new SqlConnection(ConfigurationManager.ConnectionStrings["warehouse_db"].ConnectionString))
            {
                try
                {
                    c.Open();
                    string sql = "select * from [Request] where ship_date = null";
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

        public int AddRequest(string title, int quantity, DateTime sent, DateTime received)
        {
            using (SqlConnection c = new SqlConnection(ConfigurationManager.ConnectionStrings["warehouse_db"].ConnectionString))
            {
                try
                {
                    c.Open();
                    string sql = "insert into [Request] (title, quantity, request_sent_date, request_received_date) " +
                                 "values (@title, @quantity, @sent, @received)";
                    SqlCommand cmd = new SqlCommand(sql, c);
                    cmd.Parameters.Add("@title", SqlDbType.NVarChar, 50).Value = title;
                    cmd.Parameters.Add("@quantity", SqlDbType.Int).Value = quantity;
                    cmd.Parameters.Add("@sent", SqlDbType.DateTime2, 7).Value = sent;
                    cmd.Parameters.Add("@received", SqlDbType.DateTime2, 7).Value = received;
                    cmd.ExecuteNonQuery();
                }
                catch (SqlException e)
                {
                    return -1;
                }
                finally
                {
                    c.Close();
                }
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
                    string sql = "update [Request] set ship_date = @now output order_id where id = @id";
                    SqlCommand cmd = new SqlCommand(sql, c);
                    cmd.Parameters.Add("@now", SqlDbType.DateTime2, 7).Value = DateTime.Now;
                    cmd.Parameters.Add("@id", SqlDbType.Int).Value = id;
                    int orderId = (int)cmd.ExecuteScalar();
                    {
                        serviceStore.ChangeOrderState(orderId, 'W', DateTime.Now.AddDays(2).ToString("yyyy-MM-dd"));
                    }
                }
                catch (SqlException e)
                {
                    return -1;
                }
                finally
                {
                    c.Close();
                }
            }
            
            return 0;
             
        }
    }
}
