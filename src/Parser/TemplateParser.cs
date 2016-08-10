using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Parser
{
    public class TemplateParser
    {
        public PartList Parse(string template)
        {
            if (!string.IsNullOrEmpty(template))
            {
                var context = new ParserContext(template);
                var parts = Parse(context);
                return new PartList(parts);

            }
            return new PartList();
        }

        private IEnumerable<IPart> Parse(ParserContext context)
        {

            foreach (var c in context.GetNext())
            {

                if (c == '{')
                {
                    context.CharIndex++;
                    yield return ParseOpenBracketPart(context);
                }
                else if (c == '}')
                {
                    context.CharIndex++;
                    yield return ParseCloseBracketPart(context);
                }
                else
                {
                    yield return ParseTextPart(context);
                }


            }
        }

        /// <summary>
        /// Parse normal text
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private TextPart ParseTextPart(ParserContext context)
        {
            var sb = new StringBuilder();


            foreach (var c in context.GetNext())
            {
                if (c == '{' || c == '}')
                {
                    //done

                    context.CharIndex--; //re-read { (todo fix)
                    return new TextPart(sb.ToString());
                }
                else
                {
                    sb.Append(c);
                }

            }
            return new TextPart(sb.ToString());
        }

        private IPart ParseCloseBracketPart(ParserContext context)
        {
            var nextChar = context.GetNext().FirstOrDefault();
            if (nextChar == '}')
            {
                return new TextPart('}', '}');
            }
            throw new Exception("invalid close } on index " + context.CharIndex);

        }

        /// <summary>
        /// Parse after found {
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private IPart ParseOpenBracketPart(ParserContext context)
        {
            foreach (var c in context.GetNext())
            {
                switch (c)
                {
                    case '{':
                        {
                            return new TextPart('{', '{');
                        }
                    case '@':
                        {
                            return ParseHole(context, HoleType.Destructuring);
                        }
                    case '$':
                        {
                            return ParseHole(context, HoleType.Stringification);
                        }
                    case '0':
                    case '1':
                    case '2':
                    case '3':
                    case '4':
                    case '5':
                    case '6':
                    case '7':
                    case '8':
                    case '9':
                        {
                            //todo "0a" is wrong of course
                            return ParseHole(context, HoleType.Numeric);
                        }
                    default:
                        return ParseHole(context, HoleType.Text);
                }
            }
            return new TextPart(context.CurrentChar.ToString());

        }

        /// <summary>
        /// Parse after {@, {$, {0-9, {a-z
        /// </summary>
        /// <param name="context"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        private IPart ParseHole(ParserContext context, HoleType type)
        {
            var nameSb = new StringBuilder();

            foreach (var c in context.GetNext())
            {
                switch (c)
                {
                    case ':':
                        {
                            var format = ParseFormat(context);
                            //done
                            return new HolePart(nameSb.ToString(), type, context.PartIndex++, format);
                        }
                    case '}':
                        {
                            //done
                            return new HolePart(nameSb.ToString(), type, context.PartIndex++, null);
                        }
                    default:
                        nameSb.Append(c);
                        break;

                }
            }
            //todo
            throw new Exception("parse failed");
        }

        private string ParseFormat(ParserContext context)
        {
            var formatSb = new StringBuilder();
            formatSb.Append(context.CurrentChar);
            foreach (var c in context.GetNext())
            {

                if (c == '}')
                {
                    break;
                }
                else
                {
                    formatSb.Append(c);
                }

            }
            return formatSb.ToString();
        }
    }

    //{0}asd
    //asd{0}
}
