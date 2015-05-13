using System;
using System.Web.UI;
using StoreService;

public partial class _Default : Page {
    OrderServiceClient _proxy;

  protected void Page_Load(object sender, EventArgs e) {
      _proxy = new OrderServiceClient();
  }

    protected void CreateOrderButton_OnClick_(object sender, EventArgs e)
    {
        _proxy.CreateOrder("Titulo1", "Cliente 1", "email 1", "address teste", 2);
    }

    protected void Button1_OnClick_(object sender, EventArgs e)
    {
        _proxy.GetStock("Titulo1");
    }

    protected void Button2_OnClick_(object sender, EventArgs e)
    {
        _proxy.StoreSell("Titulo1", "cliente", 2);
    }

    protected void Button3_OnClick_(object sender, EventArgs e)
    {
        _proxy.UpdateStock("Titulo1", 2);
        //_proxy.ChangeOrderState(2, 'D', DateTime.Now.ToString("yyyy-MM-dd"));
    }
}