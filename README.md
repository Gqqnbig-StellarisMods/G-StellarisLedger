# G-Stellaris Ledger 群星统计器・极
[![Build status](https://ci.appveyor.com/api/projects/status/9d682arbbq245nsw?svg=true)](https://ci.appveyor.com/project/gqqnbig/stellarisingameledgerincsharp)


Watch [Stellaris Ledger Performance Comparison](https://www.youtube.com/watch?v=hKqeLxOMQMs)

Watch [Stellaris Ledger demonstration](https://www.youtube.com/watch?v=lsYn0TM1NG4)

## Invite Pull Requests
Please help work on the following functions
- [x] Localization: Ideally the ledger reads table columns from the game folder or the mod folder. If the user installed translation mod, the ledger can display the same text as the game. However, hard-code localization text is acceptable.
- [ ] Planet distance: Create a separate view that allows to show distance between planets. One use case is to select a planet, and show distance between another planet to this planet. This is useful when we need to find a planet to build a colonization ship.

## FAQ
### Available settings in `appsettings.json`?
The following are all the settings. You may ignore the Logging section.

```
{
	"Logging": {
		"IncludeScopes": false,
		"Debug": {
			"LogLevel": {
				"Default": "Warning"
			}
		},
		"Console": {
			"LogLevel": {
				"Default": "Warning"
			}
		}
	},

	"AppSettings": {
		"saveGamesPath": "D:\\Documents\\Paradox Interactive\\Stellaris\\save games",
		"localizationModPath": "D:\\Documents\\Paradox Interactive\\Stellaris\\mod\\cn\\localisation\\english",
		"localizationModLanguage": "Chinese (Simplified)",
		"fallbackLocalizationPath": "D:\\SteamLibrary\\steamapps\\common\\Stellaris\\localisation"
	}
}
```
localizationModLanguage must take a value of the format specified on https://docs.microsoft.com/dotnet/api/system.globalization.cultureinfo.englishname . Sample values are 

Arabic                                  
Bulgarian                               
Catalan                                 
Chinese (Simplified)                    
Czech                                   
Danish                                  
German                                  
Greek                                   
English                                 
Spanish                                 
Finnish                                 
Chinese                                 
Chinese (Traditional)                   
Chinese (Simplified) Legacy             
Chinese (Traditional) Legacy            



### Can the ledger display in my language?
Yes, as long as Stellaris game displays in your language, and the preferred language header in the HTTP request is the language you like. To change the preferred language header, go to your browser settings.
