<%@ Page Language="C#" AutoEventWireup="true" CodeFile="TrackOrder.aspx.cs" Inherits="TrackOrder" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div style="margin: 0px auto 0px auto; text-align: center;">
            <asp:TextBox runat="server" ID="OrderCode" style="margin-bottom: 2px"></asp:TextBox>
        </div>
        <div style="margin: 0px auto 0px auto; text-align: center;">
            <asp:Button runat="server" Text="Track Order" OnClick="GetOrder_Click"/>
        </div>
    </form>
    <div style="margin: 0 auto; text-align: center">
        <br/>
        <asp:Label ID="ErrorInfo" runat="server"/><br/>
        <asp:Label ID="OrderId" runat="server"/><br/>
        <asp:Label ID="OrderBook" runat="server"/><br/>
        <asp:Label ID="OrderQuantity" runat="server"/><br/>
        <asp:Label ID="OrderPrice" runat="server"/><br/><br/>

        <asp:Label ID="OrderClient" runat="server"/><br/>
        <asp:Label ID="OrderAddress" runat="server"/><br/>
        <asp:Label ID="OrderEmail" runat="server"/><br/><br/>
        
        <asp:Label ID="OrderState" runat="server"/><br/>
        <asp:Label ID="OrderDate" runat="server"/><br/>
        
    </div>
</body>
</html>
