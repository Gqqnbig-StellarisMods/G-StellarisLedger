namespace StellarisInGameLedgerInCSharp
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
    }
}
