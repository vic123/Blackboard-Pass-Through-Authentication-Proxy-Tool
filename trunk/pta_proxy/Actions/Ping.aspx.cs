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

namespace Idla.PtaProxy.Actions {
    public partial class Ping : ActionWithValidationPage {
        protected void Page_Load(object sender, EventArgs e) {
        }
        protected override void Action() {
            log4net.ILog log = PtaUtil.getLog4netLogger(this.GetType().FullName + ".ActionInternal(): ");
            base.Action();
            Response.Output.Write(PtaServerConstants.SIMPLE_SUCCESS_RESPONSE);
        }

    }
}
