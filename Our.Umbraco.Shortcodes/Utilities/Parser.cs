using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using umbraco;
using umbraco.cms.helpers;
using umbraco.presentation.umbracobase;

namespace Our.Umbraco.Shortcodes.Utilities
{
	public class Parser
	{
		private page Page;

		private Regex ShortcodeMatch = new Regex(@"\[([^]\r\n]*)\]", RegexOptions.Compiled);

		private Encoding TextEncoding;

		public Parser(Encoding encoding, int pageId)
		{
			this.TextEncoding = encoding;
			this.Page = new page(pageId, Guid.Empty);
		}

		public string ParseShortcodes(string content)
		{
			// if the Umbraco page context is null
			if (this.Page == null)
			{
				// return the content
				return content;
			}

			// loop through all the shortcode matches.
			foreach (Match match in this.ShortcodeMatch.Matches(content))
			{
				// take the shortcode.
				string shortcode = match.Captures[0].Value;
				string value = shortcode;

				// test on the 2nd character (used for Umbraco internals).
				switch (shortcode.Substring(1, 1))
				{
					// for empty brackets []
					case "]":
						break;

					// Umbraco page elements
					case "#":
						value = string.Equals(shortcode, "[#urlName]")
							? url.FormatUrl(this.Page.PageName.ToLower())
							: helper.parseAttribute(this.Page.Elements, shortcode);
						break;

					// Umbraco internals
					case "@":
					case "%":
					case "$":
						value = helper.parseAttribute(this.Page.Elements, shortcode);
						break;

					// attempt to parse for extension method
					default:
						value = this.ParseExtensionMethod(shortcode);
						break;
				}

				// check if the value has changed.
				if (!string.Equals(shortcode, value))
				{
					if (string.IsNullOrEmpty(value))
					{
						// if the value is empty, remove the leading space from the shortcode
						content = content.Replace(string.Concat(" ", shortcode), value);
					}
					else
					{
						content = content.Replace(shortcode, value);
					}
				}
			}

			return content;
		}

		private string ParseExtensionMethod(string shortcode)
		{
			var extensionMethod = new Regex(@"\[([\w]+)(?:\:)([\w]+)\((.*)\b\)(?:\(\/\1\))?\]", RegexOptions.Compiled);

			// test if the shortcode is a valid triple-tag.
			if (extensionMethod.IsMatch(shortcode))
			{
				// match the extension method groups.
				var match = extensionMethod.Match(shortcode);
				if (match != null && match.Groups.Count > 3)
				{
					// assign the extension method groups.
					string ns = match.Groups[1].Value;
					string method = match.Groups[2].Value;
					string values = match.Groups[3].Value;

					// get the 'restExtension'.
					var restExtension = new restExtension(ns, method);
					if (restExtension.isAllowed)
					{
						// load up the parameters.
						var parameters = new List<object>() { null, null, ns, method };
						parameters.AddRange(values.Split(',').Select(s => s.Trim()));

						// invoke the /base method.
						var obj = new requestModule();
						var dynMethod = obj.GetType().GetMethod("invokeMethod", BindingFlags.NonPublic | BindingFlags.Instance);
						var result = dynMethod.Invoke(obj, new object[] { restExtension, parameters.ToArray() });

						return result.ToString();
					}

					// XsltExtension?
				}
			}

			// default, return shortcode.
			return shortcode;
		}
	}
}