﻿using System.Collections.Generic;
using System.Data;
using System.ServiceModel;

namespace OrderStore
{
    [ServiceContract(CallbackContract=typeof(IStoreCallback))]
    public interface IOrderService
    {
        [OperationContract]
        int CreateOrder(string title, string client, string email, string address, int quantity);

        [OperationContract]
        int GetStock(string title);

        [OperationContract]
        int StoreSell(string title, string client, int quantity);

        [OperationContract]
        int UpdateStock(string title, int quantity);

        [OperationContract]
        int ChangeOrderState(int id, char state, string stateDate);

        [OperationContract]
        DataTable GetOrder(int id);

        [OperationContract]
        DataTable GetBooks();

        [OperationContract]
        void TestMSMQ(string body);

        [OperationContract]
        void Subscribe(string printer);

        [OperationContract]
        void Unsubscribe();

        [OperationContract]
        void PrintReceipt(string printer, Receipt receipt);

        [OperationContract]
        List<string> GetAvailablePrinters();
    }
}