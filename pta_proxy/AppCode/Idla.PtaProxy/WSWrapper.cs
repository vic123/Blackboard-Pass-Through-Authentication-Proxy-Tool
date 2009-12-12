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

using Microsoft.Web.Services3.Security;
using Microsoft.Web.Services3.Security.Tokens;
using System.Data.SqlClient;


namespace Idla.PtaProxy
{
    /// <remarks>
    /// Wrapper for generated interfaces of Blackboard web services. 
    /// Its main function is to handle current state of communication (initializedFlag).
    /// and Blackboard instance related data (bboardDR)
    /// </remarks>

    public class WSWrapper
    {
        protected ContextWS contextWS;
        protected bool initializedFlag;
        protected SqlDataReader bboardDR;

        /// <summary>
        /// Opens bboardDR
        /// </summary>
        public WSWrapper(String bbHostCode) {
            bboardDR = PtaUtil.GetBBoardDataReaderByCode(bbHostCode);
            contextWS = new ContextWS();
            this.initializedFlag = false;
        }
        /// <summary>
        /// Initializes communication channel with Blackboard.
        /// Depending on BBoard.BBoardHTTPSchema DB setting first tries to establish https 
        /// and in case of fail - http one. Or makes single attempt on one of them if restricted to such behavior.
        /// </summary>
        public void initialize() {
            log4net.ILog log = PtaUtil.getLog4netLogger(this.GetType().FullName + ".initialize(): ");
            String url_prefix = "http://";
            Int16 port_num = PtaUtil.GetDBReaderInt16Field(bboardDR, "BBoardHTTPPort");
            String bb_http_schema = "";
            try {
                bb_http_schema = PtaUtil.GetDBReaderStringField(bboardDR, "BBoardHTTPSchema");
                log.Debug("bb_http_schema: " + bb_http_schema);
                if ("HTTPS".Equals(bb_http_schema)
                    || "BOTH".Equals(bb_http_schema)
                        ) {
                    url_prefix = "https://";
                    port_num = PtaUtil.GetDBReaderInt16Field(bboardDR, "BBoardHTTPSPort");
                } else if (!"HTTP".Equals(bb_http_schema)) {
                    throw new PtaException("BBoardHTTPSchema has to contain either HTTP, HTTPS or BOTH.");
                }
                contextWS.Url = url_prefix + GetBBHostURL() + ":" + port_num.ToString() + "/webapps/ws/services" + "/Context.WS";
                initializeInternal();
            } catch (System.Net.WebException we) {
                //checking of message text looks to be too strong condition 
                //When certificate is incorrect message is "The underlying connection was closed: Could not establish trust relationship for the SSL/TLS secure channel."
                //When certificate is incorrect message is "The underlying connection was closed: An unexpected error occurred on a send" and inner exception "Authentication failed because the remote party has closed the transport stream."
                //if (we.Message.Equals("The underlying connection was closed: Could not establish trust relationship for the SSL/TLS secure channel.")
                    log.Warn(we);
                    if ("BOTH".Equals(bb_http_schema)) {
                        port_num = PtaUtil.GetDBReaderInt16Field(bboardDR, "BBoardHTTPPort");
                        contextWS.Url = "http://" + GetBBHostURL() + ":" + port_num.ToString() + "/webapps/ws/services" + "/Context.WS";
                        initializeInternal();
                } else throw;
            }
        }


        /// <summary>
        /// Separarted technical initialization processing, called by initialize()
        /// </summary>
        protected void initializeInternal() {
            log4net.ILog log = PtaUtil.getLog4netLogger(this.GetType().FullName + ".initializeInternal(): ");
            log.Debug("Before contextWS.initialize - contextWS.RequestSoapContext.ContentType: " + contextWS.RequestSoapContext.ContentType);
            //contextWS.ResponseSoapContext is null here
            //log.Debug("Before contextWS.initialize - contextWS.ResponseSoapContext.ContentType: " + contextWS.ResponseSoapContext.ContentType);
            contextWS.RequestEncoding = System.Text.Encoding.UTF8;
            UsernameToken userToken = new UsernameToken("session", "nosession", PasswordOption.SendPlainText);
            contextWS.RequestSoapContext.Security.Tokens.Clear();
            contextWS.RequestSoapContext.Security.Tokens.Add(userToken);
            //NOTE that this session is only valid for 5 minutes - in other words you must login within 5 minutes or call initialize again.
            initializeResponse ir = contextWS.initialize();
            String sessionId = ir.@return;
            log.Debug("After contextWS.initialize - contextWS.ResponseSoapContext.ContentType: " + contextWS.ResponseSoapContext.ContentType);
            userToken = new UsernameToken("session", sessionId, PasswordOption.SendPlainText);
            contextWS.RequestSoapContext.Security.Tokens.Clear();
            contextWS.RequestSoapContext.Security.Tokens.Add(userToken);
            initializedFlag = true;
        }


        public String GetBBHostCode() {
            return PtaUtil.GetDBReaderStringField(bboardDR, "BBoardCode");
        }
        private String GetBBHostURL() {
            return PtaUtil.GetDBReaderStringField(bboardDR, "BBoardURL");
        }

        public bool IsInitialized() {
            return initializedFlag;
        }

        public ContextWS GetContextWS() {
            return contextWS;
        }
        public SqlDataReader GetBBoardDR() {
            return bboardDR;
        }


    }
}
