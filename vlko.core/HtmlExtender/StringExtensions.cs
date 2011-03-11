using System.Web.Mvc;
using vlko.core.Tools;

namespace vlko.core.HtmlExtender
{
	/// <summary>
	/// Custom string utility methods.
	/// </summary>
	public static class StringExtensions
	{
		/// <summary>
		/// Get a substring of the first N characters.
		/// </summary>
		public static string Truncate(this string source, int length)
		{
			return StringTool.Truncate(source, length);
		}

		/// <summary>
		/// Line breaks replaces with BR tag.
		/// </summary>
		/// <param name="source">The source.</param>
		/// <returns>Line breaks replaced with BR tag</returns>
		public static MvcHtmlString LineBreaksToBr(this HtmlHelper htmlHelper, string source)
		{
			if (!string.IsNullOrEmpty(source))
			{
				source = source.Replace("\n", "<br/>");
			}
			return MvcHtmlString.Create(source);
		}
	}
}
