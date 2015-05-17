using System;

namespace OrderStore
{
    [Serializable]
    public class BookOrder
    {
        public string Title { get; set; }
        public int Quantity{ get; set; }

        public BookOrder(string title, int quantity)
        {
            Title = title;
            Quantity = quantity;
        }

        public BookOrder(){}
    }
}
