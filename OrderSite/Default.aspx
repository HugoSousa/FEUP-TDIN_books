<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="_Default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Order Item</title>
    <style type="text/css">
        .auto-style1 {
            width: 100%;
        }

        .auto-style3 {
            width: 65px;
            font-family: Arial;
        }

        .hiddenColumn {
            display: none;
        }
    </style>
    <link rel="stylesheet" href="css/bootstrap.min.css" />
</head>
<body>
    <form id="form1" runat="server">
        <div class="container-fluid">
            <div class="jumbotron text-center">
                <h1>Order an Item</h1>
            </div>

            <asp:GridView class="table table-hover table-bordered text-capitalize" ID="GridView1" runat="server" OnSelectedIndexChanged="GridView1_SelectedItemChanged" EnableEventValidation="false" EnableViewState="true" OnItemCommand="Item_Command" DataKeyField="title" DataSourceID="Books">
                <Columns>
                    <asp:CommandField ButtonType="Button" ShowSelectButton="true" HeaderStyle-CssClass="hiddenColumn"
                        ItemStyle-CssClass="hiddenColumn" FooterStyle-CssClass="hiddenColumn" />
                </Columns>
            </asp:GridView>

            <br />
            <div class="row">
                <div class="col-md-4 col-md-offset-4">
                    <div class="input-group">
                        <span class="input-group-addon"><strong>Quantity</strong></span>
                        <asp:TextBox runat="server" type="number" class="form-control" ID="quantityInput" placeholder="Enter Quantity" min="0" autocomplete="off" />
                    </div>
                </div>
            </div>

            <br />

            <div class="row">
                <div class="col-md-4">
                    <div class="input-group">
                        <span class="input-group-addon"><strong>&nbsp;&nbsp;Name&nbsp;&nbsp;&nbsp;</strong></span>
                        <asp:TextBox runat="server" type="text" class="form-control" placeholder="Enter Name" ID="clientInput" autocomplete="off" />
                    </div>
                </div>
                <div class="col-md-4">
                    <div class="input-group">
                        <span class="input-group-addon"><strong>&nbsp;&nbsp;Email&nbsp;&nbsp;&nbsp;</strong></span>
                        <asp:TextBox runat="server" type="text" class="form-control" placeholder="Enter Email" ID="emailInput" autocomplete="off" />
                    </div>
                </div>

                <div class="col-md-4">
                    <div class="input-group ">
                        <span class="input-group-addon"><strong>Address</strong></span>
                        <asp:TextBox runat="server" type="text" class="form-control" placeholder="Enter Address" ID="addressInput" autocomplete="off" />
                    </div>
                </div>
            </div>

            <br />

            <div class="row">
                <div class="col-md-8 col-md-offset-2">
                    <div class="alert text-center" role="alert">
                        <asp:Label runat="server" ID="ErrorLabel" />
                    </div>
                </div>
            </div>



            <div class="row">
                <div class="col-md-6 col-md-offset-3">
                    <div class="btn-group btn-group-justified" role="group">
                        <div class="btn-group" role="group">
                            <asp:Button runat="server" class="btn btn-default" ID="Order" Text="Order Item" OnClick="OrderItem_OnClick_" />
                        </div>
                    </div>
                </div>
            </div>

            <br />
            <br />

            <div class="row">
                <div class="col-md-6 col-md-offset-3">
                    <button onclick="window.location.href='/TrackOrder.aspx'" type="button" class="btn btn-primary btn-lg btn-block">Already have an Order? Check their state!</button>
                </div>
            </div>

        </div>
        <asp:SqlDataSource ID="Books" runat="server" ConnectionString="<%$ ConnectionStrings:store_db %>" SelectCommand="SELECT * FROM [Book]"></asp:SqlDataSource>
    </form>
</body>
</html>
