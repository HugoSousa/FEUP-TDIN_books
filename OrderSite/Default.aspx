<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="_Default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Order Item</title>
    <style type="text/css">
        .auto-style1 { width: 100%; }

        .auto-style3 {
            width: 65px;
            font-family: Arial;
        }

        .hiddenColumn
        {
            display: none;
        }
    </style>
</head>
<body>
<form id="form1" runat="server">
    <div>
        <h1>Order an Item</h1>
        <asp:GridView ID="GridView1" runat="server" OnSelectedIndexChanged="GridView1_SelectedItemChanged" EnableEventValidation = "false" EnableViewState="true" OnItemCommand="Item_Command" DataKeyField="title" DataSourceID="Books" >
        <Columns>
            <asp:CommandField ButtonType="Button" ShowSelectButton="true" HeaderStyle-CssClass="hiddenColumn"
                                ItemStyle-CssClass="hiddenColumn" FooterStyle-CssClass="hiddenColumn"/>
        </Columns>
        </asp:GridView>
        
        <br/>
        <asp:Label runat="server" Text="Quantity"></asp:Label>
        <asp:TextBox runat="server" type="number" ID="quantityInput"/>
        <br/><br/>
        
        <asp:Label runat="server" Text="Name"></asp:Label>
        <asp:TextBox runat="server" ID="clientInput"/>
        <br/><br/>
        
        <asp:Label runat="server" Text="Email"></asp:Label>
        <asp:TextBox runat="server" ID="emailInput"/>
        <br/><br/>
        
        <asp:Label runat="server" Text="Address"></asp:Label>
        <asp:TextBox runat="server" ID="addressInput"/>
        <br/><br/>
        
        <!--
        <asp:Button runat="server" ID="New" Text="Check Selected Index" OnClick="CheckSelectedIndex_OnClick_"/>
        <asp:Button runat="server" ID="CreateOrderButton" Text="Create an Order" OnClick="CreateOrderButton_OnClick_"/>
        <asp:Button runat="server" ID="Button1" Text="Test1" OnClick="Button1_OnClick_"/>
        <asp:Button runat="server" ID="Button2" Text="Test2" OnClick="Button2_OnClick_"/>
        <asp:Button runat="server" ID="Button3" Text="Test3" OnClick="Button3_OnClick_"/>
        -->
        
        <asp:Label runat="server" ID="ErrorLabel"/>
        <br/>
       
        <asp:Button runat="server" ID="Order" Text="Order Item" OnClick="OrderItem_OnClick_"/>

    </div>
    <asp:SqlDataSource ID="Books" runat="server" ConnectionString="<%$ ConnectionStrings:ConnectionString %>" SelectCommand="SELECT * FROM [Book]"></asp:SqlDataSource>
</form>
</body>
</html>
