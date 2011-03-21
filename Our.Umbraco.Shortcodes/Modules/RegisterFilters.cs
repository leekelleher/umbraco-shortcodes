using System;
using System.Web;

using Our.Umbraco.Shortcodes.Filters;

namespace Our.Umbraco.Shortcodes.Modules
{
	/// <summary>
	/// HttpModule to register the response filters.
	/// </summary>
	public class RegisterFilters : IHttpModule
	{
		/// <summary>
		/// Disposes of the resources (other than memory) used by the module that implements <see cref="T:System.Web.IHttpModule"/>.
		/// </summary>
		public void Dispose()
		{
		}

		/// <summary>
		/// Initializes a module and prepares it to handle requests.
		/// </summary>
		/// <param name="context">An <see cref="T:System.Web.HttpApplication"/> that provides access to the methods, properties, and events common to all application objects within an ASP.NET application</param>
		public void Init(HttpApplication context)
		{
			context.PostReleaseRequestState += new EventHandler(this.context_PostReleaseRequestState);
		}

		/// <summary>
		/// Handles the PostReleaseRequestState event of the context control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected void context_PostReleaseRequestState(object sender, EventArgs e)
		{
			if (HttpContext.Current.Response.ContentType == "text/html")
			{
				HttpContext.Current.Response.Filter = new ParseShortcodes(HttpContext.Current.Response.Filter);
			}
		}
	}
}