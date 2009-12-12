using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;

using System.Web.Configuration;

namespace Idla.PtaProxy.Actions {
    /// <remarks>
    /// http://www.edugarage.com/display/BBDN/Reregister+Action
    /// </remarks>
    public partial class ReRegister : ActionWithAdminValidation {
        protected void Page_Load(object sender, EventArgs e) {

        }
        protected override void Action() {
            log4net.ILog log = PtaUtil.getLog4netLogger(this.GetType().FullName + ".ActionInternal(): ");
            base.Action();
            String bb_code = PtaUtil.GetDBReaderStringField(bboardDR, "BBoardCode");
            String bbRegPwd = Request.Form[PtaServerConstants.REGISTRATION_PASSWORD_KEY];

            String proxy_folder = Request.Path;
            log.Debug("proxy_folder before Replace(): " + proxy_folder);
            proxy_folder = proxy_folder.ToLower().Replace("/actions/reregister.aspx", "");
            //remove prefixed "/" 
            proxy_folder = proxy_folder.Substring(1);
            log.Debug("Request.ApplicationPath: " + Request.ApplicationPath + "; Request.Path: " + Request.Path + "; proxy_folder: " + proxy_folder);
            //if (proxy_folder.Length != 0) proxy_folder = proxy_folder.Substring(1); 
            if (!PtaUtil.GetProxyWebFolder().Contains(proxy_folder))
                throw new Idla.PtaProxy.PtaException("Possibly bad AppSettings[\"PtaProxyWebFolder\"] = " + WebConfigurationManager.AppSettings["PtaProxyWebFolder"]
                        + "; Calculated proxy_folder = " + proxy_folder + "; PtaUtil.GetProxyWebFolder(): " + PtaUtil.GetProxyWebFolder());
            Idla.PtaProxy.RegisterToolWSCall.RegisterTool(bb_code, bbRegPwd, true);
            Response.Output.Write(PtaServerConstants.SIMPLE_SUCCESS_RESPONSE);
        }
    }
}
