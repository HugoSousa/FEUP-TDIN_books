using System;
using System.Data;
using System.ServiceModel;

namespace TTService {
  [ServiceContract]
  public interface ITTServ {
    [OperationContract]
    int AddTicket(int author, string Problem);

    [OperationContract]
    DataTable GetTickets(int author);
  }
}