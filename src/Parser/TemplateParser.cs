using System;
using System.Collections.Generic;
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
                switch (c)
                {
                    case '{':
                        {
                            context.CharIndex++;
                            yield return ParseBracketPart(context);
                            break;
                        }
                    default:
                        {
                            yield return ParseTextPart(context, false);
                            break;
                        }
                }
            }
        }

        private TextPart ParseTextPart(ParserContext context, bool escapedPart)
        {
            var sb = new StringBuilder();

            char stopChar = escapedPart ? '}' : '{';

            foreach (var c in context.GetNext())
            {
                if (c == stopChar)
                {
                    //done
                    
                    context.CharIndex--; //re-read { (todo fix)
                    return new TextPart(sb.ToString(), escapedPart);
                }
                else
                {
                    sb.Append(c);
                }

            }
            return new TextPart(sb.ToString(), escapedPart);
        }


        private IPart ParseBracketPart(ParserContext context)
        {
            foreach (var c in context.GetNext())
            {
                switch (c)
                {
                    case '{':
                        {
                            //escacped bracket
                            var part = ParseTextPart(context, true);
                            context.CharIndex++;
                            return part;
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
            return new TextPart(context.CurrentChar, false);

        }


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
