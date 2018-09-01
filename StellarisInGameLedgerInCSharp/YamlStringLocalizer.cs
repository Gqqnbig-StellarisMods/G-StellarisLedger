using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;

namespace StellarisInGameLedgerInCSharp
{
	public class YamlStringLocalizer : IStringLocalizer
	{
		public YamlStringLocalizer(IOptions<AppSettings> appSettings)
		{
			var c = CultureInfo.CurrentCulture;
			//appSettings.Value.LocalizationModPath
		}


		public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
		{
			throw new NotImplementedException();
		}

		public IStringLocalizer WithCulture(CultureInfo culture)
		{
			throw new NotImplementedException();
		}

		public LocalizedString this[string name]
		{
			get
			{
				return new LocalizedString(name, "hello");
			}
		}

		public LocalizedString this[string name, params object[] arguments] => throw new NotImplementedException();
	}

}
