﻿using System;
using System.Web;

namespace Our.Umbraco.Shortcodes
{
	public class Application: IHttpModule
	{
		public void Dispose()
		{
		}

		public void Init(HttpApplication context)
		{
			context.PostReleaseRequestState += new EventHandler(context_PostReleaseRequestState);
		}

		protected void context_PostReleaseRequestState(object sender, EventArgs e)
		{
			if (HttpContext.Current.Response.ContentType == "text/html")
			{
				HttpContext.Current.Response.Filter = new ParseShortcodes(HttpContext.Current.Response.Filter);
			}
		}
	}
}
