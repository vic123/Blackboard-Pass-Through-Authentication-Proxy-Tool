<%@ Page Language="C#" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<%@ Import Namespace="System" %>
<%@ Import Namespace="System.Data" %>
<%@ Import Namespace="System.Web.Security" %>

<script runat="server">

        protected void Page_Load(object sender, EventArgs e) {

            Response.Write("Hello, " + Server.HtmlEncode(User.Identity.Name));

            if (User.Identity.GetType() == typeof(System.Web.Security.FormsIdentity)) {
                FormsIdentity id = (FormsIdentity)User.Identity;
                FormsAuthenticationTicket ticket = id.Ticket;
                Response.Write("<p/>Forms Identity Ticket: ");
                Response.Write("<br/>TicketName: " + ticket.Name);
                Response.Write("<br/>Cookie Path: " + ticket.CookiePath);
                Response.Write("<br/>Ticket Expiration: " +
                                ticket.Expiration.ToString());
                Response.Write("<br/>Expired: " + ticket.Expired.ToString());
                Response.Write("<br/>Persistent: " + ticket.IsPersistent.ToString());
                Response.Write("<br/>IssueDate: " + ticket.IssueDate.ToString());
                Response.Write("<br/>UserData: " + ticket.UserData);
                Response.Write("<br/>Version: " + ticket.Version.ToString());
            } else if (User.Identity.GetType() == typeof(System.Security.Principal.WindowsIdentity)) {
                    System.Security.Principal.WindowsIdentity id = (System.Security.Principal.WindowsIdentity)User.Identity;
                    Response.Write("<p/>WindowsIdentity: ");
                    Response.Write("<br/>WindowsIdentity.Name: " + id.Name);
            }


            HttpContext ctx = HttpContext.Current;
            HttpResponse response = ctx.Response;
            HttpRequest request = ctx.Request;
            System.Web.SessionState.HttpSessionState session = ctx.Session;

            response.Write("URL: " + ctx.Request.Url.ToString() + "<p/>");
            response.Write("Querystring:<p/>");

            for (int i = 0; i < request.QueryString.Count; i++) {
                response.Write("<br/>" + request.QueryString.Keys[i].ToString() + ": " + request.QueryString[i].ToString() + "<br/>");// + nvc.
            }

            response.Write("<p>-------------------------<p/>Form:<p/>");

            for (int i = 0; i < request.Form.Count; i++) {
                response.Write("<br/>" + request.Form.Keys[i].ToString() + ": " + request.Form[i].ToString() + "<br/>");// + nvc.
            }

            response.Write("<p>-------------------------<p/>Headers:<p/>");

            for (int i = 0; i < request.Headers.Count; i++) {
                response.Write("<br/>" + request.Headers.Keys[i].ToString() + ": " + request.Headers.Get(i) + "<br/>");// + nvc.
            }


            response.Write("<p>-------------------------<p/>Session:<p/>");

			response.Write("<br/> session.Count: " + session.Count.ToString()  + "<br/>");// + nvc.
            for (int i = 0; i < session.Count; i++) {
                String sess_key = "null";
                if (session.Keys[i] != null) sess_key = session.Keys[i].ToString();
                response.Write("<br/>" + sess_key + ": " + session[i] + "<br/>");// + nvc.
            }

            response.Write("<p>-------------------------<p/>RequestType: " + request.RequestType + "<p/>");
             

            response.Write("<p>-------------------------<p/>Body:<p/>");
            System.IO.StreamReader reader = new System.IO.StreamReader(request.InputStream);
            String page_body = reader.ReadToEnd();
            response.Write("<br/>" + page_body + "<br/>");
		}
</script>


<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Untitled Page</title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    
    </div>
    </form>
</body>
</html>
