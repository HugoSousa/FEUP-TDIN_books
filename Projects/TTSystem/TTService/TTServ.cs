using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace TTService {
  public class TTServ : ITTServ {
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
  }
}
