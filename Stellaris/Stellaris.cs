using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

		public static IEnumerable<string> GetMostRecentSaves(string saveGamesPath, int? limit)
		{
			var saveGamesDirectory = new DirectoryInfo(saveGamesPath);
			var directories = saveGamesDirectory.GetDirectories();
			if (directories.Any() == false)
				throw new ArgumentException($"存档目录\"{saveGamesPath}\"不包含子文件夹");

			var mostRecentWriteTime = directories.Max(d => d.LastWriteTime);
			var mostRecentPlayDirectory = directories.First(d => d.LastWriteTime == mostRecentWriteTime);

			var files = mostRecentPlayDirectory.GetFiles("*.sav");
			if (files.Any() == false)
				throw new ArgumentException($"存档目录\"{mostRecentPlayDirectory.FullName}\"不包含游戏存档文件(*.sav)");

			var orderedFiles = files.OrderByDescending(f => f.LastWriteTime).ToList();
			//排除文件名日期顺序不符的

			int lastDate = Int32.MaxValue;
			for (int i = 0; i < orderedFiles.Count; i++)
			{
				var dateString = Path.GetFileNameWithoutExtension(orderedFiles[i].Name);
				try
				{
					dateString = dateString.Replace(".", "");
					int d = Convert.ToInt32(dateString);
					if (d < lastDate)
						lastDate = d;
					else
						orderedFiles.RemoveAt(i--);
				}
				catch
				{
					orderedFiles.RemoveAt(i--);
				}
			}

			var result = orderedFiles.Select(f => f.FullName.Substring(saveGamesPath.Length).Trim(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar));
			if (limit == null)
				return result;
			else
				return result.Take(limit.Value);
		}
	}
}
