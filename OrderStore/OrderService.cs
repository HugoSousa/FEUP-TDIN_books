using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Mail;
using System.ServiceModel;

namespace OrderStore
{
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, InstanceContextMode = InstanceContextMode.Single, UseSynchronizationContext = false)]
    public class OrderService : IOrderService
    {
        private List<PrinterCallback> _subscribers = new List<PrinterCallback>();
        private readonly object _locker = new object();

        //returns the id of inserted order
        //return -1 if the book title doesn't exist
        //return -2 if other sql error
        public int CreateOrder(string title, string client, string email, string address, int quantity)
        {
            int stock;
            double unitPrice;
            bool requestWarehouse = false;
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
                        UpdateStock(title, 0 - quantity);
                        // TODO: send email to the client with the info
                    }
                    else
                    {
                        cmd.Parameters.Add("@state", SqlDbType.Char, 1).Value = 'W'; //waiting expedition
                        cmd.Parameters.Add("@state_date", SqlDbType.DateTime2).Value = DBNull.Value;

                        
                        requestWarehouse = true;
                        
                    }

                    cmd.Parameters.Add("@book", SqlDbType.NVarChar, 50).Value = title;
                    cmd.Parameters.Add("@total_price", SqlDbType.Real).Value = unitPrice*quantity;

                    Int32 inserted = (Int32) cmd.ExecuteScalar();

                    if (requestWarehouse)
                    {
                        Warehouse.SendMessage(new BookOrder(title, 10*quantity, inserted));
                    }
                    else
                    {
                        string body = "Mr./Ms. " + client + ",<br>" +
                                      "Your order was succesfully processed and will be dispatched tomorrow.<br><br><br>" +
                                      "<u>Details</u><br>" +
                                      "<b>Book Title:</b> " + title + "<br>" +
                                      "<b>Quantity:</b> " + quantity + "<br>" +
                                      "<b>Total Price:</b> " + Math.Round(unitPrice*quantity, 2) + "<br>" +
                                      "<b>State:</b> Dispatched at " + DateTime.Now.AddDays(1).ToString("yyyy-MM-dd") +
                                      "<br>" +
                                      "<b>Address:</b> " + address +
                                      "<br><br><br>" +
                                      "Thanks for choosing our store!";
                        try
                        {
                            SendEmail(email, "[TDIN_bookstore] Order " + inserted + " dispatching", body);
                        }
                        catch (Exception)
                        {

                        }
                    }

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
        }

        public int GetStock(string title)
        {
            int stock;

            using (
                SqlConnection c = new SqlConnection(ConfigurationManager.ConnectionStrings["store_db"].ConnectionString)
                )
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

            using (
                SqlConnection c = new SqlConnection(ConfigurationManager.ConnectionStrings["store_db"].ConnectionString)
                )
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
                        return -2;
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

            using (
                SqlConnection c = new SqlConnection(ConfigurationManager.ConnectionStrings["store_db"].ConnectionString)
                )
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
                    cmd.Parameters.Add("@total_price", SqlDbType.Real).Value = Math.Round(quantity*unitPrice, 2);
                    cmd.ExecuteNonQuery();

                    UpdateStock(title, 0 - quantity);
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

            //TODO: print a receipt (separate application?)
            foreach (var subscriber in _subscribers)
            {
                var callbackComm = (ICommunicationObject)subscriber.Callback;
                if (callbackComm.State == CommunicationState.Opened)
                {
                    if (subscriber.Printer == null)
                    {
                        subscriber.Callback.OnSuccessfullSell();
                    }
                }
            }

            return 0;
        }

        public void PrintReceipt(string printer, Receipt receipt)
        {
            foreach (var subscriber in _subscribers)
            {
                var callbackComm = (ICommunicationObject)subscriber.Callback;
                if (callbackComm.State == CommunicationState.Opened && subscriber.Printer != null && subscriber.Printer.Equals(printer))
                {
                    subscriber.Callback.OnPrint(receipt);
                }
            }
        }

        public int UpdateStock(string title, int quantity)
        {
            int newStock = GetStock(title) + quantity;

            using (
                SqlConnection c = new SqlConnection(ConfigurationManager.ConnectionStrings["store_db"].ConnectionString)
                )
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
                //only notify if stock was added. If was reduced, means it was a sell and GUI was already updated on sell callback
                foreach (var subscriber in _subscribers)
                {
                    var callbackComm = (ICommunicationObject)subscriber.Callback;
                    if (callbackComm.State == CommunicationState.Opened)
                        subscriber.Callback.OnSucessfullStockUpdate();
                }

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
                            "select id, quantity, book, address, email, state, total_price, client_name from [Order] where book = @book and state = 'S' order by order_date;";
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
                                string client = reader.GetString(7);
                                float totalPrice = reader.GetFloat(6);
                                string address = reader.GetString(3);
                                string email = reader.GetString(4);
                                int orderId = reader.GetInt32(0);

                                if (orderQuantity <= newStock)
                                {
                                    //change order state to D and actual date
                                    //send email to this user
                                    ChangeOrderState(reader.GetInt32(0), 'D', DateTime.Now.ToString("yyyy-MM-dd"));

                                    string body = "Mr./Ms. " + client + ",<br>" +
                                                  "Our stock has been updated and your order has been succesfully processed and will be dispatched today.<br><br><br>" +
                                                  "<u>Details</u><br>" +
                                                  "<b>Book Title:</b> " + title + "<br>" +
                                                  "<b>Quantity:</b> " + quantity + "<br>" +
                                                  "<b>Total Price:</b> " + Math.Round(totalPrice, 2) + "<br>" +
                                                  "<b>State:</b> Dispatched at " + DateTime.Now.ToString("yyyy-MM-dd") + "<br>" +
                                                  "<b>Address:</b> " + address +
                                                  "<br><br><br>" +
                                                  "Thanks for choosing our store!";
                                    try
                                    {
                                        SendEmail(email, "[TDIN_bookstore] Order " + orderId + " dispatching", body);
                                    }
                                    catch (Exception)
                                    {
                                        
                                    }


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
            using (
                SqlConnection c = new SqlConnection(ConfigurationManager.ConnectionStrings["store_db"].ConnectionString)
                )
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
            MailMessage mail = new MailMessage {IsBodyHtml = true};
            SmtpClient client = new SmtpClient
            {
                Port = 25,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Host = "smtp.fe.up.pt"
            };
            mail.To.Add(new MailAddress(to));
            mail.From = new MailAddress("orders@tdin_bookstore.com");
            mail.Subject = subject;
            mail.Body = body;
            client.Send(mail);

            return 0;
        }

        public DataTable GetOrder(int id)
        {
            DataTable result = new DataTable("Order");

            using (
                SqlConnection c = new SqlConnection(ConfigurationManager.ConnectionStrings["store_db"].ConnectionString)
                )
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

        public DataTable GetBooks()
        {
            DataTable result = new DataTable("Book");

            using (
                SqlConnection c = new SqlConnection(ConfigurationManager.ConnectionStrings["store_db"].ConnectionString)
                )
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

        public void TestMSMQ(string body)
        {
            Warehouse.SendMessage(new BookOrder("tituloteste", 10, 1));
        }

        public void Subscribe(string printer)
        {
            try
            {
                var callback = OperationContext.Current.GetCallbackChannel<IStoreCallback>();
                lock (_locker)
                {
                    if (! _subscribers.Any(p => p.Callback == callback))
                    {
                        _subscribers.Add(new PrinterCallback()
                        {
                            Callback = callback,
                            Printer = printer
                        });
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
                var callback = OperationContext.Current.GetCallbackChannel<IStoreCallback>();
                lock (_locker)
                {
                    _subscribers.RemoveAll(x => x.Callback == callback);
                }

            }
            catch (Exception e)
            {
            }
        }

        public List<string> GetAvailablePrinters()
        {
            List<string> printers = new List<string>();

            foreach (var subscriber in _subscribers)
            {
                var callbackComm = (ICommunicationObject)subscriber.Callback;
                if (callbackComm.State == CommunicationState.Opened && subscriber.Printer != null)
                {
                    printers.Add(subscriber.Printer);
                }
            }

            return printers;
        } 
    }

    public class PrinterCallback 
    {
        public string Printer { get; set; } //if null, it's not a printer
        public IStoreCallback Callback { get; set; }
    }
}
