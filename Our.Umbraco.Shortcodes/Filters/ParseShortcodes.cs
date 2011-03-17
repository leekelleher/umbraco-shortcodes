using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using umbraco;

namespace Our.Umbraco.Shortcodes.Filters
{
	class ParseShortcodes : MemoryStream
	{
		private Stream OutputStream = null;
		
		private Regex Shortcode = new Regex(@"\[(\@|\#|\%|\$)(.*)\b\](?:\[\/\2\])?", RegexOptions.Compiled);
		
		private page Page;

		public ParseShortcodes(Stream output)
		{
			this.OutputStream = output;

			int pageId;
			var value = HttpContext.Current.Items["pageID"];
			if (value != null && int.TryParse(value.ToString(), out pageId))
			{
				this.Page = new page(pageId, Guid.Empty);
			}
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			string content = UTF8Encoding.UTF8.GetString(buffer);

			foreach (Match match in this.Shortcode.Matches(content))
			{
				string code = match.Captures[0].Value;
				string value = helper.parseAttribute(this.Page.Elements, code);

			    content = content.Replace(code, value);
			}

			byte[] outputBuffer = UTF8Encoding.UTF8.GetBytes(content);
			this.OutputStream.Write(outputBuffer, 0, outputBuffer.Length);
		}
	}
}
