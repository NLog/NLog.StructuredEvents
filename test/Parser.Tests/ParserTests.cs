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
        [InlineData(" {0,-10:nl-nl} ", -10, "nl-nl")]
        [InlineData(" {0,-10} ", -10, null)]
        [InlineData(" {0:nl-nl} ", null, "nl-nl")]
        [InlineData(" {0} ", null, null)]
        public void ParseFormatAndAlignment_numeric(string input, string aligment, string format)
        {
            var parts = TemplateParser.Parse(input);
            var hole = parts.OfType<HolePart>().Single();
            //toString will reconstuct everthing

            Assert.Equal("0", hole.Name);
            Assert.Equal(HoleType.Numeric, hole.HoleType);
            Assert.Equal(aligment, hole.Aligment);
            Assert.Equal(format, hole.Format);
        }

        [Theory]
        [InlineData(" {car,-10:nl-nl} ", -10, "nl-nl")]
        [InlineData(" {car,-10} ", -10, null)]
        [InlineData(" {car:nl-nl} ", null, "nl-nl")]
        [InlineData(" {car} ", null, null)]
        public void ParseFormatAndAlignment_text(string input, string aligment, string format)
        {
            var parts = TemplateParser.Parse(input);
            var hole = parts.OfType<HolePart>().Single();
            //toString will reconstuct everthing

            Assert.Equal("car", hole.Name);
            Assert.Equal(HoleType.Text, hole.HoleType);
            Assert.Equal(aligment, hole.Aligment);
            Assert.Equal(format, hole.Format);
        }

        [Theory]
        [InlineData("Hello {0")]
        [InlineData("Hello 0}")]
        [InlineData("{")]
        [InlineData("}")]
        [InlineData("}}}")]
        [InlineData("}}}{")]
        [InlineData("{}}{")]
        public void ThrowException(string input)
        {
            Assert.Throws<TemplateParserException>(() => TemplateParser.Parse(input));
        }
    }
}
