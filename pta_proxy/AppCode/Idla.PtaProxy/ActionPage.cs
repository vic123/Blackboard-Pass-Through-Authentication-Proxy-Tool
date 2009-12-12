using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using System.Data.SqlClient;

namespace Idla.PtaProxy {
    /// <remarks>
    /// Base class for all actions - requests initiated by Blackboard.
    /// Implements exception handling in PerformAction(), responds with SIMPLE_ERROR_RESPONSE
    /// upon catching of exception.
    /// In finally closes DataReader that is used by most of implemented actions.
    /// Base Action() implementation sets ContentEncoding, it is intended to be called in the begining of 
    /// request processing by child actions.
    /// Final ancestors of ActionPage are located in pta_proxy/Actions subfolder
    /// </remarks>
    public abstract class ActionPage : System.Web.UI.Page {
        protected SqlDataReader bboardDR;

//        static public void PerformAction(ActionPage a_page) {
        //this way it may get overriden in Actions.Config, but still does not look like comfortable enough design
        public virtual void PerformAction(ActionPage a_page) {
            log4net.ILog log = PtaUtil.getLog4netLogger(typeof(ActionPage).FullName + ".PerformAction(): ");
            try {
                a_page.Action();
            } catch (System.Threading.ThreadAbortException ) {
                //this is normal processing of Server.Transfer() and Response.End()
                throw;
            } catch (Exception e) {
                PtaUtil.LogExceptionAndFormParameters(log, e);
                HttpContext.Current.Response.Output.Write(PtaServerConstants.SIMPLE_ERROR_RESPONSE);
            } finally {
                PtaUtil.CloseDataReader(a_page.bboardDR);
            }
        }

        //OnLoad vs. Page_Load vs. Load event
        //http://weblogs.asp.net/infinitiesloop/archive/2008/03/24/onload-vs-page-load-vs-load-event.aspx
        protected override void OnLoad(EventArgs e)  {
            base.OnLoad(e);
            PerformAction(this);
        }

        protected virtual void Action() {
            log4net.ILog log = PtaUtil.getLog4netLogger(this.GetType().FullName + ".Action(): ");
            log.Debug("Request.ContentEncoding.EncodingName: " + Request.ContentEncoding.EncodingName);
            log.Debug("Response.ContentEncoding.EncodingName: " + Response.ContentEncoding.EncodingName);
            Request.ContentEncoding = System.Text.Encoding.UTF8;
            Response.ContentEncoding = System.Text.Encoding.UTF8;
        }
    }
}
