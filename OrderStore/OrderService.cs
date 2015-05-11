using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Remoting.Messaging;

namespace StoreService
{
    public class OrderService : IOrderService
    {
        /*
      public int AddTicket(int author, string problem) {
        int id = 0;

        using (SqlConnection c = new SqlConnection(ConfigurationManager.ConnectionStrings["TTs"].ConnectionString)) {
          try {
            c.Open();
            string sql = "insert into TTickets(Author, Problem, Answer, Status) values (" + author + ", '" + problem + "', '', 1)";
            SqlCommand cmd = new SqlCommand(sql, c);
            cmd.ExecuteNonQuery();
            cmd.CommandText = "select max(Id) from TTickets";
            id = (int) cmd.ExecuteScalar();
          }
          catch (SqlException) {
          }
          finally {
            c.Close();
          }
        }
        return id;
      }

      public DataTable GetTickets(int author) {
        DataTable result = new DataTable("TTickets");
      
        using (SqlConnection c = new SqlConnection(ConfigurationManager.ConnectionStrings["TTs"].ConnectionString)) {
          try {
            c.Open();
            string sql = "select Id, Problem, Status, Answer from TTickets where Author=" + author;
            SqlCommand cmd = new SqlCommand(sql, c);
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            adapter.Fill(result);
          }
          catch (SqlException) {
          }
          finally {
            c.Close();
          }
        }
        return result;
      }
      */

        public int CreateOrder(string title, string client, string email, string address, int quantity)
        {
            int stock;

            using (SqlConnection c = new SqlConnection(ConfigurationManager.ConnectionStrings["store_db"].ConnectionString))
            {
                try
                {
                    c.Open();
                    string sql = "select stock from [book] where title = @title";
                    SqlCommand cmd = new SqlCommand(sql, c);
                    cmd.Parameters.Add("@title", SqlDbType.Int).Value = title;
                    SqlDataReader reader = cmd.ExecuteReader();
                    if (!reader.HasRows)
                        return -1;
                    else
                    {
                        stock = reader.GetInt32(0);
                    }
                }
                catch (SqlException)
                {
                    return -2;
                }
                finally
                {
                    c.Close();
                }
            }

            using (SqlConnection c = new SqlConnection(ConfigurationManager.ConnectionStrings["store_db"].ConnectionString))
            {
                try
                {
                    c.Open();
                    string sql =
                        "insert into [order](quantity, client_name, address, email, state, state_date, book, total_price) " +
                        "values(@quantity, @client_name, @address, @email, @state, @state_date, @book, @total_price)";
                    SqlCommand cmd = new SqlCommand(sql, c);
                    cmd.Parameters.Add("@quantity", SqlDbType.Int).Value = quantity;
                    cmd.Parameters.Add("@client_name", SqlDbType.NVarChar, 80).Value = client;
                    cmd.Parameters.Add("@address", SqlDbType.NVarChar, 80).Value = address;
                    cmd.Parameters.Add("@email", SqlDbType.NVarChar, 80).Value = email;
                    if (stock >= quantity)
                    {
                        cmd.Parameters.Add("@state", SqlDbType.Char, 1).Value = '';
                    }
                    else
                        cmd.Parameters.Add("@state", SqlDbType.Char, 1).Value = 'W';

                    cmd.Parameters.Add("@state_date", SqlDbType.DateTime2).Value = DBNull.Value;
                    cmd.Parameters.Add("@book", SqlDbType.NVarChar, 50).Value = title;
                    cmd.Parameters.Add("@total_price", SqlDbType.Real).Value = 50.2;
                    cmd.ExecuteNonQuery();
                }
                catch (SqlException exception)
                {
                    return -2;
                    Console.WriteLine(exception);
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
