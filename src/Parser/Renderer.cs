using System;
using System.Globalization;
using System.Text;

namespace Parser
{
    public class Renderer
    {
        /// <summary>
        /// Render all
        /// </summary>
        /// <param name="list"></param>
        /// <param name="args"></param>
        public string Render(PartList list, object[] args)
        {
            var sb = new StringBuilder();
            if (list.IsPositional)
            {
                for (var i = 0; i < list.Count; i++)
                {
                    var item = list[i];
                    item.RenderPartIndexed(sb, this, args);
                }
            }
            else
            {
                var argIndex = 0;
                for (var i = 0; i < list.Count; i++)
                {
                    var item = list[i];
                    argIndex = item.RenderPart(sb, this, argIndex, args);
                }
            }
            return sb.ToString();
        }

        public void RenderPart(StringBuilder sb, HolePart part, int holeIndex, bool allowQuoted, object[] args)
        {
            var val = args[holeIndex];

            if (part.HoleType == HoleType.Destructuring)
            {
                //todo
            }
            else if (part.HoleType == HoleType.Stringification)
            {
                //todo toString()
            }


            //don't quote on format=l
            var quote = allowQuoted && (part.Format != "l");
            if (!quote)
            {
                AppendVal(sb, part, val);
                return;
            }


            if (val is string || val is char)
            {
                //renders string values in double quotes to more transparently
                sb.Append('"');
                sb.Append(val);
                sb.Append('"');
            }
            else
            {
                AppendVal(sb, part, val);
            }
        }

        private static void AppendVal(StringBuilder sb, HolePart part, object val)
        {
            if (part.Format != null)
            {
                var formattable = val as IFormattable;
                if (formattable != null)
                {
                    sb.Append(formattable.ToString(part.Format, CultureInfo.CurrentCulture));
                    return;
                }
            }
            sb.Append(val);
        }

        public void RenderPart(StringBuilder sb, TextPart part)
        {
            sb.Append(part.Text);
        }
    }
}