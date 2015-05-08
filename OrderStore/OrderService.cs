using System.Configuration;
using System.Data;
using System.Data.SqlClient;

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

        public void CreateOrder(string title, string client, string email, string address, int quantity)
        {
            using (SqlConnection c = new SqlConnection(ConfigurationManager.ConnectionStrings["store_db"].ConnectionString))
            {
                try
                {
                    c.Open();
                    string sql = "insert into order(id, quantity, client_name, address, email, state, state_date, book, total_price) " +
                                 "values(@id, @quantity, @client_name, @address, @email, @state, @state_date, @book, @total_price)";
                    SqlCommand cmd = new SqlCommand(sql, c);
                    //SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    cmd.Parameters.Add("@id", SqlDbType.Int).Value = 1;
                    cmd.Parameters.Add("@quantity", SqlDbType.Int).Value = 2;
                    cmd.Parameters.Add("@client_name", SqlDbType.NVarChar, 80).Value = "Ricardo Silva";
                    cmd.Parameters.Add("@address", SqlDbType.NVarChar, 80).Value = "Rua das Flores, 82";
                    cmd.Parameters.Add("@email", SqlDbType.NVarChar, 80).Value = "ricardani@gmail.com";
                    cmd.Parameters.Add("@state", SqlDbType.Char, 1).Value = "W";
                    cmd.Parameters.Add("@state_date", SqlDbType.DateTime2).Value = "null";
                    cmd.Parameters.Add("@book", SqlDbType.NVarChar, 50).Value = "Titulo1";
                    cmd.Parameters.Add("@total_price", SqlDbType.Real).Value = "50.2";
                }
                catch (SqlException)
                {
                }
                finally
                {
                    c.Close();
                }
            }
        }
    }
}
