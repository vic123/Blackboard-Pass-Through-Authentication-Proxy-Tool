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
using Microsoft.ApplicationBlocks.Data;
using System.Text.RegularExpressions;
using System.Web.Configuration;


namespace Idla.PtaProxy
{
        /// <remarks>
        /// Implementation of accessing of ContextWS.register() method.
        /// Main code logic is in building of XML structure describing various proxy specifics and communication points.
        /// http://www.edugarage.com/display/BBDN/How+to+Build+Proxy+Tools?decorator=printable
        /// XML is formed from hard-coded templates with {TAGS} that later on are replaced with configurable proxy settings.
        /// See pta_proxy\readme.txt, II. Setup and configuration, 5. Root pta_proxy\Web.config httpHandlers section.
        /// and III. Blackboard server and Link(s) definition for more details.
        /// </remarks>
        public class RegisterToolWSCall : WSCall
        {
            static readonly String REG_XML = @"<?xml version=""1.0"" encoding=""UTF-8""?>
<tool-profile ltiVersion=""2.0-July08"" xmlns:locale=""http://www.ims.org/lti/localization"">
<vendor>
  <code>IDLA</code>
  <name>Idaho Digital Learning Academy</name> 
  <description></description> 
  <url>http://www.idahodigitallearning.org</url>
  <contact><email></email></contact> 
</vendor>
<tool-info>
  <code>{PROXY_CODE}</code>
  <name>Pass Through Authentication Proxy Tool</name> <!-- name not used in AS -->
  <version>1</version>
  <description>Pass Through Authentication Proxy Tool (BB login -> another application)</description>
</tool-info>
<tool-instance>
  <!-- Dynamic information that changes based on each installation of the tool -->
  <base-urls>
    {BASE_URLS}
  </base-urls>
  <contact><email></email></contact>
  <security-profile>
    <digest-algorithm>MD5</digest-algorithm>
  </security-profile>
</tool-instance>
<required-webservices>
    <tool-login>
        <service name=""Context.WS"">
          <operation>logout</operation>
        </service>
        <service name=""Util.WS"">
          <operation>saveSetting</operation>
          <operation>deleteSetting</operation>
        </service>
        <service name=""Content.WS"">
          <operation>saveContent</operation>
        </service>
    </tool-login>
</required-webservices>
<http-actions>
{HTTP_ACTIONS}
</http-actions>
<links>
{MENU_LINKS}
</links>
</tool-profile>";
/*
<http-actions>
    <action type=""remove""
        path=""/Actions/StateChange.aspx""/>
    <action type=""state-change""
        path=""/Actions/StateChange.aspx""/>
</http-actions>
*/
//2Do - add locale handling
            //<name locale:key=""course_tool.language.key"">ProxyToolsTestASP_01 CCPT Tool</name>
/*            static readonly String HTTP_ACTIONS_XML = @"
        <action type=""config""
            path=""{PROXY_WEB_FOLDER}Actions/Config.aspx""/>
        <action type=""ping""
            path=""{PROXY_WEB_FOLDER}Actions/Ping.aspx""/>
        <action type=""remove""
            path=""{PROXY_WEB_FOLDER}Actions/Remove.aspx""/>
        <action type=""reregister""
            path=""{PROXY_WEB_FOLDER}Actions/ReRegister.aspx""/>
        <action type=""state-change""
            path=""{PROXY_WEB_FOLDER}Actions/StateChange.aspx""/>";*/
            static readonly String HTTP_ACTIONS_XML = @"
        <action type=""remove""
            path=""{PROXY_WEB_FOLDER}Actions/Remove.aspx""/>
        <action type=""config""
            path=""{PROXY_WEB_FOLDER}Actions/Config.aspx""/>
        <action type=""state-change""
            path=""{PROXY_WEB_FOLDER}Actions/StateChange.aspx""/>
        <action type=""reregister""
            path=""{PROXY_WEB_FOLDER}Actions/ReRegister.aspx""/>
        <action type=""ping""
            path=""{PROXY_WEB_FOLDER}Actions/Ping.aspx""/>";

            static readonly String MENU_LINK_XML = @"    <menu-link>
      <category-choice>
        <category>TBD - not defined by LTI yet</category>
        <category platform=""blackboard"">{MENU_LINK_TYPE}</category> 
      </category-choice>
        {LINK_NAME}
      <http-actions>
            <action type=""menu-view"" path=""{LINK_PATH}""/>
            <param name=""someparam"" fixed=""testvalue""/>
      </http-actions>
      <description></description>
      <icons>
        <icon >
        </icon>
        {LINK_ICON}
      </icons>
    </menu-link>
";

            String toolRegistrationPassword;
            bool IsReregister;
            String initialSharedSecret;


            public RegisterToolWSCall(String bbCode, String toolRegistrationPassword, bool IsReregister)
                : base(bbCode)
            {
                this.toolRegistrationPassword = toolRegistrationPassword;
                this.IsReregister = IsReregister;
            }

            /// <summary>
            /// Made as static for simplicity of registration XML preview (pta_proxy\Admin\Register.aspx).
            /// </summary>
            static public String GetRegistrationXML(String bbCode)
            {
                log4net.ILog log = PtaUtil.getLog4netLogger(typeof(RegisterToolWSCall).FullName + ".getRegistrationXML: ");
                String desc;
                //method is static because can be called for preview, i.e. WSCall.wsWrapper.GetBBoardDR() is unavailvable 
                SqlDataReader bboardDR = PtaUtil.GetBBoardDataReaderByCode(bbCode);
                try {
                    if ("Y".Equals(PtaUtil.GetDBReaderStringField(bboardDR, "IsRegistered"))) {
                        //??probably some logic on preliminary handling of this can be placed here, 
                        //but capability to reregister even with this flag set is necessary 
                        //because it is possible proxy to be unregistered at Blackboard but not updated at proxy
                        //(if proxy is not accessibel at the moment of unregistration),
                        //i.e. IsRegistered (as well as ProxyState) can be out of sync with Blackboard
                    }
                    String proxy_http_url = WebConfigurationManager.AppSettings["PtaProxyHttpUrl"];
                    String proxy_https_url = WebConfigurationManager.AppSettings["PtaProxyHttpsURL"];
                    String proxy_s2s_url = WebConfigurationManager.AppSettings["PtaProxyServerToServerURL"];
                    String base_urls = "";
                    if ("Y".Equals(PtaUtil.GetDBReaderStringField(bboardDR, "RegisterHttpsURL"))) base_urls += "<base-url type=\"https\">" + proxy_https_url + "</base-url>\n";
                    if ("Y".Equals(PtaUtil.GetDBReaderStringField(bboardDR, "RegisterHttpURL"))) base_urls += "<base-url type=\"http\">" + proxy_http_url + "</base-url>\n";
                    if ("Y".Equals(PtaUtil.GetDBReaderStringField(bboardDR, "RegisterS2SURL"))) base_urls += "<base-url type=\"server-to-server\">" + proxy_s2s_url + "</base-url>\n";
                    desc = Regex.Replace(REG_XML, "{BASE_URLS}", base_urls);
                    if ("Y".Equals(PtaUtil.GetDBReaderStringField(bboardDR, "RegisterActions"))) {
                        desc = Regex.Replace(desc, "{HTTP_ACTIONS}", HTTP_ACTIONS_XML);
                    }
                    String proxy_code = PtaUtil.GetDBReaderStringField(bboardDR, "BBoardProxyCode");
                    desc = Regex.Replace(desc, "{PROXY_CODE}", proxy_code);
                    

                    //String desc = Regex.Replace(REG_XML, "{BASE_HTTP_URL}", proxy_http_url);
                    //desc = Regex.Replace(desc, "{BASE_HTTPS_URL}", proxy_https_url);
                    desc = Regex.Replace(desc, "{PROXY_WEB_FOLDER}", PtaUtil.GetProxyWebFolder());


                    String sql = "SELECT BBLinkPath, BBLinkType, BBLinkName, BBLinkIconPath FROM BBMenuLink WHERE BBoardCode = @BBoardCode AND Enabled = 'Y'";
                    log.Info("sql: " + sql);
                    SqlConnection con = PtaUtil.GetSqlConnection();
                    System.Data.SqlClient.SqlParameter param
                            = new System.Data.SqlClient.SqlParameter("BBoardCode", PtaUtil.GetDBReaderStringField(bboardDR, "BBoardCode"));
                    SqlDataReader drMenuLink =
                        Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteReader(con, System.Data.CommandType.Text,
                                sql, param);
                    String xml_links = "";
                    while (drMenuLink.Read()) {
                        String xml_part = MENU_LINK_XML;
                        String link_path = PtaUtil.GetProxyWebFolder() + PtaUtil.PTA_SUBFOLDER + PtaUtil.GetDBReaderStringField(drMenuLink, "BBLinkPath");
                        String link_type = PtaUtil.GetDBReaderStringField(drMenuLink, "BBLinkType");
                        String link_name = PtaUtil.GetDBReaderStringField(drMenuLink, "BBLinkName");
                        String icon_path = PtaUtil.GetDBReaderStringField(drMenuLink, "BBLinkIconPath");
                        xml_part = Regex.Replace(xml_part, "{LINK_PATH}", link_path);
                        xml_part = Regex.Replace(xml_part, "{MENU_LINK_TYPE}", link_type);
                        xml_part = Regex.Replace(xml_part, "{LINK_NAME}", "<name locale:key=\"" + link_name + "\"></name>");
                        if (icon_path != null) {
                            xml_part = Regex.Replace(xml_part, "{LINK_ICON}", "<icon platform=\"blackboard\" style=\"listitem\" >" + icon_path + "</icon>");
                        } else xml_part = Regex.Replace(xml_part, "{LINK_ICON}", "");
                        xml_links = xml_links + xml_part;
                    }
                    desc = Regex.Replace(desc, "{MENU_LINKS}", xml_links);
                } finally {
                    PtaUtil.CloseDataReader(bboardDR);
                }
                return desc;
            }

            /// <summary>
            /// Entry point of registration processing, called by static DoCall() of base class.
            /// Builds XML, generates SharedSecret, calls ContextWS register() method and updates database upon success.
            /// </summary>
            public override void Call()
            {
                log4net.ILog log = PtaUtil.getLog4netLogger(typeof(RegisterToolWSCall).FullName + ".Call(): "); 
                String desc = GetRegistrationXML(wsWrapper.GetBBHostCode());
                if (!IsReregister) {
                    initialSharedSecret = System.Guid.NewGuid().ToString();
                    initialSharedSecret = Regex.Replace(initialSharedSecret, "-", "");
                    initialSharedSecret = Regex.Replace(initialSharedSecret, ":", "_");
                } else initialSharedSecret = PtaUtil.GetDBReaderStringField(wsWrapper.GetBBoardDR(), "SharedSecret");


                RegisterToolResultVO res;
                string[] requiredToolMethods = null;
                string[] requiredTicketMethods = null;
                //commented for security reasons
                //log.Debug("toolRegistrationPassword: " + toolRegistrationPassword);
                log.Info("desc: " + desc);
                //commented for security reasons
                //log.Debug("initialSharedSecret: " + initialSharedSecret);
                String proxy_code = PtaUtil.GetDBReaderStringField(wsWrapper.GetBBoardDR(), "BBoardProxyCode");

                res = wsWrapper.GetContextWS().registerTool("IDLA", proxy_code, toolRegistrationPassword, desc, initialSharedSecret, requiredToolMethods, requiredTicketMethods);
                String str = res.ToString();
                str = res.proxyToolGuid;
                if (res.status) {
                    String sql = "UPDATE BBoard SET IsRegistered = @IsRegistered, SharedSecret = @SharedSecret, GUID = @GUID, ProxyState='Inactive'  WHERE BBoardCode = @BBoardCode";
                    log.Info("sql: " + sql);
                    SqlParameter[] update_bb_params = new SqlParameter[] {   
                     new SqlParameter("@IsRegistered", "Y") ,   
                     new SqlParameter("@SharedSecret", initialSharedSecret),
                     new SqlParameter("@GUID", res.proxyToolGuid),
                     new SqlParameter("@BBoardCode", wsWrapper.GetBBHostCode())
                    };
                    log.Info("res.proxyToolGuid: " + res.proxyToolGuid);
                    log.Info("ws_wrapper.bbHostCode: " + wsWrapper.GetBBHostCode());
                    Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteNonQuery(PtaUtil.GetSqlConnection(), 
                                                                System.Data.CommandType.Text, sql, update_bb_params);
                } else {
                    String failure_errors = "";
                    for (int i = 0; i < res.failureErrors.Count(); i++)
                        failure_errors += "BB Falire Err.#" + i + ": " + res.failureErrors[i] + ";\r\n ";
                    throw new PtaRegisterToolException(wsWrapper.GetContextWS().GetType().FullName + ".registerTool() failed with following error(s): \r\n " + failure_errors, res);
                }
            }
        }
}

