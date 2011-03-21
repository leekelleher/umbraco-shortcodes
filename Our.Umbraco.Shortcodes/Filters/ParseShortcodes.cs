using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

using umbraco;
using umbraco.cms.helpers;
using umbraco.presentation.umbracobase;

namespace Our.Umbraco.Shortcodes.Filters
{
	/// <summary>
	/// Response filter for parsing shortcodes.
	/// </summary>
	public class ParseShortcodes : MemoryStream
	{
		/// <summary>
		/// Field for the response output stream.
		/// </summary>
		private Stream OutputStream = null;

		/// <summary>
		/// Field for the regular expression to match shortcodes
		/// </summary>
		private Regex ShortcodeMatch = new Regex(@"\[(.*)\b\](?:\[\/\1\])?", RegexOptions.Compiled);

		/// <summary>
		/// Field for the Page object.
		/// </summary>
		private page Page;

		/// <summary>
		/// Initializes a new instance of the <see cref="ParseShortcodes"/> class.
		/// </summary>
		/// <param name="output">The output.</param>
		public ParseShortcodes(Stream output)
		{
			// grab the response output stream.
			this.OutputStream = output;

			// get the Umbraco page
			int pageId;
			var value = HttpContext.Current.Items["pageID"];
			if (value != null && int.TryParse(value.ToString(), out pageId))
			{
				this.Page = new page(pageId, Guid.Empty);
			}
		}

		/// <summary>
		/// Writes a block of bytes to the current stream using data read from buffer.
		/// </summary>
		/// <param name="buffer">The buffer to write data from.</param>
		/// <param name="offset">The byte offset in <paramref name="buffer"/> at which to begin writing from.</param>
		/// <param name="count">The maximum number of bytes to write.</param>
		/// <exception cref="T:System.ArgumentNullException">
		/// 	<paramref name="buffer"/> is null. </exception>
		/// <exception cref="T:System.NotSupportedException">The stream does not support writing. For additional information see <see cref="P:System.IO.Stream.CanWrite"/>.-or- The current position is closer than <paramref name="count"/> bytes to the end of the stream, and the capacity cannot be modified. </exception>
		/// <exception cref="T:System.ArgumentException">
		/// 	<paramref name="offset"/> subtracted from the buffer length is less than <paramref name="count"/>. </exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		/// 	<paramref name="offset"/> or <paramref name="count"/> are negative. </exception>
		/// <exception cref="T:System.IO.IOException">An I/O error occurs. </exception>
		/// <exception cref="T:System.ObjectDisposedException">The current stream instance is closed. </exception>
		public override void Write(byte[] buffer, int offset, int count)
		{
			// if the Umbraco page context is null
			if (this.Page == null)
			{
				// return the response output stream
				this.OutputStream.Write(buffer, offset, count);
				return;
			}

			// get the string from the buffer
			string content = UTF8Encoding.UTF8.GetString(buffer);

			// loop through all the shortcode matches.
			foreach (Match match in this.ShortcodeMatch.Matches(content))
			{
				// take the shortcode.
				string shortcode = match.Captures[0].Value;
				string value = shortcode;

				// test on the 2nd character (used for Umbraco internals).
				switch (shortcode.Substring(1, 1))
				{
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

					// attempt to parse for triple-tag.
					default:
						value = this.ParseTripleTag(shortcode);
						break;
				}

				// check if the value has changed.
				if (!string.Equals(shortcode, value))
				{
					content = content.Replace(shortcode, value);
				}
			}

			// write the content changes back to the buffer.
			byte[] outputBuffer = UTF8Encoding.UTF8.GetBytes(content);
			this.OutputStream.Write(outputBuffer, 0, outputBuffer.Length);
		}

		/// <summary>
		/// Parses the triple tag.
		/// </summary>
		/// <param name="shortcode">The shortcode.</param>
		/// <returns>
		/// Returns the parsed triple tag, (with value from restExtension).
		/// </returns>
		private string ParseTripleTag(string shortcode)
		{
			var tripleTag = new Regex(@"\[([\w]+)(?:\:)([\w]+)(?:\=)?(.*)?\]", RegexOptions.Compiled);

			// test if the shortcode is a valid triple-tag.
			if (tripleTag.IsMatch(shortcode))
			{
				// match the triple-tag groups.
				var match = tripleTag.Match(shortcode);
				if (match != null && match.Groups.Count > 3)
				{
					// assign the triple-tag groups.
					string ns = match.Groups[1].Value;
					string predicate = match.Groups[2].Value;
					string values = match.Groups[3].Value;

					// get the 'restExtension'.
					var restExtension = new restExtension(ns, predicate);
					if (restExtension.isAllowed)
					{
						// load up the parameters.
						var parameters = new List<object>() { null, null, ns, predicate };
						parameters.AddRange(values.Split('/'));

						// invoke the /base method.
						var obj = new requestModule();
						var dynMethod = obj.GetType().GetMethod("invokeMethod", BindingFlags.NonPublic | BindingFlags.Instance);
						var result = dynMethod.Invoke(obj, new object[] { restExtension, parameters.ToArray() });

						return result.ToString();
					}
				}
			}

			// default, return shortcode.
			return shortcode;
		}
	}
}
