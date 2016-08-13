using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Parser.Tests
{
    public class ParserTests
    {
        [Theory]
        [InlineData("")]
        [InlineData("Hello {0}")]
        [InlineData("I like my {car}")]
        [InlineData("But when Im drunk I need a {cool} bike")]
        [InlineData("I have {0} {1} {2} parameters")]
        [InlineData("{0} on front")]
        [InlineData(" {0} on front")]
        [InlineData("end {1}")]
        [InlineData("end {1} ")]
        [InlineData("{name} is my name")]
        [InlineData(" {name} is my name")]
        [InlineData("{multiple}{parameters}")]
        [InlineData("I have {{text}} and {{0}}")]
        [InlineData("{{text}}{{0}}")]
        [InlineData(" {{text}}{{0}} ")]
        [InlineData(" {0} ")]
        [InlineData(" {1} ")]
        [InlineData(" {2} ")]
        [InlineData(" {3} {4} {9} {8} {5} {6} {7}")]
        [InlineData(" {{ ")]
        [InlineData("{{ ")]
        [InlineData(" {{")]
        [InlineData(" }} ")]
        [InlineData("}} ")]
        [InlineData(" }}")]
        [InlineData("{0:000}")]
        [InlineData("{aaa:000}")]
        [InlineData(" {@destructre} ")]
        [InlineData(" {$stringify} ")]
        public void ParseAndPrint(string input)
        {
            var parts = TemplateParser.Parse(input);

            //toString will reconstuct everthing
            var printed = parts.Print();
            Assert.Equal(input, printed);
        }

        [Theory]
        [InlineData("{0}", true, 0)]
        [InlineData("{9}", true, 9)]
        [InlineData("{1 }", true, 1)]
        [InlineData("{1} {2}", true, 1)]
        [InlineData("{@3} {$4}", true, 3)]
        [InlineData("{3,6}", true, 3)]
        [InlineData("{5:R}", true, 5)]
        [InlineData("{ 0}", false, -1)]
        [InlineData("{-1}", false, -1)]
        [InlineData("{1.2}", false, -1)]
        [InlineData("{42r}", false, -1)]
        public void ParsePositional(string input, bool positional, int index)
        {
            var parts = TemplateParser.Parse(input);

            Assert.Equal(positional, parts.IsPositional);
            Assert.Equal(index, parts.Cast<HolePart>().First().HoleIndex);
        }

        [Theory]
        [InlineData("{hello}", "hello")]
        [InlineData("{@hello}", "hello")]
        [InlineData("{$hello}", "hello")]
        [InlineData("{#hello}", "#hello")]
        [InlineData("{  spaces  ,-3}", "  spaces  ")]
        [InlineData("{special!:G})", "special!")]
        [InlineData("{noescape_in_name}}}", "noescape_in_name")]
        [InlineData("{noescape_in_name{{}", "noescape_in_name{{")]
        [InlineData("{0}", "0")]
        [InlineData("{18 }", "18 ")]
        public void ParseName(string input, string name)
        {
            var parts = TemplateParser.Parse(input);

            Assert.Equal(name, parts.Cast<HolePart>().First().Name);
        }

        [Theory]
        [InlineData("{aaa}", HoleType.Normal)]
        [InlineData("{@a}", HoleType.Destructuring)]
        [InlineData("{@A}", HoleType.Destructuring)]
        [InlineData("{@8}", HoleType.Destructuring)]
        [InlineData("{@aaa}", HoleType.Destructuring)]
        [InlineData("{$a}", HoleType.Stringification)]
        [InlineData("{$A}", HoleType.Stringification)]
        [InlineData("{$9}", HoleType.Stringification)]
        [InlineData("{$aaa}", HoleType.Stringification)]
        public void ParseHoleType(string input, HoleType holeType)
        {
            var parts = TemplateParser.Parse(input);

            Assert.Equal(1, parts.Count);

            var holePart = parts.OfType<HolePart>().Single();
            Assert.Equal(holeType, holePart.HoleType);
        }

        [Theory]
        [InlineData(" {0,-10:nl-nl} ", -10, "nl-nl")]
        [InlineData(" {0,-10} ", -10, null)]
        [InlineData("{0,  36  }", 36, null)]
        [InlineData("{0,-36  :x}", -36, "x")]
        [InlineData(" {0:nl-nl} ", null, "nl-nl")]
        [InlineData(" {0} ", null, null)]
        public void ParseFormatAndAlignment_numeric(string input, int? aligment, string format)
        {
            var parts = TemplateParser.Parse(input);
            var hole = parts.OfType<HolePart>().Single();
            //toString will reconstuct everthing

            Assert.Equal("0", hole.Name);
            Assert.Equal(0, hole.HoleIndex);
            Assert.Equal(aligment, hole.Aligment);
            Assert.Equal(format, hole.Format);
        }

        [Theory]
        [InlineData(" {car,-10:nl-nl} ", -10, "nl-nl")]
        [InlineData(" {car,-10} ", -10, null)]
        [InlineData(" {car:nl-nl} ", null, "nl-nl")]
        [InlineData(" {car} ", null, null)]
        public void ParseFormatAndAlignment_text(string input, int? aligment, string format)
        {
            var parts = TemplateParser.Parse(input);
            var hole = parts.OfType<HolePart>().Single();
            //toString will reconstuct everthing

            Assert.Equal("car", hole.Name);
            Assert.Equal(-1, hole.HoleIndex);
            Assert.Equal(aligment, hole.Aligment);
            Assert.Equal(format, hole.Format);
        }

        [Theory]
        [InlineData("Hello {0")]
        [InlineData("Hello 0}")]
        [InlineData("Hello {a:")]
        [InlineData("Hello {a")]
        [InlineData("Hello {a,")]
        [InlineData("Hello {a,1")]
        [InlineData("{")]
        [InlineData("}")]
        [InlineData("}}}")]
        [InlineData("}}}{")]
        [InlineData("{}}{")]
        [InlineData("{a,-3.5}")]
        [InlineData("{a,2x}")]
        [InlineData("{a,--2}")]
        [InlineData("{a,-2")]
        [InlineData("{a,-2 :N0")]
        [InlineData("{a,-2.0")]
        [InlineData("{a,:N0}")]
        [InlineData("{a,}")]
        [InlineData("{6} {x}")]   // First is numeric, although not positional overall
        [InlineData("{x} {1} {a}")]   // First is numeric, although not positional overall
        public void ThrowsTemplateParserException(string input)
        {
            Assert.Throws<TemplateParserException>(() => TemplateParser.Parse(input));
        }

        [Theory]
        [InlineData(null)]
        public void ThrowsArgumentNullException(string input)
        {
            Assert.Throws<ArgumentNullException>(() => TemplateParser.Parse(input));
        }
    }
}
