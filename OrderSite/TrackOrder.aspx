<%@ Page Language="C#" AutoEventWireup="true" CodeFile="TrackOrder.aspx.cs" Inherits="TrackOrder" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Track Order</title>
    <link rel="stylesheet" href="css/bootstrap.min.css" />
</head>
<body>
    <div class="container-fluid">
        <form id="form1" runat="server">

            <div class="jumbotron text-center">
                <h1>Track Order</h1>
            </div>

            <div class="row">
                <div class="col-md-4 col-md-offset-4">
                    <asp:TextBox runat="server" class="form-control text-center" placeholder="Enter Order Code" ID="OrderCode" autocomplete="off"></asp:TextBox>
                </div>
            </div>

            <br />

            <div class="row">
                <div class="col-md-4 col-md-offset-4">
                    <asp:Button runat="server" class="btn btn-primary btn-lg btn-block" Text="Track Order" OnClick="GetOrder_Click" />
                </div>
            </div>

            <div style="margin: 0px auto 0px auto; text-align: center;">
            </div>

        </form>

        <br />
        <br />

        <div class="row">
            <div class="col-md-6 col-md-offset-3">
                <div class="panel panel-default">
                    <div class="panel-heading">
                        Order Number :
                        <asp:Label ID="OrderId" runat="server" />
                    </div>
                    <div class="panel-body">
                        <div class="page-header" style="margin:0">
                            <h3><strong>Book</strong></h3>
                        </div>
                        <table class="table table-striped">
                            <thead>
                                <tr>
                                    <th>Name</th>
                                    <th>Quantity</th>
                                    <th>Price</th>
                                </tr>
                            </thead>
                            <tbody>
                                <tr>
                                    <td>
                                        <asp:Label ID="OrderBook" runat="server" /></td>
                                    <td>
                                        <asp:Label ID="OrderQuantity" runat="server" /></td>
                                    <td>
                                        <asp:Label ID="OrderPrice" runat="server" /></td>
                                </tr>
                            </tbody>
                        </table>

                        <div class="page-header" style="margin:0;margin-top:30px">
                            <h3><strong>Client</strong></h3>
                        </div>

                        <table class="table table-striped">
                            <thead>
                                <tr>
                                    <th>Name</th>
                                    <th>Address</th>
                                    <th>Email</th>
                                </tr>
                            </thead>
                            <tbody>
                                <tr>
                                    <td>
                                        <asp:Label ID="OrderClient" runat="server" /></td>
                                    <td>
                                        <asp:Label ID="OrderAddress" runat="server" /></td>
                                    <td>
                                        <asp:Label ID="OrderEmail" runat="server" /></td>
                                </tr>
                            </tbody>
                        </table>

                        <br />

                        <div class="well well-sm text-center"><strong><asp:Label ID="OrderState" runat="server" /></strong></div>

                    </div>
                </div>
            </div>
        </div>

        <div style="margin: 0 auto; text-align: center">
            <br />
            <asp:Label ID="ErrorInfo" runat="server" /><br />
            <br />

            <br />

        </div>
    </div>

</body>
</html>
