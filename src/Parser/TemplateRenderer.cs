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
            if (value == null)
            {
                sb.Append("NULL");
                return;
            }

            switch (hole.CaptureType)
            {
                case CaptureType.Stringification:
                    // TODO: we don't need to support format and alignment here?
                    sb.Append('"').Append(value.ToString()).Append('"');
                    break;
                case CaptureType.Destructuring:
                    DestructorManager.Instance.DestructureObject(sb, value);
                    break;
                default:
                    var holeFormat = hole.Format;
                    ValueRenderer.AppendValue(sb, value, legacy, holeFormat);
                    break;
            }
        }
    }
}
