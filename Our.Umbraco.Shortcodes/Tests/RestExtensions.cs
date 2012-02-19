using System;
using umbraco.presentation.umbracobase;

namespace Our.Umbraco.Shortcodes.Tests
{
	/// <summary>
	/// Test REST methods for shortcodes.
	/// </summary>
	[RestExtension("Shortcodes")]
	public class RestExtensions
	{
		/// <summary>
		/// Returns an 'hello world' string.
		/// </summary>
		/// <returns>Returns an example string.</returns>
		[RestExtensionMethod(returnXml = false)]
		public static string HelloWorld()
		{
			return "hello world";
		}

		/// <summary>
		/// Returns today's date in the specified format.
		/// </summary>
		/// <param name="format">The format.</param>
		/// <returns>Returns a string of today's date in the specified format.</returns>
		[RestExtensionMethod(returnXml = false)]
		public static string Today(string format)
		{
			return DateTime.Today.ToString(format);
		}

		/// <summary>
		/// Returns the larger of the 2 values.
		/// </summary>
		/// <param name="val1">The first value.</param>
		/// <param name="val2">The second value.</param>
		/// <returns>Returns the larger of the specified values.</returns>
		[RestExtensionMethod(returnXml = false)]
		public static int Max(int val1, int val2)
		{
			return Math.Max(val1, val2);
		}
	}
}