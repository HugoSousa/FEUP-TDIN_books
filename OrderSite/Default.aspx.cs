using System;
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
                r.Attributes["onmouseover"] = "this.style.cursor='pointer';this.style.textDecoration='underline';";
                r.Attributes["onmouseout"] = "this.style.textDecoration='none';";
                r.ToolTip = "Click to select row";
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
}