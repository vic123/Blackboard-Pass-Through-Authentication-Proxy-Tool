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
using System.Web.Configuration;
using System.Text.RegularExpressions;

namespace Idla.PtaProxy {

    //Debugging with Soap Toolkit Trace Utility, Start Menu\Programs\Microsoft SOAP Toolkit Version 3 
    //http://www.pluralsight.com/community/blogs/jimw/archive/2009/09/03/accessing-the-visual-studio-asp-net-development-server-from-iphone.aspx

    /// <remarks>
    /// Root of targeted PtaProxy functionality - processing of end-user request pass-through redirection.
    /// Configured in pta_proxy\Web.config httpHandlers section to handle pta/*.aspx URLs.
    /// ProcessRequest() validates redirection URL, performs redirect according to logic defined in Param table
    /// for received URL (BBMenuLink.BBLinkPath is parsed out from it with help of PtaProxyWebFolder application setting from pta_proxy\Web.config),
    /// in case of error either performs Server.Transfer to location defined by BBoard.OnErrorServerTransfer value,
    /// or constructs simplest error page with link back to Blackboard.
    /// Any exceptions or error details are logged, but do not provided for end user for additional security protection.
    /// </remarks>
    public class PtaHttpHandler : IHttpHandler, System.Web.SessionState.IRequiresSessionState {

        private System.Data.SqlClient.SqlDataReader bboardDR = null;
        private System.Data.SqlClient.SqlDataReader drMenuLink = null;
        private System.Data.SqlClient.SqlDataReader drParam = null;

        private HttpContext context = null;
        private String onErrorServerTransfer = null;


        /// <summary>
        /// Standard entry point of IHttpHandler, main processing flow.
        /// </summary>
        public void ProcessRequest(HttpContext context) {
            log4net.ILog log = PtaUtil.getLog4netLogger(this.GetType().FullName + ".ProcessRequest(): ");
            try {
                this.context = context;
                String guid = context.Request.Form["ourguid"];
                bboardDR = PtaUtil.GetBBoardDataReaderByGuid(guid);
                onErrorServerTransfer = PtaUtil.GetDBReaderStringField(bboardDR, "OnErrorServerTransfer");
                RequestValidator rv = new RequestValidator(bboardDR);
                rv.Validate(context.Request);
                Redirect();
            } catch (System.Threading.ThreadAbortException ) {
                //this is normal processing of Server.Transfer() and Response.End()
                throw;
            } catch (Exception e) {
                PtaUtil.LogExceptionAndFormParameters(log, e);
                //String on_err_st = onErrorServerTransfer;
                //if (on_err_st == null) on_err_st = PtaUtil.GetProxyWebFolder() + "ErrorPages/PtaDefaultErrPage.aspx";
                //context.Server.Transfer(on_err_st, true);
                String back_link = context.Request.Form["tcbaseurl"] + context.Request.Form["returnurl"];
                String err_page = PtaUtil.GenerateSimpleErrorPage(back_link);
                context.Response.Write(err_page);
                log.Debug("onErrorServerTransfer: " + onErrorServerTransfer);
                if (onErrorServerTransfer != null) context.Server.Transfer(onErrorServerTransfer, true);
                /*                else {
                                    back_link = context.Request.Form["tcbaseurl"] + context.Request.Form["returnurl"];
                                    context.Response.Write("Your request could not be processed. <br/>");
                                    context.Response.Write("Back to Blackboard: " + "<a href=\"" + back_link + "\">" + back_link + "</a><br/>");
                                }*/

                //throw;

            } finally {
                PtaUtil.CloseDataReader(drParam);
                PtaUtil.CloseDataReader(drMenuLink);
                PtaUtil.CloseDataReader(bboardDR);
            }
        }

        /// <summary>
        /// Implements redirection logic. 
        /// Please see "V. Link redirection processing definition" in pta_proxy\readme.txt for description of processing. 
        /// Parameters provided by Blackboard along with redirection link are described at 
        /// http://www.edugarage.com/display/BBDN/Generic+Proxy+Tool+Request+Arguments
        /// Link types at
        /// http://www.edugarage.com/display/BBDN/Link+Type
        /// Response status codes and header fields:
        /// http://www.w3.org/Protocols/rfc2616/rfc2616-sec6.html#sec6
        /// http://grokable.com/understanding-asp-net-response-redirect/ 301 vs. 302: Response.Redirect sets an HTTP 302 header along with the URL to be redirected to. The 302 status code essentially says, "this item has moved temporarily". Search engines crawling your site are not guaranteed to follow 302 redirects, nor should they. By following a 302 the search engine could incorrectly index the location of content.
        /// Response.Redirect with POST instead of Get?
        /// http://stackoverflow.com/questions/46582/response-redirect-with-post-instead-of-get
        /// </summary>
        protected void Redirect() {
            log4net.ILog log = PtaUtil.getLog4netLogger(this.GetType().FullName + ".Redirect(): ");
            String bb_code = PtaUtil.GetDBReaderStringField(bboardDR, "BBoardCode");
            String proxy_web_folder = PtaUtil.GetProxyWebFolder();
            String bb_link = context.Request.Path.Replace(proxy_web_folder + PtaUtil.PTA_SUBFOLDER, "");
            log.Debug("bb_code: " + bb_code + "; proxy_web_folder: " + proxy_web_folder + "; bb_link: " + bb_link);

            System.Data.SqlClient.SqlConnection con = PtaUtil.GetSqlConnection();
            String sql = "SELECT BBLinkType FROM BBMenuLink WHERE BBoardCode = @BBoardCode AND BBLinkPath = @BBLinkPath";
            System.Data.SqlClient.SqlParameter[] sel_ml_params = new System.Data.SqlClient.SqlParameter[] {   
                     new System.Data.SqlClient.SqlParameter("@BBoardCode", bb_code) ,   
                     new System.Data.SqlClient.SqlParameter("@BBLinkPath", bb_link)
                    };
            log.Info("sql: " + sql);
            drMenuLink = Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteReader(PtaUtil.GetSqlConnection(),
                    System.Data.CommandType.Text, sql, sel_ml_params);
            if (!drMenuLink.HasRows) {
                throw new PtaException("Unknown proxy link. " + "bb_code: " + bb_code + "; proxy_web_folder: " + proxy_web_folder + "; bb_link: " + bb_link);
            }
            drMenuLink.Read();

            sql = "SELECT ParamTag, ParamOrderCode, ParamKey, ParamInputType, ParamOutputType, ParamInputTemplate, ";
            sql += "RegExp, RegExpOperation, RegExpReplaceString, RegExpMatchGroup, RegExpMatchCapture, SProcName ";
            sql +=  "FROM Param WHERE BBoardCode = @BBoardCode AND BBLinkPath = @BBLinkPath ORDER BY ParamOrderCode";
            log.Info("sql: " + sql);
            System.Data.SqlClient.SqlDataReader drParam =
                Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteReader(PtaUtil.GetSqlConnection(),
                    System.Data.CommandType.Text, sql, sel_ml_params);
            if (!drParam.HasRows) {
                throw new PtaException("No defined link parameter transformations. " + "bb_code: " + bb_code + "; proxy_web_folder: " + proxy_web_folder + "; bb_link: " + bb_link);
            }

            System.Collections.Hashtable param_tags_hash = new System.Collections.Hashtable();
            while (drParam.Read()) {
                String param_tag = PtaUtil.GetDBReaderStringField(drParam, "ParamTag");
                String param_input = PtaUtil.GetDBReaderStringField(drParam, "ParamInputType");
                String param_output = PtaUtil.GetDBReaderStringField(drParam, "ParamOutputType");
                String input_templ = PtaUtil.GetDBReaderStringField(drParam, "ParamInputTemplate");
                String param_key = PtaUtil.GetDBReaderStringField(drParam, "ParamKey"); 
                String param_value = null;
                log.Debug("param_tag: " + param_tag + "; param_input: " + param_input + "; param_output: " + param_output + "; input_templ: " + input_templ + "; param_key: " + param_key);
                switch (param_input) {
                    case "Param.InputTemplate":
                        MatchCollection mc = Regex.Matches(input_templ, "{[^}]*}");
                        foreach (Match match in mc)
                        {
                            GroupCollection groups = match.Groups;
                            foreach (Group grp in groups) {
                                String tag = grp.Value;
                                String tag_value = (string)param_tags_hash[tag];
                                input_templ = Regex.Replace(input_templ, tag, tag_value);
                                log.Debug("tag: " + tag + "; tag_value: " + tag_value + "; input_templ: " + input_templ);
                            }
                        }
                        param_value = input_templ;
                        break;
                    case "ProxyLinkPath":
                        param_value = bb_link;
                        break;
                    case "ProxyLinkType":
                        param_value = PtaUtil.GetDBReaderStringField(drMenuLink, "BBLinkType");
                        break;
                    case "ProxyPath":
                        param_value = proxy_web_folder;
                        break;
                    case "Request.ApplicationPath":
                        param_value = context.Request.ApplicationPath;
                        if (param_value.Equals("/")) param_value = "";
                        break;
                    case "Request.FilePath":
                        param_value = context.Request.FilePath;
                        break;
                    case "Request.Form":
                        param_value = context.Request.Form[param_key];
                        break;
                    case "Request.InputStream":
                        System.IO.StreamReader reader = new System.IO.StreamReader(context.Request.InputStream);
                        param_value = reader.ReadToEnd();
                        break;
                    case "Request.QueryString":
                        throw new PtaException("ParamInputType=Request.QueryString is not implemented. Supplied URL query string parameters are ignored and cut off by Blackboard.");
                    case "Request.PathInfo":
                        param_value = context.Request.PathInfo;
                        break;
                    case "Request.Url.ToString":
                        log.Debug("Request.Url.AbsolutePath(): " + context.Request.Url.AbsolutePath + "; context.Request.Url.ToString(): " + context.Request.Url.ToString());
                        param_value = context.Request.Url.ToString();
                        break;
                    case "SProcCall":
                        //defined this way list provides correct conversion psp_params.ToArray() accepted by SqlHelper.ExecuteNonQuery()
                        System.Collections.Generic.List<System.Data.SqlClient.SqlParameter> psp_params = 
                                new System.Collections.Generic.List<System.Data.SqlClient.SqlParameter>();

                        MatchCollection mc1 = Regex.Matches(input_templ, "{[^}]*}");
                        foreach (Match match in mc1) {
                            GroupCollection groups = match.Groups;
                            foreach (Group grp in groups) {
                                String tag = grp.Value;
                                String tag_value = (string)param_tags_hash[tag];
                                tag = tag.Replace("{", "").Replace("}", "");
                                log.Debug("tag: " + tag + "; tag_value: " + tag_value);
                                psp_params.Add(new System.Data.SqlClient.SqlParameter("@" + tag, tag_value));
                            }
                        }
                        String sp_out_param_name = param_key;
                        System.Data.SqlClient.SqlParameter out_param = null;
                        if (sp_out_param_name != null) {
                            out_param = //had problems with getting of output value, required ParameterDirection - this is why called constructor is so complex
                                //Initializes a new instance of the SqlParameter class that uses the parameter name, the type of the parameter, the size of the parameter, a ParameterDirection, the precision of the parameter, the scale of the parameter, the source column, a DataRowVersion to use, and the value of the parameter.
                                new System.Data.SqlClient.SqlParameter("@" + sp_out_param_name, SqlDbType.NVarChar, 
                                                                        255, ParameterDirection.InputOutput, true, 0, 0, null, 
                                                                        DataRowVersion.Current, "");
                            log.Debug("sp_out_param_name: " + sp_out_param_name);
                            psp_params.Add(out_param);
                        }
                        String sproc_name = PtaUtil.GetDBReaderStringField(drParam, "SProcName");
                        log.Debug("sproc_name: " + sproc_name);
                        if (psp_params.Count > 0) {
                            Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteNonQuery(con, CommandType.StoredProcedure, sproc_name, psp_params.ToArray());
                        } else Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteNonQuery(con, CommandType.StoredProcedure, sproc_name);
                        if (out_param != null) param_value = (string)out_param.Value;
                        break;
                    case "BBWSCall":
                        throw new PtaException("ParamInputType=BBWSCall is not implemented at this time. It may provide possibility to get additional information from Blackboard via WS calls in future.");
                    default:
                        throw new PtaException("ParamInputType=" + param_input + " is not implemented. Unknown, possible inconsistensy of code and ParamInputCheck DB constraint.");
                }
                log.Debug("param_value after input processing: " + param_value);

                String regexp_oper = PtaUtil.GetDBReaderStringField(drParam, "RegExpOperation");
                String regexp = PtaUtil.GetDBReaderStringField(drParam, "RegExp");
                String regexp_rs = PtaUtil.GetDBReaderStringField(drParam, "RegExpReplaceString");
                int regexp_mc = PtaUtil.GetDBReaderInt16Field(drParam, "RegExpMatchCapture");
                int regexp_mg = PtaUtil.GetDBReaderInt16Field(drParam, "RegExpMatchGroup");
                log.Debug("regexp_oper: " + regexp_oper + "; regexp: " + regexp + "; regexp_rs: " + regexp_rs + "; regexp_mc: " + regexp_mc + "; regexp_mg: " + regexp_mg);
                switch (regexp_oper) {
                    case "Assign":
                        break;
                    case "REMatch":
                        param_value = "";
                        MatchCollection mc3 = Regex.Matches(param_value, regexp);
                        log.Debug("mc3.Count: " + mc3.Count);
                        if (mc3.Count < regexp_mc) {
                            Match m = mc3[regexp_mc];
                            log.Debug("m.Groups.Count: " + m.Groups.Count);
                            if (m.Groups.Count < regexp_mg) {
                                param_value = m.Groups[regexp_mg].Value;
                            }
                        }
                        break;
                    case "REReplace":
                        param_value = Regex.Replace(param_value, regexp, regexp_rs);
                        break;
                    default:
                        throw new PtaException("RegExpOperation=" + regexp_oper + " is not implemented. Unknown, possible inconsistensy of code and RegExpOperationCheck DB constraint.");
                }
                log.Info("final param_tag: " + param_tag + "; param_value: " + param_value);
                param_tags_hash.Add(param_tag, param_value);

                switch (param_output) {
                    case "None":
                        break;
                    case "FormsAuthentication.SetAuthCookie":
                        System.Web.Security.FormsAuthentication.SetAuthCookie(param_value, false);
                        break;
                    case "Context.Items":
                        context.Items[param_key] = param_value;
                        break;
                    case "Context.Session":
                        context.Session.Add(param_key, param_value);
                        break;
                    case "Response.AppendHeader":
                        context.Response.AppendHeader(param_key, param_value);
                        break;
                    case "Response.Output":
                        context.Response.Output.Write(param_value);
                        break;
                    case "Response.Status":
                        context.Response.Status = param_value;
                        break;
                    case "Server.Transfer":
                        context.Server.Transfer(param_value, true);
                        break;
                    default:
                        throw new PtaException("ParamOutputType=" + param_output + " is not implemented. Unknown, possible inconsistensy of code and ParamOutputCheck DB constraint.");
                }
            } //while (drParam.Read()) {
            //http://weblogs.asp.net/bleroy/archive/2004/08/03/Don_2700_t-redirect-after-setting-a-Session-variable-_2800_or-do-it-right_2900_.aspx
            //recommends use of Response.Redirect("~/default.aspx", false);
            //In comments:
            //I was lossing session data after clicking on a new page on a web site I was building only one of the pages out of about 25 was having this issue and it was always the same page.  As soon as I copied the page on to the IIS server on my computer the problem went away.  I think that something was going wrong on the server that visual web developer 2005 was running.

            //context.Response.End(); -- does not preserve session variables
            //finally ended with no specific processing here - everything seems fine this way.
            //probably enabling of Flush() should be tried first if something will stop working as required
            //context.Response.Flush();

        }

        /// <summary>
        /// Not thread safe, utilizes object variables.
        /// </summary>
        public bool IsReusable {
            get {
                //http://neilkilbride.blogspot.com/2008/01/ihttphandler-isreusable-property.html - requires to be thread safe
                //Important when your IHttpHandler implementation does expensive initialisations, otherwise it probabaly doesn't really matter if you return true or false (since simple object allocation is fairly inexpensive in .NET).  
                return false;
            }
        }
    }
}





