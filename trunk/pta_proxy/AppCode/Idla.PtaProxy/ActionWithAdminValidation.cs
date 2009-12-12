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

namespace Idla.PtaProxy {
    /// <remarks>
    /// Parent of Actions performing validation of URL and restricted to be initiated only by Administrator (config and re-register).
    /// </remarks>
    public class ActionWithAdminValidation : ActionWithValidationPage {

        protected override void Action() {
            base.Action();
            if (!"_1_1".Equals(Request[PtaServerConstants.USERID_KEY])) {
                throw new PtaException("Access is allowed only for Blackboard Administrator (userid = \"_1_1\"). Current user_id: " + Request[PtaServerConstants.USERID_KEY]);
            }
        }
    }
}
