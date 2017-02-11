using System;
using System.Text;
using NLog.StructuredEvents.Parts;
using NLog.StructuredEvents.Serialization;

namespace NLog.StructuredEvents
{
    /// <summary>
    /// Render templates
    /// </summary>
    public static class TemplateRenderer
    {
        /// <summary>
        /// Render a template to a string.
        /// </summary>
        /// <param name="template">The template.</param>
        /// <param name="formatProvider">Culture.</param>
        /// <param name="parameters">Parameters for the holes.</param>
        /// <returns>Rendered template, never null.</returns>

        public static string Render(this Template template, IFormatProvider formatProvider, object[] parameters)
        {
            var sb = new StringBuilder(template.Value.Length + 64 * template.Holes.Length);
            int pos = 0;
            int holeIndex = 0;
            foreach (var literal in template.Literals)
            {
                sb.Append(template.Value, pos, literal.Print);
                pos += literal.Print;
                if (literal.Skip == 0)
                {
                    pos++;
                }
                else
                {
                    pos += literal.Skip;
                    if (template.IsPositional)
                    {
                        Hole hole = template.Holes[holeIndex++];
                        RenderHole(sb, hole, formatProvider, parameters[hole.Index], true);
                    }
                    else
                    {
                        RenderHole(sb, template.Holes[holeIndex], formatProvider, parameters[holeIndex++]);
                    }
                }
            }
            return sb.ToString();
        }

        private static void RenderHole(StringBuilder sb, Hole hole, IFormatProvider formatProvider, object value, bool legacy = false)
        {
            if (value == null)
            {
                sb.Append("NULL");
                return;
            }

            switch (hole.CaptureType)
            {
                case CaptureType.Stringify:
                    // TODO: we don't need to support format and alignment here?
                    sb.Append('"').Append(value.ToString()).Append('"');
                    break;
                case CaptureType.Serialize:
                    SerializationManager.Instance.SerializeObject(sb, value, formatProvider);
                    break;
                default:
                    var holeFormat = hole.Format;
                    ValueRenderer.AppendValue(sb, value, legacy, holeFormat, formatProvider);
                    break;
            }
        }
    }
}
