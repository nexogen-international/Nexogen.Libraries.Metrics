using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Nexogen.Libraries.Metrics.Prometheus
{
    public static class PrometheusConventions
    {
        private static readonly Regex NameRegex = new Regex("^[a-zA-Z_:][a-zA-Z0-9_:]*$");
        private static readonly Regex LabelRegex = new Regex("^[a-zA-Z_][a-zA-Z0-9_]*$");

        public static readonly Encoding PrometheusEncoding = new UTF8Encoding(false);

        public static string BuildLabels(string[] labelNames, string[] labels)
        {
            var labelList = new StringBuilder();

            if (labelNames.Length != 0)
            {
                labelList.Append("{");

                labelList.Append(labelNames[0]);
                labelList.Append("=\"");
                labelList.Append(labels[0]);
                labelList.Append("\"");

                for (int i = 1; i < labelNames.Length; i++)
                {
                    labelList.Append(",");
                    labelList.Append(labelNames[i]);
                    labelList.Append("=\"");
                    labelList.Append(labels[i]);
                    labelList.Append("\"");
                }

                labelList.Append("}");
            }

            return labelList.ToString();
        }

        public static string BuildHistogramLabels(string labels, double max, long prevBucketsTotal)
        {
            var labelList = new StringBuilder(labels);
            if (labelList.Length == 0)
            {
                labelList.Append("{");
            }
            else
            {
                labelList.Remove(labelList.Length - 1, 1);
                labelList.Append(", ");
            }

            if (Double.IsPositiveInfinity(max))
            {
                labelList.Append("le=\"" + "+Inf" + "\"} " + prevBucketsTotal);
            }
            else
            {
                labelList.Append("le=\"" + max.ToString(CultureInfo.InvariantCulture) + "\"} " + prevBucketsTotal);
            } 

            return labelList.ToString();
        }

        public static bool IsValidHistogramName(string name)
        {
            return name != "le" && IsValidName(name);
        }

        public static bool IsValidName(string name)
        {
            return NameRegex.IsMatch(name);
        }

        public static bool IsValidLabel(string label)
        {
            return !label.StartsWith("__") && LabelRegex.IsMatch(label);
        }

        public static bool AreValidHistogramNames(IEnumerable<string> names)
        {
            if (names.Any(l => l == "le"))
            {
                return false;
            }

            return AreValidNames(names);
        }

        public static bool AreValidNames(IEnumerable<string> names)
        {
            if (!names.Any())
            {
                return false;
            }

            return !names.Any(l => !NameRegex.IsMatch(l));
        }

        public static string EscapeLabel(string label)
        {
            return label.Replace("\\", @"\\")
                        .Replace("\n", @"\n")
                        .Replace("\"", @"\""");
        }

        public static string EscapeHelp(string help)
        {
            return help.Replace("\\", @"\\")
                       .Replace("\n", @"\n");
        }
    }
}
