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

		public static int IndexOf(this ReadOnlySpan<char> content, ReadOnlySpan<char> value, int startIndex)
		{
			var p = content.Slice(startIndex).IndexOf(value);
			if (p == -1)
				return -1;
			else
				return startIndex + p;
		}
	}
}
