using System.ServiceModel;

namespace StoreService
{
    [ServiceContract]
    public interface IOrderService
    {
        /*
      [OperationContract]
      int AddTicket(int author, string Problem);
        */

        [OperationContract]
        void CreateOrder(string title, string client, string email, string address, int quantity);

        /*
      [OperationContract]
      DataTable GetTickets(int author);
        */
    }
}