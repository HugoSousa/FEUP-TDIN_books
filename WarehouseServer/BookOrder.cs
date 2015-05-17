using System;

namespace OrderStore
{
    [Serializable]
    public class BookOrder
    {
        public string Title { get; set; }
        public int Quantity{ get; set; }

        public int OrderId { get; set; }

        public BookOrder(string title, int quantity, int orderId)
        {
            Title = title;
            Quantity = quantity;
            OrderId = orderId;
        }

        public BookOrder(){}
    }
}
