<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="IDLAWebTest.Login" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>PtaProxy Admin Login</title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    
        Pass-Through Authentication Proxy Administration Login Page<br /><br />
        Please provide password for pta_admin<br />
        Default pta_admin password is pta_admin<br />
    
        <asp:Login ID="Login1" runat="server" DisplayRememberMe="False">
        </asp:Login>
    
    </div>
    </form>
</body>
</html>
