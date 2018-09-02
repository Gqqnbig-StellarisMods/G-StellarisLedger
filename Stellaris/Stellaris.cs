using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace StellarisLedger
{
	public class Stellaris
	{
		public static string GetGameSaveContent(string saveGamePath)
		{
			using (var zipArchive = ZipFile.Open(saveGamePath, ZipArchiveMode.Read))
			{
				var gamestate = zipArchive.GetEntry("gamestate");
				string content;

				using (var sr = new StreamReader(gamestate.Open(), System.Text.Encoding.UTF8))
				{
					content = sr.ReadToEnd();
				}

				return content;
			}
		}
	}
}
