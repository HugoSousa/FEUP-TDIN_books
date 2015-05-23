using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace OrderStore
{
    [Serializable]
    [DataContract]
    public class Receipt
    {
        [DataMember]
        public string Client { get; set; }
        [DataMember]
        private string Title { get; set; }
        [DataMember]
        private int Quantity { get; set; }
        [DataMember]
        private double TotalPrice { get; set; }

        public Receipt(string client, string title, int quantity, double price)
        {
            Client = client;
            Title = title;
            Quantity = quantity;
            TotalPrice = price;
        }

        public Receipt(){}
    }
}
