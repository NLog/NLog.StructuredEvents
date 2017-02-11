using System;
using System.Collections;
using System.Text;

namespace NLog.StructuredEvents
{
    /// <summary>
    /// Render or serialize a value, with optionnally backwardscompatible with <see cref="string.Format(System.IFormatProvider,string,object[])"/>
    /// </summary>
    public static class ValueRenderer
    {
        private const string LiteralFormatSymbol = "l";

        /// <summary>
        /// Serialize the value and append to the <paramref name="sb"/>.
        /// </summary>
        /// <param name="sb">Append to this builder.</param>
        /// <param name="value">The value to be appended.</param>
        /// <param name="legacy">is this legacy AKA string.format style?</param>
        /// <param name="format">Formatting for <see cref="IFormattable"/>.</param>
        /// <param name="formatProvider">Provider for formatting.</param>
        public static void AppendValue(StringBuilder sb, object value, bool legacy, string format, IFormatProvider formatProvider)
        {

            // todo support all scalar types: 

            // todo boolean
            // todo numerics complete? (formatable)
            // todo byte[] - hex?
            // todo datetime, timespan, datetimeoffset
            // todo nullables correct?
            // todo idict

            string stringValue;
            // Shortcut common case. It is important to do this before IEnumerable, as string _is_ IEnumerable
            if ((stringValue = value as string) != null)
            {
                AppendValueAsString(sb, stringValue, legacy, format);
                return;
            }


            IEnumerable collection;
            if (!legacy && (collection = value as IEnumerable) != null)
            {
                bool separator = false;
                foreach (var item in collection)
                {
                    if (separator) sb.Append(", ");
                    AppendValue(sb, item, false, format, formatProvider);
                    separator = true;
                }
                return;
            }

            IFormattable formattable;
            if (format != null && (formattable = value as IFormattable) != null)
            {
                sb.Append(formattable.ToString(format, formatProvider));
            }

            else if (value is char)
            {
                if (legacy || format == LiteralFormatSymbol)
                    sb.Append((char)value);
                else
                    sb.Append('"').Append((char)value).Append('"');
            }
            else
            {
                sb.Append(Convert.ToString(value, formatProvider));
            }
        }

        private static void AppendValueAsString(StringBuilder sb, string stringValue, bool legacy, string format)
        {
            if (legacy || format == LiteralFormatSymbol)
                sb.Append(stringValue);
            else
                sb.Append('"').Append(stringValue).Append('"');
        }
    }
}
