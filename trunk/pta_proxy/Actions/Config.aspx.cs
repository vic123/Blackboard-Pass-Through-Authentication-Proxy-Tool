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
    /// http://www.edugarage.com/display/BBDN/Config+Action
    /// Logic of config action handling does not fit well into current
    /// ActionPage,ActionWithValidationPage... hierarchy because requires 
    /// different that SIMPLE_ERROR_RESPONSE message in case of error (exception).
    /// Current implementation is a prototype most necessary processing. 
    /// Probably more integration with Admin/Register.aspx is required.
    /// </remarks>
    public partial class Config : System.Web.UI.Page {
        bool rePost = false;
        SqlDataReader bboardDR;
        protected void Page_Load(object sender, EventArgs e) {
            log4net.ILog log = PtaUtil.getLog4netLogger(this.GetType().FullName + ".Page_Load(): ");
            if (!"Save".Equals(Request.Form["btnSave"])) {
                try {
                    log.Debug("Request.ContentEncoding.EncodingName: " + Request.ContentEncoding.EncodingName);
                    log.Debug("Response.ContentEncoding.EncodingName: " + Response.ContentEncoding.EncodingName);
                    Request.ContentEncoding = System.Text.Encoding.UTF8;
                    Response.ContentEncoding = System.Text.Encoding.UTF8;
                    hfGuid.Value = Request[PtaServerConstants.OURGUID_KEY];
                    bboardDR = PtaUtil.GetBBoardDataReaderByGuid(hfGuid.Value);
                    RequestValidator rv = new RequestValidator(bboardDR);
                    rv.Validate(Request);
                    if (!"_1_1".Equals(Request[PtaServerConstants.USERID_KEY])) {
                        throw new PtaException("Access is allowed only for Blackboard Administrator (userid = \"_1_1\"). Current user_id: " + Request[PtaServerConstants.USERID_KEY]);
                    }
                    hlBackToBB.NavigateUrl = Request.Form["tcbaseurl"] + Request.Form["returnurl"];
                    lblSaveXMLResult.Text = "";
                    //throw new PtaException("test how error will be processed on remote config");
                    String bb_code = PtaUtil.GetDBReaderStringField(bboardDR, "BBoardCode");
                    txtXML.Text = PtaUtil.GetBBMenuLinkParamXML(bb_code);
                } catch (Exception exc) {
                    PtaUtil.LogExceptionAndFormParameters(log, exc);
                    String err_page = PtaUtil.GenerateSimpleErrorPage(hlBackToBB.NavigateUrl);
                    Response.Write(err_page);
                    Response.End();
                } finally {
                    PtaUtil.CloseDataReader(bboardDR);
                }
            }
        }

/*
        protected override void Action() {
            log4net.ILog log = PtaUtil.getLog4netLogger(this.GetType().FullName + ".Action(): ");
            try {
                if (!"Save".Equals(Request.Form["btnSave"])) {
                    backLink = Request.Form["tcbaseurl"] + Request.Form["returnurl"];
                    lblSaveXMLResult.Text = "";
                    hlBackToBB.NavigateUrl = backLink;
                    base.Action();
                    //throw new PtaException("test how error will be processed on remote config");
                    String bb_url = PtaUtil.GetDBReaderStringField(bboardDR, "BBoardURL");
                    txtXML.Text = PtaUtil.GetBBMenuLinkParamXML(bb_url);
                }
            } catch (Exception exc) {
                PtaUtil.LogExceptionAndFormParameters(log, exc);
                String err_page = PtaUtil.GenerateSimpleErrorPage(backLink);
                Response.Write(err_page);
                Response.End();
            }
        }
 */
        protected void btnSave_Click(object sender, EventArgs e) {
            log4net.ILog log = PtaUtil.getLog4netLogger(this.GetType().FullName + ".btnSave_Click(): ");
            try {
                bboardDR = PtaUtil.GetBBoardDataReaderByGuid(hfGuid.Value);
                PtaUtil.SetBBMenuLinkParamXML(PtaUtil.GetDBReaderStringField(bboardDR, "BBoardCode"), txtXML.Text);
                lblSaveXMLResult.Text = "SUCCESS";
            } catch (Exception exc) {
                PtaUtil.LogExceptionAndFormParameters(log, exc);
                String err_page = PtaUtil.GenerateSimpleErrorPage(hlBackToBB.NavigateUrl);
                //this may not be super secure, but assuming that Blackboard Administrator 
                //is checked person and he may really need exception details upon save error
                err_page += PtaUtil.GetExceptionAndEnvHtml(exc);
                Response.Write(err_page);
                Response.End();
            } finally {
                    PtaUtil.CloseDataReader(bboardDR);
                }


        }
    }
}
