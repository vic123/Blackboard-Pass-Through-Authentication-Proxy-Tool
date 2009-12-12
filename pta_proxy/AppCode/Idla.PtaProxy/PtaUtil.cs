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
using System.Web.Configuration;

// Configure logging for this assembly using the 'SimpleApp.exe.log4net' file 
//http://logging.apache.org/log4net/release/manual/configuration.html
//[assembly: log4net.Config.XmlConfigurator(ConfigFileExtension = "log4net", Watch = true)]
[assembly: log4net.Config.XmlConfigurator(ConfigFile = "bin\\pta_proxy.log4net", Watch = true)]



namespace Idla.PtaProxy
{
    /// <remarks>
    /// Utility functions that did not find their place (yet) in more specialized classes
    /// </remarks>
    public class PtaUtil
    {
        static public readonly String PTA_SUBFOLDER = "pta/";
        //static public readonly String PTA_SUBFOLDER = "pass/through.auth/";

        static readonly String SAMPLE_SQL_CONNECTION_STRING = @"
<connectionStrings>
    <add
    name=""PtaProxyConnectionString""
    connectionString=""Data Source=localhost\SQL2K5;Initial Catalog=PtaProxy;Integrated Security=True""
    providerName=""System.Data.SqlClient""
    />
</connectionStrings>";


        /// <summary>
        /// Creates new SQL Server connection using PtaProxySQLConnectionString connection string from web.config.
        /// Opens it. No custom connection caching was implemented because 
        /// http://msdn.microsoft.com/en-us/library/8xx3tyca(VS.71).aspx states:
        /// The .NET Framework Data Provider for SQL Server provides connection pooling automatically for your ADO.NET client application. 
        /// </summary>
        public static SqlConnection GetSqlConnection()
		{
            ConnectionStringSettings constr = WebConfigurationManager.ConnectionStrings["PtaProxySQLConnectionString"];
            if (constr == null)
            {
                throw new PtaException("Add PtaProxySQLConnectionString to connectionStrings of web.xml. Sample: " + SAMPLE_SQL_CONNECTION_STRING);
            } else {
                string connectionString = constr.ConnectionString;
                System.Data.SqlClient.SqlConnection connection = new SqlConnection(connectionString);
			    connection.Open();
			    return connection;
            }
		}

        /// <summary>
        /// Reads AppSettings["PtaProxyWebFolder"] and converts obtained string to contain either single "/" 
        /// or both starting and ending "/" for simplification of concatenations.
        /// This logic may be changed to become compatible with ApplicationPath handling in PtaHttpHandler case "Request.ApplicationPath": -
        /// to return empty string instead of only root "/" and provide path with starting but without ending slash
        /// </summary>
        public static String GetProxyWebFolder() {
            String proxy_web_folder = WebConfigurationManager.AppSettings["PtaProxyWebFolder"];
            if (proxy_web_folder.Length != 0) {
                if (proxy_web_folder.Substring(0, 1) != "/") proxy_web_folder = "/" + proxy_web_folder;
                if (proxy_web_folder.Substring(proxy_web_folder.Length - 1, 1) != "/") proxy_web_folder += "/";
            } else proxy_web_folder = "/";
            return proxy_web_folder;
        }

        /// <summary>
        /// Null-safe and logged access of String fields from SqlDataReader.
        /// </summary>
        public static String GetDBReaderStringField(System.Data.SqlClient.SqlDataReader dbReader, String fieldName) {
            log4net.ILog log = PtaUtil.getLog4netLogger(typeof(PtaUtil).FullName + ".GetDBReaderStringField(): ");
            log.Debug("fieldName: " + fieldName);
            if (dbReader[fieldName] != System.DBNull.Value) {
                return (string)dbReader[fieldName];
            } else return null;
        }
        /// <summary>
        /// Null-safe and logged access of Int16 fields from SqlDataReader. Null value is replaced with -1. 
        /// </summary>
        public static Int16 GetDBReaderInt16Field(System.Data.SqlClient.SqlDataReader dbReader, String fieldName) {
            log4net.ILog log = PtaUtil.getLog4netLogger(typeof(PtaUtil).FullName + ".GetDBReaderIntField(): ");
            log.Debug("fieldName: " + fieldName);
            if (dbReader[fieldName] != System.DBNull.Value) {
                return (Int16)dbReader[fieldName];
            } else return -1;
        }

        
        /// <summary>
        /// By now invoked only by pta_proxy\AppCode\Idla.PtaProxy\GlobalErrorHandler.cs, 
        /// collects and displays available data on parameter values and exceptions.
        /// Not very clean because neccessity of GlobalErrorHandler.cs is under a question by itself.
        /// pta_proxy\TestRedirect\InfoTest.aspx contains more detailed implementation. 
        /// </summary>
        public static String GetExceptionAndEnvHtml(Exception exception) {

            HttpContext ctx = HttpContext.Current;
            HttpResponse response = ctx.Response;
            HttpRequest request = ctx.Request;

            String ex_html = "";
            ex_html += "Your request could not be processed. Please press the back button on your browser and try again.<br/> \r\n";
            ex_html += "If the problem persists, please contact technical support<p/> \r\n";
            ex_html += "Information below is for technical support:<p/> \r\n";

            ex_html += "URL: " + ctx.Request.Url.ToString() + "<p/> \r\n";
            ex_html += "Querystring:<p/>\r\n";

            for (int i = 0; i < request.QueryString.Count; i++) {
                ex_html += "<br/>\r\n" + request.QueryString.Keys[i].ToString() + ": " + request.QueryString[i].ToString() + "<br/>\r\n";// + nvc.
            }

            ex_html += "<p>-------------------------<p/>\r\nForm:<p/>\r\n";

            for (int i = 0; i < request.Form.Count; i++) {
                ex_html += "<br/>\r\n" + request.Form.Keys[i].ToString() + ": " + request.Form[i].ToString() + "<br/>\r\n";// + nvc.
            }

            ex_html += "<p>-------------------------<p/>\r\nErrorInfo:<p/>\r\n";

            Exception e = exception.GetBaseException();

            e = exception;
//            log.Error("Message: " + e.Message, e);
            String inner_e_prexif = "";
            while (e != null) {
                string errorInfo = "<p/> \r\n" + inner_e_prexif
                        + e.GetType().FullName
                        + " Error Message:<br/>\r\n" + e.Message + "<p/>\r\nStacktrace:<br/>\r\n"; // +e.StackTrace;
                // Create trace from exception 
                /*
                var trace = new System.Diagnostics.StackTrace(e);
                int frameCount = trace.FrameCount;
                for (int i = 0; i < trace.FrameCount; i++) {
                    System.Diagnostics.StackFrame frame = trace.GetFrame(i);
                    errorInfo += frame.ToString() + "<br/>";
                    // Write properties to formatted HTML, including frame.GetMethod()/frame.GetFileName(), etc. 
                    // The specific format is really up to you. 
                }
                 */
                errorInfo += "<PRE>" + e.StackTrace + "</PRE><br/>\r\n";
                // or for current code location 
                //var trace = new System.Diagnostics.StackTrace(true); 

                ex_html += errorInfo;
                //log.Warn("ws_call.GetType(): " + ws_call.GetType().FullName + "; + e_in.ToString(): " + e_in.ToString(), e_in);
                //log.Warn("ws_call.GetType(): " + ws_call.GetType().FullName + "; + e_in.StackTrace(): " + e_in.StackTrace, e_in);
                e = e.InnerException;
                inner_e_prexif += "InnerException.";
            }
            ex_html += "<p>-------------------------<p/>\r\nRequest body:<p/>\r\n";
            //response.Write("<br/>" + request.InputStream.ToString() + "<br/>");
            //request.InputStream.
            System.IO.StreamReader reader = new System.IO.StreamReader(request.InputStream);
            String page_body = reader.ReadToEnd();
            ex_html += "<br/>\r\n" + page_body + "<br/>\r\n";
            return ex_html;

        }
        /// <summary>
        /// Checked closing of SqlDataReader
        /// </summary>
        public static void CloseDataReader(System.Data.SqlClient.SqlDataReader dr) {
            if (dr != null) {
                if (!dr.IsClosed) dr.Close();
                dr.Dispose();
            }
        }

        /// <summary>
        /// Opens single BBoard record by GUID (parameter passed from Blackboard)
        /// </summary>
        public static System.Data.SqlClient.SqlDataReader GetBBoardDataReaderByGuid(String guid) {
            log4net.ILog log = PtaUtil.getLog4netLogger(typeof(PtaUtil).FullName + ".getBBoardDataReaderByGuid(): ");
            System.Data.SqlClient.SqlConnection con = PtaUtil.GetSqlConnection();
            String sql = "SELECT * FROM BBoard WHERE GUID = @GUID";
            System.Data.SqlClient.SqlParameter param = new System.Data.SqlClient.SqlParameter("GUID", guid);
            log.Info("sql: " + sql + ";guid: " + guid);
            System.Data.SqlClient.SqlDataReader dr =
                Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteReader(PtaUtil.GetSqlConnection(),
                    System.Data.CommandType.Text, sql, param);
            if (!dr.HasRows) {
                throw new PtaException("Unregistered Blackboard GUID: " + guid);
            }
            dr.Read();
            return dr;
        }

        /// <summary>
        /// Opens single BBoard record by URL (identification of unregistered BBoard record by pta_proxy\Admin\Register.aspx 
        /// </summary>
        public static System.Data.SqlClient.SqlDataReader GetBBoardDataReaderByCode(String bbCode) {
            log4net.ILog log = PtaUtil.getLog4netLogger(typeof(PtaUtil).FullName + ".GetBBoardDataReaderByURL(): ");
            System.Data.SqlClient.SqlConnection con = PtaUtil.GetSqlConnection();
            String sql = "SELECT * FROM BBoard WHERE BBoardCode = @BBoardCode";
            System.Data.SqlClient.SqlParameter param = new System.Data.SqlClient.SqlParameter("BBoardCode", bbCode);
            log.Info("sql: " + sql + ";bbCode: " + bbCode);
            System.Data.SqlClient.SqlDataReader dr =
                Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteReader(PtaUtil.GetSqlConnection(),
                    System.Data.CommandType.Text, sql, param);
            if (!dr.HasRows) {
                throw new PtaException("Unregistered Blackboard Code: " + bbCode);
            }
            dr.Read();
            return dr;
        }

        /// <summary>
        /// Obtains logger and logs "method entered" debug message, code is following
        /// convention to obtain logger as first statement in the method.
        /// </summary>
        public static log4net.ILog getLog4netLogger(String name) {
            log4net.ILog log = log4net.LogManager.GetLogger(name);
            log.Debug("Method entered");
            return log;
        }

        /// <summary>
        /// Reads BBMenuLink and Param tables from database in form of XML
        /// </summary>
        public static String GetBBMenuLinkParamXML(String bbCode) {
            log4net.ILog log = PtaUtil.getLog4netLogger(typeof(PtaUtil).FullName + ".GetBBMenuLinkParamXML(): ");
            SqlConnection con = GetSqlConnection();
            String sql = "SELECT * from BBMenuLink join Param " 
	                    + " on BBMenuLink.BBoardCode = Param.BBoardCode and BBMenuLink.BBLinkPath = Param.BBLinkPath "
                        + " where BBMenuLink.BBoardCode = @BBoardCode ORDER BY BBMenuLink.BBLinkPath, Param.ParamOrderCode for xml auto";
            System.Data.SqlClient.SqlParameter param = new System.Data.SqlClient.SqlParameter("BBoardCode", bbCode);
            log.Info("sql: " + sql);
            System.Data.SqlClient.SqlDataReader dr =
                Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteReader(PtaUtil.GetSqlConnection(),
                    System.Data.CommandType.Text, sql, param);
            String xml = null;
            if (dr.HasRows) {
                xml = "";
                while (dr.Read()) {
                    xml += dr.GetString(0);
                }
                 
                xml = xml.Replace("><", ">" + System.Environment.NewLine + "<");
            }
            return xml;
        }

        /// <summary>
        /// Saves XML to BBMenuLink and Param tables (relies on SetBBMenuLinkParamXML stored procedure)
        /// </summary>
        public static void SetBBMenuLinkParamXML(String bbCode, String xml) {
            log4net.ILog log = PtaUtil.getLog4netLogger(typeof(PtaUtil).FullName + ".SetBBMenuLinkParamXML(): ");
            SqlParameter[] param_arr = new SqlParameter[] {   
                new System.Data.SqlClient.SqlParameter("@BBoardCode", bbCode),
                new System.Data.SqlClient.SqlParameter("@Xml", xml)
            };
            SqlConnection con = GetSqlConnection();
            Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteNonQuery(
                    con, CommandType.StoredProcedure, "SetBBMenuLinkParamXML", param_arr);
        }

        public static String GenerateSimpleErrorPage(String backLink) {
            String err_page = "<br/><br/>Your request could not be processed. <br/>"
            + "<a href=\"" + backLink + "\">Back to Blackboard</a><br/>";
            return err_page;
        }

        public static void LogExceptionAndFormParameters(log4net.ILog log, Exception exception) {
            //just in case if we get some more exceptions during GetExceptionAndEnvHtml()
            log.Error(exception);
            //ex_html by itself will include exception info too, traced back to all InnerExceptions
            String ex_html = PtaUtil.GetExceptionAndEnvHtml(exception);
            log.Info(ex_html);
        }

    }
}
