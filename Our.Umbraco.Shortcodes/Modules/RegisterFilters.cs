using System;
using System.Linq;
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
			if (HttpContext.Current.Response.ContentType == "text/html" && this.IsPathAllowed())
			{
				HttpContext.Current.Response.Filter = new ParseShortcodes(HttpContext.Current.Response.Filter);
			}
		}

		/// <summary>
		/// Determines whether [is path allowed].
		/// </summary>
		/// <returns>
		/// 	<c>true</c> if [is path allowed]; otherwise, <c>false</c>.
		/// </returns>
		private bool IsPathAllowed()
		{
			string path = HttpContext.Current.Request.Path;

			var disallowedPaths = new string[]
			{
				"/aspnet_client/",
				"/bin/",
				"/config/",
				"/data/",
				"/install/",
				"/macroScripts/",
				"/masterpages/",
				"/media/",
				"/umbraco/",
				"/umbraco_client/",
				"/usercontrols/",
				"/xslt/"
			};

			bool allowed = disallowedPaths.Count(s => path.StartsWith(s)) == 0;

			if (allowed)
			{
				var disallowedExtensions = new string[]
				{
					"gif",
					"jpeg",
					"jpg",
					"png",
					"woff"
				};

				allowed = disallowedExtensions.Count(s => path.EndsWith(s)) == 0;
			}

			return allowed;
		}
	}
}