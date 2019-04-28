using System;
using System.Text;

namespace StellarisLedger
{
	public static class StringExtensions
	{
		public static string DetectLineEnding(this string content)
		{
			string lineEndingSymbol;
			if (content.Contains("\r\n"))
				lineEndingSymbol = "\r\n";
			else if (content.Contains("\r"))
				lineEndingSymbol = "\r";
			else
				lineEndingSymbol = "\n";
			return lineEndingSymbol;
		}

		public static ReadOnlySpan<char> DetectLineEnding(this ReadOnlySpan<char> content)
		{
			//Span不可以是类级别的成员
			ReadOnlySpan<char> lineEndingSymbol;
			if (content.Contains("\r\n".AsSpan(), StringComparison.OrdinalIgnoreCase))
				lineEndingSymbol = "\r\n".AsSpan();
			else if (content.Contains("\r".AsSpan(), StringComparison.OrdinalIgnoreCase))
				lineEndingSymbol = "\r".AsSpan();
			else
				lineEndingSymbol = "\n".AsSpan();
			return lineEndingSymbol;
		}

		public static int IndexOf(this ReadOnlySpan<char> content, ReadOnlySpan<char> value, int startIndex)
		{
			var p = content.Slice(startIndex).IndexOf(value);
			if (p == -1)
				return -1;
			else
				return startIndex + p;
		}


		/// <summary>
		/// Returns the index of the start of the contents in a StringBuilder
		/// </summary>        
		/// <param name="value">The string to find</param>
		/// <param name="startIndex">The starting index.</param>
		/// <param name="ignoreCase">if set to <c>true</c> it will ignore case</param>
		/// <returns></returns>
		public static int IndexOf(this StringBuilder sb, string value, int startIndex = 0, bool ignoreCase = false)
		{
			int index;
			int length = value.Length;
			int maxSearchLength = (sb.Length - length) + 1;

			if (ignoreCase)
			{
				for (int i = startIndex; i < maxSearchLength; ++i)
				{
					if (Char.ToLower(sb[i]) == Char.ToLower(value[0]))
					{
						index = 1;
						while ((index < length) && (Char.ToLower(sb[i + index]) == Char.ToLower(value[index])))
							++index;

						if (index == length)
							return i;
					}
				}

				return -1;
			}

			for (int i = startIndex; i < maxSearchLength; ++i)
			{
				if (sb[i] == value[0])
				{
					index = 1;
					while ((index < length) && (sb[i + index] == value[index]))
						++index;

					if (index == length)
						return i;
				}
			}

			return -1;
		}
	}
}
