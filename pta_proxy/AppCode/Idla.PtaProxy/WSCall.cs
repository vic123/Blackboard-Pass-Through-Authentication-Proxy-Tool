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

using System.Collections;
using Microsoft.Web.Services3.Security;
using Microsoft.Web.Services3.Security.Tokens;


namespace Idla.PtaProxy
{
    /// <remarks>
    /// Base class for implementation of Proxy->Blackboard calls.
    /// Handles exceptions of actual call (class instance) in static function DoCall().
    /// Exposes static methods (currently only RegisterTool()) that implements functional interface to 
    /// creation of appropriate WSCall successor and invocation of its Call() method through uppermentioned DoCall()
    /// </remarks>
    public abstract class WSCall
    {

        /*
         * Just some code that may be useful in the future 
        //Microsoft.Web.Services3.WebServicesClientProtocol
        //http://msdn.microsoft.com/en-us/library/microsoft.web.services3.webservicesclientprotocol.aspx
        //Thread Safety 
        //Any public static (Shared in Visual Basic) members of this type are thread safe. Any instance members are not guaranteed to be thread safe.
        //caching of established/initialized/authenticated webservices does not seem important task by this time
        static Hashtable webservices = new Hashtable();
        static ContextWS GetContextWS(String bbHostURL) {
            String ws_key = bbHostURL + "/" + typeof(ContextWS).FullName;
            ContextWS ctx;
            lock (webservices) {
                ctx = (ContextWS)webservices[ws_key];
                if (ctx == null) {
                        ctx = new  ContextWS();
                        webservices.Add(ws_key, ctx);
                }
            }
            return ctx;
        }
        */
        /// <summary>
        /// Conditionally initializes connection to Blackboard 
        /// (this is not of some practical use by this moment as long as there are no series of calls to Blackboard - just register() one)
        /// and performs centrilized exception handling and resources cleanup for calls of Blackboard webservices by PtaProxy.
        /// Caught exceptions are logged and re-thrown.
        /// </summary>
        static public void DoCall(WSCall ws_call) {
            log4net.ILog log = PtaUtil.getLog4netLogger(typeof(WSCall).FullName + ".DoCall(): ");
            //try {
            try {
                if (ws_call.NeedsInitialize() && !ws_call.wsWrapper.IsInitialized()) {
                    ws_call.wsWrapper.initialize();
                }
                ws_call.Call();
            } catch (Exception e) {
                PtaUtil.LogExceptionAndFormParameters(log, e);
                throw;
            } finally {
                PtaUtil.CloseDataReader(ws_call.wsWrapper.GetBBoardDR());
            }
        }

        /// <summary>
        /// Interfaced call of PtaProxy registration processing
        /// </summary>
        public static void RegisterTool(String bbCode, String toolRegistrationPassword, bool IsReregister) {
            RegisterToolWSCall rtc = new RegisterToolWSCall(bbCode, toolRegistrationPassword, IsReregister);
            DoCall(rtc);
        }

        protected internal String bbHostURL;
        protected WSWrapper wsWrapper;

        /// <summary>
        /// Constructor creating new WSWrapper() - actual interface to Blackboard webservices API.
        /// </summary>
        public WSCall(String bbHostURL) {
            this.wsWrapper = new WSWrapper(bbHostURL);
        }
        /// <summary>
        /// Constructor that reuses already available WSWrapper() for current thread of processing
        /// </summary>
        public WSCall(WSWrapper wsWrapper) {
            this.wsWrapper = wsWrapper;
        }

        /// <summary>
        /// Start point of exception-handled processing
        /// </summary>
        public abstract void Call();

        /// <summary>
        /// Most of BB webservice calls need intializatian and it is made to be default
        /// </summary>
        protected virtual bool NeedsInitialize() {
            return true;
        }
    }
}
