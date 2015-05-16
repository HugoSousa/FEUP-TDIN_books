using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Net.Mail;

namespace OrderStore
{
    public class OrderService : IOrderService
    {
        Warehouse proxy_wh;
        public OrderService()
        {
            proxy_wh = new Warehouse();

        }
        //return -1 if the book title doesn't exist
        //return -2 if other sql error
        //Order from the Web! 
        public int CreateOrder(string title, string client, string email, string address, int quantity)
        {
            int stock;
            double unitPrice;

            using (SqlConnection c = new SqlConnection(ConfigurationManager.ConnectionStrings["store_db"].ConnectionString))
            {
                try
                {
                    c.Open();
                    string sql = "select stock, price from [book] where title = @title";
                    SqlCommand cmd = new SqlCommand(sql, c);
                    cmd.Parameters.Add("@title", SqlDbType.NVarChar, 50).Value = title;
                    SqlDataReader reader = cmd.ExecuteReader();
                    if (!reader.HasRows)
                        return -1;
                    else
                    {
                        reader.Read();
                        stock = reader.GetInt32(0);
                        unitPrice = reader.GetFloat(1);
                    }
                }
                catch (SqlException e)
                {
                    Console.WriteLine(e);
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
                        "output inserted.id " +
                        "values(@quantity, @client_name, @address, @email, @state, @state_date, @book, @total_price)";
                    SqlCommand cmd = new SqlCommand(sql, c);
                    cmd.Parameters.Add("@quantity", SqlDbType.Int).Value = quantity;
                    cmd.Parameters.Add("@client_name", SqlDbType.NVarChar, 80).Value = client;
                    cmd.Parameters.Add("@address", SqlDbType.NVarChar, 80).Value = address;
                    cmd.Parameters.Add("@email", SqlDbType.NVarChar, 80).Value = email;
                    if (stock >= quantity)
                    {
                        cmd.Parameters.Add("@state", SqlDbType.Char, 1).Value = 'D'; //dispatched in next day
                        DateTime nextDay = DateTime.Now.AddDays(1);
                        cmd.Parameters.Add("@state_date", SqlDbType.DateTime2).Value = nextDay.ToString("yyyy-MM-dd");
                        UpdateStock(title, 0-quantity);
                        // TODO: send email to the client with the info
                    }
                    else
                    {
                        cmd.Parameters.Add("@state", SqlDbType.Char, 1).Value = 'W'; //waiting expedition
                        cmd.Parameters.Add("@state_date", SqlDbType.DateTime2).Value = DBNull.Value;

                        // TODO: send message to the warehouse (10 * quantity)
                    }

                    cmd.Parameters.Add("@book", SqlDbType.NVarChar, 50).Value = title;
                    cmd.Parameters.Add("@total_price", SqlDbType.Real).Value = unitPrice * quantity;

                    Int32 inserted = (Int32)cmd.ExecuteScalar();
                    return inserted;
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
            return 0;
        }

        public int GetStock(string title)
        {
            int stock;

            using (SqlConnection c = new SqlConnection(ConfigurationManager.ConnectionStrings["store_db"].ConnectionString))
            {
                try
                {
                    c.Open();
                    string sql = "select stock from [book] where title = @title";
                    SqlCommand cmd = new SqlCommand(sql, c);
                    cmd.Parameters.Add("@title", SqlDbType.NVarChar, 50).Value = title;
                    SqlDataReader reader = cmd.ExecuteReader();
                    if (!reader.HasRows)
                        return -1;
                    else
                    {
                        reader.Read();
                        stock = reader.GetInt32(0);
                    }
                }
                catch (SqlException e)
                {
                    Console.WriteLine(e);
                    return -2;
                }
                finally
                {
                    c.Close();
                }
            }

            return stock;
        }

        //return -1 if stock isn't enough (needs to create an order)
        //return -2 if other sql error
        public int StoreSell(string title, string client, int quantity)
        {
            double unitPrice;

            if (GetStock(title) < quantity)
            {
                return -1;
            }

            using (SqlConnection c = new SqlConnection(ConfigurationManager.ConnectionStrings["store_db"].ConnectionString))
            {
                try
                {
                    c.Open();
                    string sql = "select price from [Book] where title = @book";
                    SqlCommand cmd = new SqlCommand(sql, c);
                    cmd.Parameters.Add("@book", SqlDbType.NVarChar, 50).Value = title;
                    SqlDataReader reader = cmd.ExecuteReader();
                    if (!reader.HasRows)
                    {
                        return -3;
                    }
                    else
                    {
                        reader.Read();
                        unitPrice = reader.GetFloat(0);
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
                    string sql = "insert into [Sell](client, book, quantity, total_price) " +
                                 "values(@client, @book, @quantity, @total_price)";
                    SqlCommand cmd = new SqlCommand(sql, c);
                    cmd.Parameters.Add("@client", SqlDbType.NVarChar, 80).Value = client;
                    cmd.Parameters.Add("@book", SqlDbType.NVarChar, 50).Value = title;
                    cmd.Parameters.Add("@quantity", SqlDbType.Int).Value = quantity;
                    cmd.Parameters.Add("@total_price", SqlDbType.NVarChar, 50).Value = quantity * unitPrice;
                    cmd.ExecuteNonQuery();

                    UpdateStock(title, 0 - quantity);
                }
                catch (SqlException e)
                {
                    return -2;
                }
                finally
                {
                    c.Close();
                }
            }

            //TODO: print a receipt (separate application?)

            return 0;
        }

        
        public int UpdateStock(string title, int quantity)
        {
            int newStock = GetStock(title) + quantity;

            using (SqlConnection c = new SqlConnection(ConfigurationManager.ConnectionStrings["store_db"].ConnectionString))
            {
                try
                {
                    c.Open();
                    string sql = "update [book] set stock = stock + @quantity where title = @title";
                    SqlCommand cmd = new SqlCommand(sql, c);
                    cmd.Parameters.Add("@quantity", SqlDbType.Int).Value = quantity;
                    cmd.Parameters.Add("@title", SqlDbType.NVarChar, 50).Value = title;
                    cmd.ExecuteNonQuery();
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

            if (quantity > 0) //if increment of stock
            {
                // cover pending orders with the new stock (Update state to 'D' at actual date)
                // send email
                using (
                    SqlConnection c =
                        new SqlConnection(ConfigurationManager.ConnectionStrings["store_db"].ConnectionString))
                {
                    try
                    {
                        c.Open();
                        string sql =
                            "select id, quantity, book, address, email, state, total_price from [Order] where book = @book and state = 'W' order by order_date;";
                        SqlCommand cmd = new SqlCommand(sql, c);
                        cmd.Parameters.Add("@book", SqlDbType.NVarChar, 50).Value = title;
                        SqlDataReader reader = cmd.ExecuteReader();

                        if (newStock > 0)
                        {
                            while (reader.Read())
                            {
                                if (newStock == 0)
                                    break;

                                int orderQuantity = reader.GetInt32(1);

                                if (orderQuantity <= newStock)
                                {
                                    //change order state to D and actual date
                                    //send email to this user
                                    ChangeOrderState(reader.GetInt32(0), 'D', DateTime.Now.ToString("yyyy-MM-dd"));
                                    //SendEmail(reader.GetString(4), "Order Dispatch", "blabla");

                                    //update book stock (recursively)
                                    UpdateStock(title, 0 - orderQuantity);

                                    newStock -= orderQuantity;
                                }
                            }
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
            }

            return 0;
        }

        public int ChangeOrderState(int id, char state, string stateDate)
        {
            using (SqlConnection c = new SqlConnection(ConfigurationManager.ConnectionStrings["store_db"].ConnectionString))
            {
                try
                {
                    c.Open();
                    string sql = "update [Order] set state = @state, state_date = @state_date where id = @id";
                    SqlCommand cmd = new SqlCommand(sql, c);
                    cmd.Parameters.Add("@state", SqlDbType.Char, 1).Value = @state;
                    cmd.Parameters.Add("@state_date", SqlDbType.DateTime2).Value = stateDate;
                    cmd.Parameters.Add("@id", SqlDbType.Int).Value = id;
                    cmd.ExecuteNonQuery();
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

            return 0;
        }

        private int SendEmail(string to, string subject, string body)
        {
            MailMessage mail = new MailMessage();
            SmtpClient client = new SmtpClient
            {
                Port = 25,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Host = "smtp.gmail.com" //TODO change
            };
            mail.To.Add(new MailAddress(to));
            mail.From = new MailAddress("you@yourcompany.com");
            mail.Subject = subject;
            mail.Body = body;
            client.Send(mail);

            return 0;
        }

        public DataTable GetOrder(int id)
        {
            DataTable result = new DataTable("Order");

            using (SqlConnection c = new SqlConnection(ConfigurationManager.ConnectionStrings["store_db"].ConnectionString))
            {
                try
                {
                    c.Open();
                    string sql = "select * from [Order] where id = @id";
                    SqlCommand cmd = new SqlCommand(sql, c);
                    cmd.Parameters.Add("@id", SqlDbType.Int).Value = id;
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(result);
                }
                catch (SqlException)
                {
                }
                finally
                {
                    c.Close();
                }
            }

            return result;
        }

        private void checkMessageQueue()
        {


   	 }


        public DataTable GetBooks()
        {
            DataTable result = new DataTable("Book");

            using (SqlConnection c = new SqlConnection(ConfigurationManager.ConnectionStrings["store_db"].ConnectionString))
            {
                try
                {
                    c.Open();
                    string sql = "select * from [Book]";
                    SqlCommand cmd = new SqlCommand(sql, c);
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(result);
                }
                catch (SqlException)
                {
                }
                finally
                {
                    c.Close();
                }
            }

            return result;
        }

}
