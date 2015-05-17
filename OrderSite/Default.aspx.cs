using System;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Web.UI;
using System.Web.UI.WebControls;
using StoreService;

public partial class _Default : Page
{
    private OrderServiceClient _proxy;

    protected void Page_Load(object sender, EventArgs e)
    {
        _proxy = new OrderServiceClient();
    }

    protected override void Render(HtmlTextWriter writer)
    {
        foreach (GridViewRow r in GridView1.Rows)
        {
            if (r.RowType == DataControlRowType.DataRow)
            {
                r.Attributes["onmouseover"] = "this.style.cursor='pointer';this.style.color='#808080'";
                r.Attributes["onmouseout"] = "this.style.textDecoration='none';this.style.color='black'";
                r.ToolTip = "Click to select this book";
                r.Attributes["onclick"] = this.Page.ClientScript.GetPostBackClientHyperlink(this.GridView1, "Select$" + r.RowIndex);

            }
        }

        base.Render(writer);
    }
    protected void GridView1_SelectedItemChanged(object sender, EventArgs e)
    {
        foreach (GridViewRow row in GridView1.Rows)
        {
            if (row.RowIndex == GridView1.SelectedIndex)
            {
                row.BackColor = ColorTranslator.FromHtml("#A1DCF2");
            }
            else
            {
                row.BackColor = ColorTranslator.FromHtml("#FFFFFF");
            }
        }
    }

    protected void CheckSelectedIndex_OnClick_(object sender, EventArgs e)
    {
        int a = GridView1.SelectedIndex;
        string c = GridView1.Rows[GridView1.SelectedIndex].Cells[1].Text; //titulo

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

    protected void OrderItem_OnClick_(object sender, EventArgs e)
    {
        ErrorLabel.Text = "";

        if (GridView1.SelectedIndex == -1)
        {
            ErrorLabel.Text = "You didn't select a book.";
            return;
        }
        else
        {
            try
            {
                string title = GridView1.Rows[GridView1.SelectedIndex].Cells[1].Text;
                int quantity;
                if (!Int32.TryParse(quantityInput.Text, out quantity))
                {
                    ErrorLabel.Text = "Invalid quantity";
                    return;
                }
                string client = clientInput.Text;
                string email = emailInput.Text;
                if (!new EmailAddressAttribute().IsValid(email))
                {
                    ErrorLabel.Text = "Invalid email.";
                    return;
                }
                string address = addressInput.Text;

                if (client == null || email == null || address == null)
                {
                    ErrorLabel.Text = "You need to fill all the fields.";
                    return;
                }
                else
                {
                    int id = _proxy.CreateOrder(title, client, email, address, quantity);
                    if (id > 0)
                    {
                        ErrorLabel.Text = "Order successfully created. The id of your order is " + id + ".\n You can check your order state at http://localhost:8994/TrackOrder.aspx";
                    }
                }
            }
            catch (Exception)
            {
                ErrorLabel.Text = "Error processing the input.";
                return;
            }

            
        }

        
        //_proxy.ChangeOrderState(2, 'D', DateTime.Now.ToString("yyyy-MM-dd"));
    }

    public void Test(object sender, EventArgs e)
    {
        _proxy.TestMSMQ("teste");
    }
    
}