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
                        template.RenderHolePositional(sb, ref template.Holes[h++], parameters);
                    else
                        template.RenderHole(sb, ref template.Holes[h], parameters[h++]);
                }
            }
            return sb.ToString();
        }

        private static void RenderHolePositional(this Template template, StringBuilder sb, ref Hole hole, object[] parameters)
            => template.RenderHole(sb, ref hole, parameters[hole.Index], true);

        private static void RenderHole(this Template template, StringBuilder sb, ref Hole hole, object value, bool legacy = false)
        {
            // TODO: handle value == null
        
            // TODO: destructuring {@x}
        
            if (hole.CaptureType == CaptureType.Stringification)
            {
                // TODO: we don't need to support format and alignment here?
                sb.Append('"').Append(value.ToString()).Append('"');
                return;
            }

            // Shortcut common case. It is important to do this before IEnumerable, as string _is_ IEnumerable
            if (value is string)
            {
                template.AppendValue(sb, ref hole, value, legacy);
                return;
            }

            IEnumerable collection;
            if (!legacy && (collection = value as IEnumerable) != null)
            {
                bool separator = false;
                foreach (var item in collection) {
                    if (separator) sb.Append(", ");
                    template.AppendValue(sb, ref hole, item, false);
                    separator = true;
                }
                return;
            }

            template.AppendValue(sb, ref hole, value, legacy);
        }

        private static void AppendValue(this Template template, StringBuilder sb, ref Hole hole, object value, bool legacy)
        {
            // TODO: value can be null again (from IEnumerable)
            IFormattable formattable;
            string stringValue;
            if (hole.Format != null && (formattable = value as IFormattable) != null)
            {
                sb.Append(formattable.ToString(hole.Format, CultureInfo.CurrentCulture));
            }
            else if ((stringValue = value as string) != null)
            {
                if (legacy || hole.Format == "l")
                    sb.Append(stringValue);
                else
                    sb.Append('"').Append(stringValue).Append('"');
            }
            else if (value is char)
            {
                if (legacy || hole.Format == "l")
                    sb.Append((char)value);
                else
                    sb.Append('"').Append((char)value).Append('"');
            }
            else
            {
                sb.Append(value.ToString());
            }
        }
    }
}
