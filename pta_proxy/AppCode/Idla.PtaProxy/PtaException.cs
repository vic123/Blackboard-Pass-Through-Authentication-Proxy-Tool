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

namespace Idla.PtaProxy
{
    /// <remarks>
    /// Just customly tagged by its type exception used for throwing of application logic initiated error conditions.
    /// </remarks>
    public class PtaException : System.ApplicationException
    {
        public PtaException(string message):base(message){}
    }

    /// <remarks>
    /// A way to provide actual error reasons to exception handler - RegisterToolResultVO.
    /// </remarks>
    public class PtaRegisterToolException : System.ApplicationException
    {
        public RegisterToolResultVO registerToolResult;
        public PtaRegisterToolException(string message, RegisterToolResultVO res) : base(message) {
            registerToolResult = res;
        }
    }

    
}
