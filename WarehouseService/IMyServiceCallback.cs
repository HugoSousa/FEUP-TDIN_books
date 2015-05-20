using System.ServiceModel;

namespace WarehouseService
{
    public interface IMyServiceCallback
    {
        [OperationContract]
        void OnCallback();
    }
}
