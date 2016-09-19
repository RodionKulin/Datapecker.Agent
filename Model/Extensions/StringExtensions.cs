using Datapecker.Agent.ReportingService;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Datapecker.Agent
{
    internal static class StringExtensions
    {
        public static string ToShortString(this string input, int? maxLength = null)
        {
            if (input == null)
                return null;

            maxLength = maxLength ?? AgentConstants.MAX_STRING_LENGTH;

            if (input.Length > maxLength)
                return input.Substring(0, maxLength.Value);
            else
                return input;
        }

        public static string ToIso8601String(this DateTime dateTime)
        {
            string zEnding = dateTime.Kind != DateTimeKind.Local
                ? "Z"
                : string.Empty;

            return dateTime.ToString("yyyy-MM-ddTHH:mm:ss") + zEnding;
        }

        public static DateTime? ToIso8601DateTime(this string dateTime)
        {
            if (string.IsNullOrEmpty(dateTime))
            {
                return null;
            }

            DateTime parsedDateTime;
            if (!DateTime.TryParse(dateTime, null, DateTimeStyles.AssumeUniversal, out parsedDateTime))
            {
                return null;
            }

            if (dateTime.ToLowerInvariant().EndsWith("z"))
            {
                parsedDateTime = parsedDateTime.ToUniversalTime();
            }

            return parsedDateTime;
        }
    }
}
