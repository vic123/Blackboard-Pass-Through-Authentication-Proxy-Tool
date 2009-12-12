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
    /// Parent of Actions performing validation of URL (by this moment all implemented actions require validation).
    /// </remarks>
    public class ActionWithValidationPage : ActionPage {

        protected override void Action() {
            base.Action();
            String guid = Request[PtaServerConstants.OURGUID_KEY];
            bboardDR = PtaUtil.GetBBoardDataReaderByGuid(guid);
            RequestValidator rv = new RequestValidator(bboardDR);
            rv.Validate(Request);
        }
        

    }
}
