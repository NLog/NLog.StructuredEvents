using System;
using System.Collections;
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

        /// <summary>
        /// Render hole
        /// </summary>
        /// <param name="sb"></param>
        /// <param name="part"></param>
        /// <param name="holeIndex"></param>
        /// <param name="legacyMode"></param>
        /// <param name="args"></param>
        public void RenderHole(StringBuilder sb, HolePart part, int holeIndex, bool legacyMode, object[] args)
        {
            var val = args[holeIndex];

            var convertedToString = false;
            if (part.HoleType == HoleType.Destructuring)
            {
                //todo
            }
            else if (part.HoleType == HoleType.Stringification)
            {
                convertedToString = true;
                val = val.ToString();
            }
            
            AppendVal(sb, part, val, legacyMode, convertedToString);
        }
   

        /// <summary>
        /// render value
        /// </summary>
        /// <param name="sb"></param>
        /// <param name="part"></param>
        /// <param name="val"></param>
        /// <param name="legacyMode"></param>
        /// <param name="convertedToString"></param>
        private static void AppendVal(StringBuilder sb, HolePart part, object val, bool legacyMode,
            bool convertedToString)
        {
            if (convertedToString || val is string || val is char)
            {
                //don't quote on format=l
                var quote = !legacyMode && (part.Format != "l");
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
                AppendNonString(sb, part, val, legacyMode);
            }
        }

        /// <summary>
        /// render a non-string value
        /// </summary>
        /// <param name="sb"></param>
        /// <param name="part"></param>
        /// <param name="val"></param>
        /// <param name="legacyMode"></param>
        private static void AppendNonString(StringBuilder sb, HolePart part, object val, bool legacyMode)
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

                        AppendVal(sb, part, valItem, false, false);
                    }
                    return;
                }
            }

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

   
    }
}