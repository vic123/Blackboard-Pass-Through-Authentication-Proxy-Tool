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
using System.Collections;


namespace Idla.PtaProxy {
    /// <remarks>
    /// Request validation mechanism. Used both for end-user pass-though authentication followed links 
    /// and Blackboard actions. 
    /// Separated into standalone class mainly because of static knownNonces collection.
    /// </remarks>
    public class RequestValidator {
        private System.Data.SqlClient.SqlDataReader bboardDR = null;
        private String guid = null;
        private static System.Collections.Hashtable knownNonces = new System.Collections.Hashtable();
        private static int nonceCleanupCounter = 0;

        /// <summary>
        /// This constructor will cause independent reading of BBoard table by Validate() method using GUID as WHERE clause parameter.
        /// Not used by current code (Nov29-2009)
        /// </summary>
        public RequestValidator(String guid) : base() {
            this.guid = guid;
        }

        /// <summary>
        /// This constructor will cause using of currently selected record of already opened BBoard table by Validate() method
        /// </summary>
        public RequestValidator(System.Data.SqlClient.SqlDataReader bboardDR) : base() {
            this.bboardDR = bboardDR;
        }

        /// <summary>
        /// Performs series of actions necessary for validation of passed by Blackboard parameters.
        /// See http://www.edugarage.com/display/BBDN/Proxy+Tool+SSO , internal method comments and java client sample code for details. 
        /// Performs periodical cleanup of collected nonces upon every NonceCleanupCount invocation 
        /// </summary>
        public void Validate(HttpRequest request) {
            log4net.ILog log = PtaUtil.getLog4netLogger(this.GetType().FullName + ".Validate(): ");
            request.ContentEncoding = System.Text.Encoding.UTF8;


            if (bboardDR == null) bboardDR = PtaUtil.GetBBoardDataReaderByGuid(guid);

            String s_secret = PtaUtil.GetDBReaderStringField(bboardDR, "SharedSecret");

            //http://www.edugarage.com/display/BBDN/Proxy+Tool+SSO
            // 1. Sort the posted data based on the key names and then concatenate all of the values together in the sorted order 
            String[] sorted_form_keys = request.Form.AllKeys;
            Array.Sort(sorted_form_keys);
            System.Text.StringBuilder data_string = new System.Text.StringBuilder("");
            foreach (String form_key in sorted_form_keys) {
                /* from proxy\java\src\com\blackboard\test\proxy\GenericParameters.java:
                 * // ignore extra parameters added after the mac was generated/set
                    if ( key.startsWith( "top_" ) || key.startsWith( "bottom_" ) )
                        {
                            //ignore
                        }
                 * */
                if (form_key.StartsWith("top_", StringComparison.OrdinalIgnoreCase)
                        || form_key.StartsWith("bottom_", StringComparison.OrdinalIgnoreCase)
                    //MAC cannot be calculated over itself
                        || form_key.Equals("mac", StringComparison.OrdinalIgnoreCase)) continue;
                data_string.Append(request.Form[form_key]);
            }
            log.Debug("data_string: " + data_string);
            // 2. append the shared secret 
            data_string.Append(s_secret);
            // 3. Turn this string into a string of utf-8 bytes 
            byte[] data_buffer = System.Text.Encoding.UTF8.GetBytes(data_string.ToString());
            // 4. Create a message digest from these bytes 
            System.Security.Cryptography.MD5 md = new System.Security.Cryptography.MD5CryptoServiceProvider();
            byte[] md5_hash = md.ComputeHash(data_buffer);
            // 5. Convert the resulting byte array into a base64 encoded string
            String md5_hash_64 = System.Convert.ToBase64String(md5_hash);
            log.Debug("md5_hash_64: " + md5_hash_64);
            //relace operations below are copied from proxy\java\src\com\blackboard\test\proxy\ProxyUtil.java  generateMac()
            md5_hash_64.Replace("\r", "");
            md5_hash_64.Replace("\n", "");
            log.Debug("md5_hash_64: " + md5_hash_64);

            String bb_mac = request.Form[PtaServerConstants.MAC_KEY];
            log.Debug("bb_mac: " + bb_mac);

            //compare received and calculated MACs
            if (!bb_mac.Equals(md5_hash_64)) {
                throw new PtaException("MAC mismatch. md5_hash_64: " + md5_hash_64 + "; bb_mac: " + bb_mac);
            }

            //check that timestamp is expected timeframe
            String bb_ts = request.Form[PtaServerConstants.TIMESTAMP_KEY];
            long bb_millis = Int64.Parse(bb_ts);
            log.Debug("bb_millis: " + bb_millis);

            long mac_life_ms = Int64.Parse(WebConfigurationManager.AppSettings["MacLifetimeInMinutes"]);
            log.Debug("mac_life_ms: " + mac_life_ms);
            long mac_life_millis = mac_life_ms * 1000 * 60;

            //http://bytes.com/topic/c-sharp/answers/557734-java-system-currenttimemillis-equivalent 
            //- about java System.currentTimeMillis() in .net
            DateTime utc_now = DateTime.UtcNow;
            log.Debug("utc_now: " + utc_now);
            DateTime base_time = new DateTime(1970, 1, 1, 0, 0, 0);
            log.Debug("base_time: " + base_time);
            long proxy_millis = (utc_now - base_time).Ticks / 10000;
            log.Debug("proxy_millis: " + proxy_millis);

            if (Math.Abs(proxy_millis - bb_millis) > mac_life_millis) { //added abs() - not to be much earlier too. Assuming that earlier timestamp may be possible due to non-synchronized system time on BB and proxy server
                throw new PtaException("Timestamp is out of max request delay. proxy_millis(secs): " + proxy_millis + "(" + proxy_millis / 1000 + "); bb_millis(secs): " + bb_millis + "(" + bb_millis / 1000 + "); mac_life_ms(secs): " + mac_life_millis + "(" + mac_life_millis / 1000 + ")");
            }

            String nonce = request.Form[PtaServerConstants.NONCE_KEY];
            if (nonce == null) {
                throw new PtaException("Invalid null Nonce");
            }
            if (RequestValidator.knownNonces.Contains(nonce)) {
                throw new PtaException("Invalid duplicated Nonce. nonce: " + nonce);
            }
            int nonce_cleanup_count = Int16.Parse(WebConfigurationManager.AppSettings["NonceCleanupCount"]);
            lock (typeof(RequestValidator)) {
                knownNonces.Add(nonce, proxy_millis);
                nonceCleanupCounter++;

                if (nonceCleanupCounter > nonce_cleanup_count) { 
                       //!! test/debug
                    Hashtable non_expired_nonces = new Hashtable();
                    foreach (DictionaryEntry nonce_de in RequestValidator.knownNonces) {
                        long nonce_millis = (long)nonce_de.Value;
                        if (nonce_millis >= proxy_millis - mac_life_millis) non_expired_nonces.Add (nonce_de.Key, nonce_millis);
                    }
                    knownNonces = non_expired_nonces;
                    nonceCleanupCounter = 0;
                }
            }

        }
    }
}
