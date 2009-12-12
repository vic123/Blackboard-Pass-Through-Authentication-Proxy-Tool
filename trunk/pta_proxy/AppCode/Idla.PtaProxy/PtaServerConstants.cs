using System;
namespace Idla.PtaProxy
{
	
	/*
	* Refer to LICENSE_for_samples.txt for license details relating to this file
	*/
    /// <remarks>
    /// Class is just left as it is after conversion of proxy\java\src\com\blackboard\test\proxy\ProxyServerConstants.java 
    /// of java sample.
    /// </remarks>
    public class PtaServerConstants
	{
		public static System.String DataDir
		{
			get
			{
				return dataDir;
			}
			
			set
			{
				dataDir = value;
			}
			
		}
		public static System.String Axis2Home
		{
			get
			{
				return axis2Home;
			}
			
			set
			{
				axis2Home = value;
			}
			
		}
		public static System.String Axis2Xml
		{
			get
			{
				return axis2Xml;
			}
			
			set
			{
				axis2Xml = value;
			}
			
		}
		private static System.String dataDir = "d:\\proxy\\data"; // Default in case web.xml isn't substituted
		private static System.String axis2Home = "d:\\axis2\\axis2-1.3"; // Default in case web.xml isn't substituted
		private static System.String axis2Xml = "D:\\proxy\\apache-tomcat-6.0.14\\webapps\\ws.sample.proxyserver\\WEB-INF\\conf\\axis2.client.xml"; // Default in case web.xml isn't substituted
		
		public const System.String ATR_PERCOURSE_DATA = "percoursedata";
		public const System.String ATR_PERUSER_DATA = "peruserdata";
		public const System.String ATR_PERGROUP_DATA = "pergroupdata";
		
		public const System.String EXACT_TYPE = "exact";
		
		public const System.String ACTION_KEY = "action";
		public const System.String LMS_SESSION_KEY = "lms_session";
		public const System.String GROUP_ID_KEY = "group_id";
		public const System.String COURSE_ID_KEY = "course_id";
		public const System.String FOLDER_ID_KEY = "folder_id";
		public const System.String CONTENT_ID_KEY = "content_id";
		public const System.String EXTERNAL_CONTENT_ID_KEY = "external_content_id";
		public const System.String CONTENT_NAME_KEY = "content_name";
		public const System.String CONTENT_DESCRIPTION_KEY = "content_description";
		public const System.String EXTERNAL_ID_KEY = "external_id";
		public const System.String FULLRETURNURL_KEY = "fullreturnurl";
		public const System.String BASETYPE_KEY = "basetype";
		public const System.String CONTENT_EXT_DATA_KEY = "external_data";
		public const System.String CREATE_GRADEBOOK_KEY = "create_gradebook";
		public const System.String CREATE_EXTRAS_KEY = "create_extras";
		public const System.String OURCONTENT_KEY = "ourcontent";
		public const System.String GENERIC_KEY = "generic";
		
		public const System.String TCPROFILEURL_KEY = "tcprofileurl";
		public const System.String HTTPURL_KEY = "httpurl";
		public const System.String DIGEST_ALGORITHMS_KEY = "digestalgorithms";
		public const System.String CSSURL_KEY = "cssUrl";
		public const System.String STR_XML_TOOL_CONSUMER_PROFILE = "tool-consumer-profile";
		
		public const System.String OURGUID_KEY = "ourguid";
		public const System.String RETURNURL_KEY = "returnurl";
		public const System.String LOCALE_KEY = "locale";
		public const System.String DIRECTION_KEY = "direction";
		public const System.String TIMESTAMP_KEY = "timestamp";
		public const System.String TICKET_KEY = "ticket";
		public const System.String NONCE_KEY = "nonce";
		public const System.String TCBASEURL_KEY = "tcbaseurl";
		public const System.String USERID_KEY = "userid";
		public const System.String LTI_ROLE_KEY = "role";
		public const System.String TC_ROLE_KEY = "tcrole";
		public const System.String MAC_KEY = "mac";
		public const System.String TARGET_USERID_KEY = "target_userid";
		public const System.String REGISTRATION_PASSWORD_KEY = "regpass";
		public const System.String STATE_KEY = "state";
		
		public const System.String MD5_DIGEST_ALGORITHM = "MD5";
		//UPGRADE_NOTE: Final was removed from the declaration of 'DEFAULT_DIGEST_ALGORITHM '. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1003'"
		public static readonly System.String DEFAULT_DIGEST_ALGORITHM = MD5_DIGEST_ALGORITHM;
		public const System.String SHA_DIGEST_ALGORITHM = "SHA";
		
		// This prefix will be added to each tool setting we save.  This shouldn't be necessary
		// in a 'real' proxy server, but since we're just randomly setting/retrieving settings
		// we don't really "know" what settings we're expecting so I'm using this to
		// distinguish our settings from any other arguments.
        public const System.String SETTING_PREFIX = "samplesetting";

        public const System.String SIMPLE_SUCCESS_RESPONSE = "<result>SUCCESS<result>";
        public const System.String SIMPLE_ERROR_RESPONSE = "<result>ERROR<result>";

		
		public const int SYSTEM_LEVEL_SETTING = 0;
		public const int COURSE_LEVEL_SETTING = 1;
		public const int CONTENT_LEVEL_SETTING = 2;
		public const System.String SYSTEM_SETTING_MIDFIX = "_system_";
		public const System.String COURSE_SETTING_MIDFIX = "_course_";
		public const System.String CONTENT_SETTING_MIDFIX = "_content_";
	}
}