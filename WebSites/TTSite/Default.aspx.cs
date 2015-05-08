using System;
using System.Drawing;
using System.Web.UI;
using TTSvc;

public partial class _Default : Page {
  TTServClient proxy;

  protected void Page_Load(object sender, EventArgs e) {
    proxy = new TTServClient();
  }

  protected void Button1_Click(object sender, EventArgs e) {
    int id = 0;

    if (DropDownList1.SelectedIndex > 0) {
      if (TextBox1.Text.Length > 0) {
        id = proxy.AddTicket(Convert.ToInt32(DropDownList1.SelectedValue), TextBox1.Text);
        Label1.ForeColor = Color.DarkBlue;
        Label1.Text = "Result: Inserted with Id = " + id;
      }
      else {
        Label1.ForeColor = Color.Red;
        Label1.Text = "Result: Please describe a problem!";
      }
    }
    else {
      Label1.ForeColor = Color.Red;
      Label1.Text = "Result: Select an Author!";
    }
  }

  protected void Button2_Click(object sender, EventArgs e) {
    if (DropDownList1.SelectedIndex > 0) {
      GridView1.DataSource = proxy.GetTickets(Convert.ToInt32(DropDownList1.SelectedValue));
      GridView1.DataBind();
      GridView1.Visible = true;
      Label2.Text = "";
    }
    else {
      GridView1.Visible = false;
      Label2.Text = "Select an Author!";
    }
  }
}