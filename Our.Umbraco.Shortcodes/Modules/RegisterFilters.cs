using System;
using System.Net.Mime;
using System.Web;
using Our.Umbraco.Shortcodes.Filters;
using Our.Umbraco.Shortcodes.Utilities;
using umbraco.IO;

namespace Our.Umbraco.Shortcodes.Modules
{
	/// <summary>
	/// HttpModule to register the response filters.
	/// </summary>
	public class RegisterFilters : IHttpModule
	{
		public const string InstallKey = "ShortcodesModuleInstalled";

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
			if (HttpContext.Current != null)
			{
				var context = HttpContext.Current;
				if (!context.Items.Contains(InstallKey))
				{
					var response = context.Response;
					var currentExecutionFilePath = context.Request.CurrentExecutionFilePath;

					if ((response.ContentType == MediaTypeNames.Text.Html) && (!this.IsReservedPath(currentExecutionFilePath)))
					{
						int pageId;
						var value = HttpContext.Current.Items["pageID"];
						if (value != null && int.TryParse(value.ToString(), out pageId))
						{
							var parser = new Parser(response.ContentEncoding, pageId);
							var filter = new ResponseFilterStream(response.Filter);
							filter.TransformString += new Func<string, string>(parser.ParseShortcodes);
							response.Filter = filter;
						}
					}

					context.Items.Add(InstallKey, new object());
				}
			}
		}

		private bool IsReservedPath(string path)
		{
			var reservedPaths = new[]
			{
				"~/aspnet_client/",
				SystemDirectories.Base,
				SystemDirectories.Bin,
				SystemDirectories.Config,
				SystemDirectories.Css,
				SystemDirectories.Data,
				SystemDirectories.Install,
				SystemDirectories.MacroScripts,
				SystemDirectories.Masterpages,
				SystemDirectories.Media,
				SystemDirectories.Packages,
				SystemDirectories.Preview,
				SystemDirectories.Scripts,
				SystemDirectories.Umbraco,
				SystemDirectories.Umbraco_client,
				SystemDirectories.Usercontrols,
				SystemDirectories.Webservices,
				SystemDirectories.Xslt
			};

			foreach (var reservedPath in reservedPaths)
			{
				if (path.StartsWith(IOHelper.ResolveUrl(reservedPath)))
				{
					return true;
				}
			}

			foreach (var extension in new[] { ".gif", ".jpeg", ".jpg", ".png", ".woff" })
			{
				if (path.EndsWith(extension))
				{
					return true;
				}
			}

			return false;
		}
	}
}