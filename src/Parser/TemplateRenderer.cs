using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Parser.Parts;

namespace Parser
{
    public static class TemplateRenderer
    {

        public static string Render(this Template template, object[] parameters)
        {
            var sb = new StringBuilder(template.Value.Length + 64 * template.Holes.Length);
            int pos = 0;
            int h = 0;
            foreach (var literal in template.Literals)
            {
                sb.Append(template.Value, pos, literal.Print);
                pos += literal.Print;
                if (literal.Skip == 0)
                    pos++;
                else
                {
                    pos += literal.Skip;
                    if (template.IsPositional)
                        RenderHolePositional(sb, template.Holes[h++], parameters);
                    else
                        RenderHole(sb, template.Holes[h], parameters[h++]);
                }
            }
            return sb.ToString();
        }

        private static void RenderHolePositional(StringBuilder sb, Hole hole, object[] parameters)
            => RenderHole(sb, hole, parameters[hole.Index], true);

        private static void RenderHole(StringBuilder sb, Hole hole, object value, bool legacy = false)
        {
            // TODO: handle value == null

            // TODO: destructuring {@x}

            if (hole.CaptureType == CaptureType.Stringification)
            {
                // TODO: we don't need to support format and alignment here?
                sb.Append('"').Append(value.ToString()).Append('"');
                return;
            }


            var holeFormat = hole.Format;




            AppendValue(sb, value, legacy, holeFormat);


        }

        private static void AppendValue(StringBuilder sb, object value, bool legacy, string holeFormat)
        {

            string stringValue;
            // Shortcut common case. It is important to do this before IEnumerable, as string _is_ IEnumerable
            if ((stringValue = value as string) != null)
            {
                AppendValueAsString(sb, stringValue, legacy, holeFormat);
                return;
            }


            IEnumerable collection;
            if (!legacy && (collection = value as IEnumerable) != null)
            {
                bool separator = false;
                foreach (var item in collection)
                {
                    if (separator) sb.Append(", ");
                    AppendValue(sb, item, false, holeFormat);
                    separator = true;
                }
                return;
            }

            IFormattable formattable;
            if (holeFormat != null && (formattable = value as IFormattable) != null)
            {
                sb.Append(formattable.ToString(holeFormat, CultureInfo.CurrentCulture));
            }

            else if (value is char)
            {
                if (legacy || holeFormat == "l")
                    sb.Append((char)value);
                else
                    sb.Append('"').Append((char)value).Append('"');
            }
            else
            {
                sb.Append(value.ToString());
            }
        }

        private static void AppendValueAsString(StringBuilder sb, string stringValue, bool legacy, string holeFormat)
        {
            if (legacy || holeFormat == "l")
                sb.Append(stringValue);
            else
                sb.Append('"').Append(stringValue).Append('"');
        }
    }
}
