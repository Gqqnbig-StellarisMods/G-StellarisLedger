using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace StellarisLedger
{
	public static class HtmlHelpers
	{
		public static IHtmlContent RawResource(this IHtmlHelper html, LocalizedHtmlString value)
		{
			return html.Raw(value.Value);
		}

		public static IHtmlContent Raw(this IHtmlHelper html, LocalizedHtmlString format, params LocalizedHtmlString[] objects)
		{
			return html.Raw(string.Format(format.Value, objects.Select(s => s.Value).ToArray()));
		}
	}
}
