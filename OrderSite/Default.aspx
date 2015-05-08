<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="_Default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>TTSystem</title>
    <style type="text/css">
        .auto-style1 { width: 100%; }

        .auto-style3 {
            width: 65px;
            font-family: Arial;
        }
    </style>
</head>
<body style="background-color: #CCFFCC">
<form id="form1" runat="server">
    <div>
        <h1>Acme Corporation TTSystem</h1>
        <table class="auto-style1">
            <tr>
                <td class="auto-style3">Author:</td>
                <td>
                    <asp:DropDownList ID="DropDownList1" runat="server" DataSourceID="Books" DataTextField="title" DataValueField="title">
                    </asp:DropDownList>
                </td>
            </tr>
        </table>
    </div>
    <asp:SqlDataSource ID="Books" runat="server" ConnectionString="Data Source=(LocalDB)\v11.0;AttachDbFilename=|DataDirectory|\store_db.mdf;Integrated Security=True" ProviderName="System.Data.SqlClient" SelectCommand="SELECT * FROM [Book]"></asp:SqlDataSource>
</form>
</body>
</html>
