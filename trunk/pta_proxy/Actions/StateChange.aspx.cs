using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

using System.Data.SqlClient;

using Idla.PtaProxy;

namespace Idla.PtaProxy.Actions
{
    /// <remarks>
    /// http://www.edugarage.com/display/BBDN/State+Change+Action
    /// </remarks>
    public partial class StateChange : ActionWithValidationPage 
    {
        protected override void Action() {
            log4net.ILog log = PtaUtil.getLog4netLogger(this.GetType().FullName + ".ActionInternal(): ");
            base.Action();

            System.String proxy_state = Request[PtaServerConstants.STATE_KEY];
            System.String bb_code = PtaUtil.GetDBReaderStringField(bboardDR, "BBoardCode");
            String sql = "UPDATE BBoard SET ProxyState = @ProxyState WHERE BBoardCode = @BBoardCode";
            log.Info("sql: " + sql);
            SqlParameter[] update_bb_params = new SqlParameter[] {   
                     new SqlParameter("@ProxyState", proxy_state) ,   
//                     new SqlParameter("@IsRegistered", is_reg) ,   
                     new SqlParameter("@BBoardCode", bb_code)
                    };
            log.Info("proxy_state: " + proxy_state);
            log.Info("bb_code: " + bb_code);
            Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteNonQuery(PtaUtil.GetSqlConnection(),
                                                        System.Data.CommandType.Text, sql, update_bb_params);
            Response.Output.Write(PtaServerConstants.SIMPLE_SUCCESS_RESPONSE);
        }

    }


}
