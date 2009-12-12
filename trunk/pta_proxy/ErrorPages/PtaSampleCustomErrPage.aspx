<%@ Page Language="C#" AutoEventWireup="true" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<script runat="server">
        protected void Page_Load(object sender, EventArgs e) {
//            Response.Clear();
        }
</script>


<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Custom error page</title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    	---------------------------------------------------- <br/>
        The line above, and this text come from custom error page test/sample specified in BBoard.OnErrorServerTransfer. <br/>
        It may clear "inherited" standard error response preserved above the line with Response.Clear() <br/> </div>
    </form>
</body>
</html>
