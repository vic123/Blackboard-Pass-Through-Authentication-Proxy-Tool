Contents:
I. Integration with authenticated application
II. Setup and configuration
III. Blackboard server and Link(s) definition
IV. Link redirection processing definition
V. Notes on possible code improvements


I. Integration with authenticated application:
-------------------------------------------------------
1. 
Copy all files from pta_proxy\bin to application's bin folder.

2. 
log4net configuration file is bin\pta_proxy.log4net and placed in bin folder altogether with other files that have to be copied. Default output configuration is App_Data\pta_proxy.log.
However if hosting application already uses log4net then its configuration file may be better to be used. log4net assembly directive is located in pta_proxy\AppCode\Idla.PtaProxy\PtaUtil.cs and has to be changed appropriately in order for relying on application’s default log4net configuration. 

3.
Authorization in pta_proxy\Admin\Web.config has to be configured according to supported by hosting application mechanisms. Proxy by itself has implemented simple authentication mechanism with pta_proxy\AppCode\XmlMembershipProvider.cs and pta_proxy\App_Data\users.xml.
"authentication" and "membership defaultProvider" from pta_proxy\Web.config have to be commented out and pta_proxy\Admin\Web.config to be edited to contain user names permitted to perform PtaProxy administration.

4.
Error handling. pta_proxy\Web.config defines GlobalErrorHandler that however does not seem to be loaded when pta_proxy is just a folder of hosting web application. Registration errors are logged and then rethrown. Errors occurred upon user redirection are logged but not rethrown. Custom error handler can be defined per Blackboard instance in DB, in BBoard.OnErrorServerTransfer.

5.
WSE requires additional runtime libraries installation, setup can be downloaded from here - http://www.microsoft.com/DownLoads/details.aspx?familyid=018A09FD-3A74-43C5-8EC1-8D789091255D&displaylang=en (Microsoft WSE 3.0.msi)


II. Setup and configuration
---------------------------------
1. 
Use VS2008 "Publish" to build project and separate files required for deployment. Copy pta_proxy to application subfolder in case of integrated deployment or its own application path under IIS. This path to has to be specified in pta_proxy\Web.config appsettings according to point 4 of this section.

2. 
Create MSSQL server database named PtaProxy and run pta_proxy\sql\schema.sql and pta_proxy\sql\data.sql scripts against it. 
Create Windows authentication login for ASPNET account in SQL Server. Add login mapping to PtaProxy database with db_owner permissions.
OR, configure native SQL Server authentication in connectionString (next point).

3. 
Root pta_proxy\Web.config connectionStrings - has to point to PtaProxy database containig registration and redirection configuration settings.
<connectionStrings>
    <add
    name="PtaProxySQLConnectionString"
    connectionString="Data Source=localhost\SQL2K5;Initial Catalog=PtaProxy;Integrated Security=True"
    providerName="System.Data.SqlClient"
    />
</connectionStrings>

4.
Root pta_proxy\Web.config appsettings:
  <appSettings>
<!-- Proxy URL values are provided along with registration at Blackboard - this is address of the proxy that will be used by blackboard for accessing of proxy server --> 
    <add key="PtaProxyHttpURL" value="http://PtaProxy.com:4436"/>
    <add key="PtaProxyHttpsURL" value="https://PtaProxy.com:4438"/>
    <add key="PtaProxyServerToServerURL" value="https://PtaProxy.com:4438"/>
<!-- This is the folder where proxy resides in full application path. May contain or not starting/ending slashes. Introduced assuming that RegisterTool may be called from another than pta_proxy\Admin\Register.aspx location (i.e. in case if application performs some custom processing of registration).
    <add key="PtaProxyWebFolder" value="pta_proxy"/>
<!-- This is a maximum delay between Blackboard timestamp and calculated currentTimeMillis on proxy server -->
    <add key="MacLifetimeInMinutes" value="10"/>
<!--nonces are collected in application wide internal structure-->
    <!--cycle on cleaning of nonces is initiated upon each NonceCleanupCount invocation in RequestValidator.Validate-->
    <!--i.e. after processing of NonceCleanupCount validations, structure containing nonces is cleaned up from those with expired timestamp-->
    <add key="NonceCleanupCount" value="100"/>
  </appSettings>

5. 
Root pta_proxy\Web.config httpHandlers section.
Currently working for passthrough authentication http handler verb is "pta/*". The only extension it handles consistently is .aspx. Tests under VS2008 handled any extension at all, but in IIS experiment with .ascx which is handled by ASP.NET and was added as explicit rule in httpHandlers resulted with "The page cannot be found". Suggested form of links structure that should simplify their recognition and parsing is etiher with specific link name pattern or with "path info" (some_link.aspx/this_part_of_url_is_path_info).

Under VS2008 HttpHandler got activated on following URLs, for IIS ".Dot" has to be replaced with ".aspx" :
http://localhost:4435/pta_proxy/pta/AnyNameWithoutDot
http://localhost:4435/pta_proxy/pta/AnyNameWith.Dot
http://localhost:4435/pta_proxy/pta/AnyNameWith.Dot/
http://localhost:4435/pta_proxy/pta/AnyNameWith.Dot/AnyNameWithoutDot
http://localhost:4435/pta_proxy/pta/AnyNameWith.Dot/AnyNameWithoutDot/AnyNameWithoutDot
http://localhost:4435/pta_proxy/pta/AnyNameWith.Dot/AnyNameWithoutDot/AnyNameWithoutDot/ and so on, and this remaining part after AnyNameWith.Dot is interpreted as HttpRequest.PathInfo property.
and is not activated for following ones:
http://localhost:4435/pta_proxy/pta/AnyNameWithoutDot/
http://localhost:4435/pta_proxy/pta/AnyNameWithoutDot/AnyNameWithoutDot
http://localhost:4435/pta_proxy/pta/AnyNameWithoutDot/AnyNameWith.Dot
http://localhost:4435/pta_proxy/pta/AnyNameWith.Dot/AnyNameWith.Dot
Link path specified in BBMenuLink.BBLinkPath DB table/field starts from "AnyName..." without preceding slash.

File Types Managed by ASP.NET - http://msdn.microsoft.com/en-us/library/2wawkw1c.aspx
http://www.codeproject.com/KB/web-image/thumbnailer.aspx :
MSDN defines an HTTP handler as: "a process (frequently referred to as the 'endpoint') that runs in response to a request made to an ASP.NET Web application. The most common handler is an ASP.NET page handler that processes .aspx files. When users request an .aspx file, the request is processed by the page via the page handler." Other common HTTP handlers are the Web service handler (*.asmx extension), ASP.NET user control handler (*.ascx extension), and the Trace handler (trace.axd).
http://msdn.microsoft.com/en-us/library/ms972953.aspx :
When customizing the ASP.NET engine's mapping of file extensions to HTTP handlers, it is important to understand that the file extensions being set in the machine.config or Web.config files must be mapped to the aspnet_isapi.dll in the IIS metabase. In order for the ASP.NET engine to be able to route a request to the proper HTTP handler, it must first receive the request from IIS. IIS will route the request to the ASP.NET engine only if the requested file's extension is mapped to the aspnet_isapi.dll file in the IIS metabase.


III. Blackboard server and Link(s) definition
-------------------------------------------------------
Settings related to Blackboard server identification and defined links are stored in PtaProxy.BBoard and PtaProxy.BBMenuLink database tables.
Clarification of BBoard table fields is provided directly on Register.aspx page.
BBMenuLink and Param data can be edited in XML form within Register.aspx page and “Remote Configure” action.

PtaProxy.BBMenuLink defines links that will be registered in Blackboard interface. 
Fields:
 - BBLinkPath - the rest of the URL after application path/proxy location/pta/. Should not contain starting "/". Should follow conventions discussed at II.5. Supplied URL query string parameters are ignored and cut off by Blackboard. Form parameters that can be hardcoded for Actions (i.e. BB callbacks) are ignored for menu links according to test results. The only way to distinguish different links is with their virtual filename or path info.
 - BBLinkType - 'course_tool', 'system_tool', 'user_tool', 'tool' or 'group_tool', check is encoded in DB schema. These are constants accepted by Blackboard that define link location in Blackboard interface. user_tool link is used in the Tools module on your My Institution module. user_tool must be manually added by the Blackboard Learn System Administrator on System Admin/Communities/Tabs and Modules/Tool Panel. See http://www.edugarage.com/display/BBDN/Link+Type for more details. 
 - BBLinkName - this is link caption, i.e. text shown in Blackboard GIU as link name. Localization settings are not supported. Tested that cyrrilic link for example shows up on test Blackboard server as "????" and responded to clicks (?? not sure), so probably localization is not so important as long as probably any specific language characters can be used if they are supported by target system (!! need more testing - this is just hypothesis by now).
 - BBLinkIconPath - 
 - Enabled - Y/N, allows to preserve some pre-parameterized link templates without their actual registration at Blackboard
 - Comment - any comments on what should be expected passthrough authentication system response on receiving of request on URL defined through BBLinkPath.

Specifying of data described above is enough for processing of proxy server registration. List links defined once cannot be modified without proxy re-registration. However processing rules for any link (Param table) can be defined/modified at any time (section IV. Link redirection processing definition)



IV. Link redirection processing definition
---------------------------------------------------
1. 
Principal schema of processing

                                     ___{ParamTag)_>_
                                    |                |<----RegExpOperation
    PtaProxy.Param.ParamInputType   ^                |
    in order of ParamOrderCode      |                |param_tag_value, 
-->-------------------------------->|                |stored in code
 ^                                                   |
 |  Through  ParamInputTemplate                      |----->ParamOutputType
  ------------------------------------------<--------


2.
Textual description.
Process cycles through records in Param table and (at least) assigns value to {ParamTag}. Value may come for example from HttpRequest supplied data or from already processed {Tag}. Processed {Tags} may be concatenated in ParamInputTemplate field along with any "hard-configured" string values (this concatenation is assumed "part" of input processing... sense of input vs. operation is not very cleaned by this time, Param table is just quick and dirty approach to provide storage for configuration data). After that value can be processed with match or replace regular expression, i.e. to be modified with regexp capabilities. Regular expression looks to me like something quite handy for such kind of processing and now it is possible it to be applied on any round. 
Currently all constraints are named as close as possible to their real in-code sources. So, that to cause as minimum confusion as possible for ASP.NET person that needs to define transformation of input parameters into already pre-defined URL of application.
After getting of value from somewhere (see below list of currently possible options) and optional applying of regular expression to the result, it (also optionally) may be assigned to some output channel (i.e. response, or session, or some cookie). And in any case this value remains assigned to {ParamTag} that may be reused later.

3. 
Definitions of other Param table fields that were not covered above.
 - ParamKey - defines key value when accessing query string/post or session parameters, etc., see definition of each input/output type for details
 - RegExp - either match or replace regular expression that has to be applied to value of the {Tag} after its (value) obtaing from input source. 
 - RegExpOperation - i.e. 'Assign', 'REMatch' or 'REReplace'. 'Assign' is just direct assignment of ParamInputTemplate concatenation as value.
 - RegExpReplaceString - replacement string for 'REReplace' operation.
 - RegExpMatchGroup - matching group for 'REMatch' operation
 - RegExpMatchCapture - capture number for 'REMatch' operation (i.e. both according to .net API terminology and default processing)
 - SProcName - name of stored procedure to be called in case of 'SProcCall' Param.ParamInputType


* ParamInputType values:

** Param.InputTemplate
Place where tags may be combined/concatenated with string data.

** ProxyLinkPath
proxy link path - path that is stored in BBMenuLink.BBLinkPath, path of the URL after pta_proxy/pta/. It is extracted from URL and used for identification of BBMenuLink record. Does not include starting "/"

** ProxyLinkType
e.g. ('course_tool', 'system_tool', 'user_tool', 'tool', 'group_tool') - http://www.edugarage.com/display/BBDN/Link+Type

** ProxyPath
e.g. like application path to the proxy, i.e. including proxy subfolder. Contains starting and ending "/", may be changed to become compatible with ApplicationPath

** Request.ApplicationPath
ApplicationPath, if only "/" - then empty string (because otherwise returns "/pta_proxy", requires if logic during concatenation) 

** Request.FilePath

** Request.Form
ParamKey should contain name of the parameter

** Request.InputStream

** Request.QueryString
??BB seems not to pass QS parameters, just custs them out if included in link path

** Request.PathInfo

** Request.Url.ToString
Request.Url.ToString()

** SProcCall	
Calls stored procedure. InputTemplate is list of Tags, SProc parameters must have same names as Tags (without {}). And single output parameter with name of the ParamKey value. All parameters have to be of String-compatible type.

** BBWSCall
??May be future possibility to get additional information from Blackboard via WS calls.

** FormsAuthentication.SetAuthCookie
Sets FormsAuthentication.SetAuthCookie (i.e. user name) value of the tag

** Context.Items
Can be used as a storage upon Server.Transfer. 
context.Items[param_key] = param_value;

** Context.Session
ParamKey should contain name of the parameter. Could not get Session parameters preserved when tested under VS2008 with Status 301/302/307, IIS seems to behave better.

** Response.AppendHeader
context.Response.AppendHeader(param_key, param_value);

** Response.Output
context.Response.Output.Write(param_value);

** Response.Status
context.Response.Status = param_value;

** Server.Transfer
This is most universal and allowing any customization way - all post, etc. parameters are preserved. However has restrictions and specifics - http://www.codeproject.com/KB/aspnet/ASP_NETRedirectAndPost.aspx

** None


* RegExpOperation

** Assign
Assigns concatenation of InputTemplate tags

** REMatch
Applies RegExp to concatenation of InputTemplate Tags. Assigns to tag value of specified RegExpMatchCapture in RegExpMatchGroup 

** REReplace
Replaces RegExp matches in InputTemplate with RegExpReplaceString

Please research sample links and tests on their processing (test-cases.txt, “III. Sample links processing in IIS”) for more details.


V. Notes on possible code improvements
--------------------------------------------------
1. 
Implement "tool-provision" and "bundle" actions

2. 
Port WSE to more modern WCF, links describing how it is possible to implement WCF-Blackboard connectivity without SSL requirement:
http://webservices20.blogspot.com/2008/11/introducing-wcf-clearusernamebinding.html
http://webservices20.blogspot.com/2008/11/how-to-use-clear-usernamepassword-with.html
http://www.devproconnections.com/article/microsoft-net-framework/custom-bindings-part-i.aspx

3. 
Probably capturing of redirection links with HttpModule instead of HttpHandler will be more in place and allow construction of more complex links without any restrictions to their form.

4. 
Analyze http://bbserver.com/webapps/ws/wsadmin/tcprofile before registration.

5. 
Move PtaProxy configuration settings from database to XML file. 

