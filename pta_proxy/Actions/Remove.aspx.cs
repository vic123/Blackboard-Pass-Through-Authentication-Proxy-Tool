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

using System.Data.SqlClient;

namespace Idla.PtaProxy.Actions {
    /// <remarks>
    /// http://www.edugarage.com/display/BBDN/Remove+Action
    /// </remarks>
    public partial class Remove : ActionWithValidationPage {
        protected void Page_Load(object sender, EventArgs e) {

        }
        protected override void Action() {
            log4net.ILog log = PtaUtil.getLog4netLogger(this.GetType().FullName + ".ActionInternal(): ");
            base.Action();
            System.String bb_code = PtaUtil.GetDBReaderStringField(bboardDR, "BBoardCode");
            String sql = "UPDATE BBoard SET IsRegistered = 'N', ProxyState=NULL, SharedSecret=NULL, GUID=NULL WHERE BBoardCode = @BBoardCode";
            log.Info("sql: " + sql);
            SqlParameter[] update_bb_params = new SqlParameter[] {   
                     new SqlParameter("@BBoardCode", bb_code)
                    };
            Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteNonQuery(PtaUtil.GetSqlConnection(),
                                                        System.Data.CommandType.Text, sql, update_bb_params);
            Response.Output.Write(PtaServerConstants.SIMPLE_SUCCESS_RESPONSE);
        }

    }
}
