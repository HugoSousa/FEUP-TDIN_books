using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace OrderStore
{
    public interface IStoreCallback
    {
        [OperationContract(IsOneWay = true)]
        void OnSuccessfullSell();

        [OperationContract(IsOneWay = true)]
        void OnSucessfullStockUpdate();
    }
}
