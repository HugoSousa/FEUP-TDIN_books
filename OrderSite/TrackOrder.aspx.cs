using System;
using System.Data;
using StoreService;

public partial class TrackOrder : System.Web.UI.Page
{
    OrderServiceClient _proxy;
    protected void Page_Load(object sender, EventArgs e)
    {
        _proxy = new OrderServiceClient();
    }

    protected void GetOrder_Click(object sender, EventArgs e)
    {
        ClearFields();

        DataTable dt = _proxy.GetOrder(Convert.ToInt32(OrderCode.Text));

        if (dt.Rows.Count > 0)
        {
            OrderId.Text = dt.Rows[0]["id"].ToString();
            OrderBook.Text = dt.Rows[0]["book"].ToString();
            OrderQuantity.Text = dt.Rows[0]["quantity"].ToString();
            OrderPrice.Text = dt.Rows[0]["total_price"].ToString();
            OrderClient.Text = dt.Rows[0]["client_name"].ToString();
            OrderAddress.Text = dt.Rows[0]["address"].ToString();
            OrderEmail.Text = dt.Rows[0]["email"].ToString();

            switch (dt.Rows[0]["state"].ToString())
            {
                case "W":
                    OrderState.Text = "Waiting for expedition.";
                    break;
                case "D":
                    OrderState.Text = "Dispatched at " + dt.Rows[0]["state_date"].ToString().Split(null)[0];
                    break;
                //TODO outro state?
            }
        }
        else
        {
            //order id doesn't exist
            ErrorInfo.Text = "The order id " + Convert.ToInt32(OrderCode.Text) + " doesn't exist.";
        }
    }

    private void ClearFields()
    {
        ErrorInfo.Text = "";
        OrderId.Text = "";
        OrderBook.Text = "";
        OrderQuantity.Text = "";
        OrderPrice.Text = "";
        OrderClient.Text = "";
        OrderAddress.Text = "";
        OrderEmail.Text = "";
        OrderState.Text = "";
        OrderDate.Text = "";
    }

}