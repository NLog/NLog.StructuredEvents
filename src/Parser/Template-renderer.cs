using System;
using System.Collections;
using System.Globalization;
using System.Text;

namespace Parser
{
    ///<summary>Render a template</summary>
    public class Renderer
    {
        private readonly Template _template;

        public Renderer(Template template)
        {
            _template = template;
        }

        public string Render(object[] args)
        {
            var sb = new StringBuilder(_template.Value.Length * 2);

            int pos = 0;
            int holeIndex = 0;
            foreach (var literal in _template.Literals)
            {
                sb.Append(_template.Value, pos, literal.Print);
                pos += literal.Print;
                if (literal.Skip == 0)
                {
                    // 0 means escaping or end of string without hole.
                    if (pos < _template.Value.Length)
                        pos++;
                }
                else
                {
                    pos += literal.Skip;
                    var hole = _template.Holes[holeIndex];
                    var argIndex = _template.IsPositional ? hole.Index : holeIndex;
                    holeIndex++;

                    RenderHole(sb, ref hole, argIndex, _template.IsPositional, args);
                }
            }

            return sb.ToString();
        }



        /// <summary>
        /// Render hole
        /// </summary>
        /// <param name="sb"></param>
        /// <param name="hole"></param>
        /// <param name="argIndex"></param>
        /// <param name="legacyMode"></param>
        /// <param name="args"></param>
        public static void RenderHole(StringBuilder sb, ref Hole hole, int argIndex, bool legacyMode, object[] args)
        {

            var val = args[argIndex];

            var convertedToString = false;
            //if (part.HoleType == HoleType.Destructuring)
            //{
            //    //todo
            //}
            //else 
            if (hole.CaptureType == CaptureType.Stringification)
            {
                convertedToString = true;
                val = val.ToString();
            }

            AppendVal(sb, ref hole, val, legacyMode, convertedToString);
        }


        /// <summary>
        /// render value
        /// </summary>
        /// <param name="sb"></param>
        /// <param name="hole"></param>
        /// <param name="val"></param>
        /// <param name="legacyMode"></param>
        /// <param name="convertedToString"></param>
        private static void AppendVal(StringBuilder sb, ref Hole hole, object val, bool legacyMode,
            bool convertedToString)
        {
            if (convertedToString || val is string || val is char)
            {
                //don't quote on format=l
                var quote = !legacyMode && (hole.Format != "l");
                if (quote)
                {
                    //renders string values in double quotes to more transparently
                    sb.Append('"');
                    sb.Append(val);
                    sb.Append('"');
                }
                else
                {
                    sb.Append(val);
                }
            }
            else
            {
                AppendNonString(sb, ref hole, val, legacyMode);
            }
        }

        /// <summary>
        /// render a non-string value
        /// </summary>
        /// <param name="sb"></param>
        /// <param name="hole"></param>
        /// <param name="val"></param>
        /// <param name="legacyMode"></param>
        private static void AppendNonString(StringBuilder sb, ref Hole hole, object val, bool legacyMode)
        {
            if (!legacyMode)
            {
                var items = val as IEnumerable;
                if (items != null)
                {
                    var isFirst = true;
                    foreach (var valItem in items)
                    {
                        if (!isFirst)
                        {
                            sb.Append(',');
                            sb.Append(' ');
                        }
                        else
                        {
                            isFirst = false;
                        }

                        AppendVal(sb, ref hole, valItem, false, false);
                    }
                    return;
                }
            }

            if (hole.Format != null)
            {
                var formattable = val as IFormattable;
                if (formattable != null)
                {
                    sb.Append(formattable.ToString(hole.Format, CultureInfo.CurrentCulture));
                    return;
                }
            }
            sb.Append(val);
        }


    }
}