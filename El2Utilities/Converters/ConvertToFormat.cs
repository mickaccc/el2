using System.Text;
using System.Text.RegularExpressions;

namespace El2Core.Converters
{
    public static class ConvertToFormat
    {

        public static string ConvertTTNR(string value, int firstBlock, int furtherBlock, char seperator, char endSeperator)
        {
            var strVal = value.Trim();

            Regex regex = new Regex("([a-zA-Z0-9]{" + firstBlock + "})([a-zA-Z0-9]{" +
                furtherBlock + "})([a-zA-Z0-9]{" + furtherBlock + "})([a-zA-Z0-9]*)");
            var match = regex.Match(strVal);
            if (match.Success)
            {
                StringBuilder sb = new StringBuilder(match.Groups[1].Value)
                    .Append(seperator).Append(match.Groups[2])
                    .Append(seperator).Append(match.Groups[3]);

                if (strVal.Length > 10)
                {
                    sb.Append(endSeperator).Append(match.Groups[4]);
                }
                return sb.ToString();
            }
            return value;
        }
    }
}
