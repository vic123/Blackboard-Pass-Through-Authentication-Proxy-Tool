using System;
using System.Web;
using System.Collections.Specialized;
using System.Reflection;
using System.Diagnostics;

namespace Idla.PtaProxy
{
    
    /// <remarks>
    /// Configured in httpModules of pta_proxy\Web.config, displays environment and exception info, nothing fancy.
    /// It is desired to be displayed for authenticated users only upon errors in pta_proxy\Admin\Register.aspx, 
    /// but also by default shows up upon accessing of pta/ subfolder links that are not handled by 
    /// PtaHttpHandler. This case is fixed inside OnError() depending on Request.FilePath and 
    /// for pta/ subfolder error is logged and forwarded to default error handler.
    /// </remarks>
	public class GlobalErrorHandler : IHttpModule
	{
        public void Init(HttpApplication app)
		{
            log4net.ILog log = PtaUtil.getLog4netLogger(this.GetType().FullName + ".Init(): ");
            log.Info("Instantiated");
			app.Error += new System.EventHandler (OnError);
		}

        public void OnError(object obj, EventArgs args)
		{
			// At this point we have information about the error
            log4net.ILog log = PtaUtil.getLog4netLogger(this.GetType().FullName + ".OnError(): ");

            log.Debug("Entered GlobalErrorHandler.OnError");
			HttpContext ctx = HttpContext.Current;
			HttpResponse response = ctx.Response;
			HttpRequest request = ctx.Request;

            Exception exception = ctx.Server.GetLastError();
            String ex_html = PtaUtil.GetExceptionAndEnvHtml(exception);

            String url = HttpContext.Current.Request.Url.ToString();
            log.Debug("HttpContext.Current.Request.FilePath: " + HttpContext.Current.Request.FilePath);
            log.Debug("HttpContext.Current.Request.ApplicationPath: " + HttpContext.Current.Request.ApplicationPath);
            //!! test when proxy is not root
            if (HttpContext.Current.Request.FilePath.StartsWith(
                PtaUtil.GetProxyWebFolder() + PtaUtil.PTA_SUBFOLDER)) {
                log.Info(ex_html);
            } else {
                response.Write(ex_html);
                ctx.Server.ClearError();
                // --------------------------------------------------
                // To let the page finish running we clear the error
                // --------------------------------------------------
            }

		}
		public void Dispose () {}
	}
}
