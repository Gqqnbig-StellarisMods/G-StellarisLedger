using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;

namespace StellarisLedger
{
	public class YamlStringLocalizer : IHtmlLocalizer
	{
		private readonly string localizationModPath;
		private readonly string localizationModLanguage;
		private readonly string fallbackLocalizationPath;

		private readonly Dictionary<string, string> languageDictionary = new Dictionary<string, string>();

		public YamlStringLocalizer(IOptions<AppSettings> appSettings)
		{
			localizationModPath = appSettings.Value.LocalizationModPath;
			localizationModLanguage = appSettings.Value.LocalizationModLanguage;
			fallbackLocalizationPath = appSettings.Value.FallbackLocalizationPath;
		}


		public LocalizedString GetString(string name)
		{
			throw new NotImplementedException();
		}

		public LocalizedString GetString(string name, params object[] arguments)
		{
			throw new NotImplementedException();
		}

		public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
		{
			throw new NotImplementedException();
		}

		IHtmlLocalizer IHtmlLocalizer.WithCulture(CultureInfo culture)
		{
			throw new NotImplementedException();
		}

		public LocalizedHtmlString this[string name]
		{
			get
			{
				var ci = CultureInfo.CurrentCulture;

				for (; CultureInfo.InvariantCulture.Equals(ci) == false; ci = ci.Parent)
				{
					var languageName = ci.EnglishName;

					if (languageName.Equals(localizationModLanguage, StringComparison.OrdinalIgnoreCase))
					{
						var path = Path.Combine(localizationModPath, "l_english.yml");
						if (languageDictionary.TryGetValue(ci.Name, out var content) == false)
						{
							if (File.Exists(path) == false)
								continue;

							content = File.ReadAllText(path);
							languageDictionary.Add(ci.Name, content);
						}

						var regex = new Regex(@"^\s*" + name + @":\d+\s+""(.*?)""\s*$", RegexOptions.Multiline);
						var match = regex.Match(content);
						if (match.Success)
						{
							var value = match.Groups[1].Value;
							return new LocalizedHtmlString(name, value);
						}

					}
					else
					{
						if (ci.Name == "pt-BR")
							languageName = "braz_por";

						var path = Path.Combine(fallbackLocalizationPath, languageName, $"l_{languageName}.yml");
						if (languageDictionary.TryGetValue(ci.Name, out var content) == false)
						{
							if (File.Exists(path) == false)
								continue;

							content = File.ReadAllText(path);
							languageDictionary.Add(ci.Name, content);
						}

						var regex = new Regex(@"^\s*" + name + @":\d+\s+""(.*?)""\s*$", RegexOptions.Multiline);
						var match = regex.Match(content);
						if (match.Success)
						{
							var value = match.Groups[1].Value;
							return new LocalizedHtmlString(name, value);
						}
					}
				}

				return new LocalizedHtmlString(name, name, true);
			}
		}

		public LocalizedHtmlString this[string name, params object[] arguments] => throw new NotImplementedException();

	}

}
