<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Config.aspx.cs" Inherits="Idla.PtaProxy.Actions.Config" ValidateRequest="false" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Pass-through authenticated links configuration</title>
    <style type="text/css">
        .style1
        {
            font-size: large;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    
        <b><span class="style1">XML data of BBMenuLink and Param tables:</span></b><br />
        <asp:TextBox ID="txtXML" runat="server" Height="230px" Width="637px" 
            TextMode="MultiLine"></asp:TextBox>
        <br />
        <br />
        <asp:Button ID="btnSave" runat="server" Text="Save" Width="103px" 
            onclick="btnSave_Click" />
        <asp:Label ID="lblSaveXMLResult" runat="server"></asp:Label>
    
    </div>
    <asp:HyperLink ID="hlBackToBB" runat="server">Back to Blackboard</asp:HyperLink>
    <asp:HiddenField ID="hfGuid" runat="server" />
    </form>
</body>
</html>
