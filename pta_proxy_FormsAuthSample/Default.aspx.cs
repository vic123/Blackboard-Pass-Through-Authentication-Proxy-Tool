using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Web.Security;

namespace IDLAWebTest
{
    public partial class _Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
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

        }
    }
}
