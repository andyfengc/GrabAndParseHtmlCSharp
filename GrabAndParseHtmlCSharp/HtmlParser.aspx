<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="HtmlParser.aspx.cs" Inherits="GrabAndParseHtmlCSharp.HtmlParser" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <style type="text/css">
        .auto-style1 {
            width: 48%;
        }
        .auto-style2 {
            width: 179px;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <div>
          
    </div>
        <table class="auto-style1">
            <tr>
                <td class="auto-style2">Website:</td>
                <td>
                    <asp:TextBox ID="txtWebsite" runat="server" Width="303px"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td class="auto-style2">Query Parameter</td>
                <td>
                    <asp:TextBox ID="txtParam" runat="server" Width="305px"></asp:TextBox>
                </td>
            </tr>
        </table>
        <br />
        <asp:Button ID="btpParse" runat="server" OnClick="btpParse_Click" Text="Grab and Parse" />
        <br />
        <br />
        <asp:Label ID="lblGrabResult" runat="server"></asp:Label>
        <br />
        <br />
        <asp:Label ID="lblParseResult" runat="server"></asp:Label>
    </form>
</body>
</html>
