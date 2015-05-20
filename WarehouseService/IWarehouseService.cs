using System;
using System.Collections.Generic;
using System.Data;
using System.ServiceModel;

namespace WarehouseService
{
    [ServiceContract(CallbackContract=typeof(IMyServiceCallback))]
    public interface IWarehouseService
    {
        [OperationContract]
        DataTable GetOpenRequests();

        [OperationContract]
        int AddRequest(string title, int quantity, DateTime sent, DateTime received, int orderId);

        [OperationContract]
        int ShipRequest(int id);

        [OperationContract]
        void Subscribe();

        [OperationContract]
        void Unsubscribe();
    }
}
