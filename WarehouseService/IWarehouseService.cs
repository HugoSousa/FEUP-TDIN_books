using System;
using System.Data;
using System.ServiceModel;

namespace WarehouseService
{
    [ServiceContract]
    public interface IWarehouseService
    {

        [OperationContract]
        DataTable GetOpenRequests();

        [OperationContract]
        int AddRequest(string title, int quantity, DateTime sent, DateTime received, int orderId);

        [OperationContract]
        int ShipRequest(int id);
    }
}
