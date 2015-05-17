using System;
using System.Data;
using System.ServiceModel;

namespace WarehouseService
{
    [ServiceContract]
    public interface IWarehouseService
    {

        [OperationContract]
        DataTable GetOpenRequests(int id);

        [OperationContract]
        int AddRequest(string title, int quantity, DateTime sent, DateTime received);

        [OperationContract]
        int ShipRequest(int id);
    }
}
