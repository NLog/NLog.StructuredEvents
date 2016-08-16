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
        [InlineData(" {alignment,-10} ")]
        [InlineData(" {alignment,10} ")]
        [InlineData(" {0,10} ")]
        [InlineData(" {0,-10} ")]
        [InlineData(" {0,-10:test} ")]
        [InlineData("{{{0:d}}}")]
        [InlineData("{{{0:0{{}")]
        public void ParseAndPrint(string input)
        {
            var template = TemplateParser.Parse(input);

            Assert.Equal(input, template.Rebuild());
        }

        [Theory]
        [InlineData("{0}", 0)]
        [InlineData("{001}", 1)]
        [InlineData("{9}", 9)]
        [InlineData("{1 }", 1)]
        [InlineData("{1} {2}", 1)]
        [InlineData("{@3} {$4}", 3)]
        [InlineData("{3,6}", 3)]
        [InlineData("{5:R}", 5)]        
        public void ParsePositional(string input, int index)
        {
            var template = TemplateParser.Parse(input);

            Assert.True(template.IsPositional);
            Assert.Equal(index, template.Holes[0].Index);
        }

        [Theory]
        [InlineData("{ 0}")]
        [InlineData("{-1}")]
        [InlineData("{1.2}")]
        [InlineData("{42r}")]
        [InlineData("{6} {x}")]
        [InlineData("{a} {x}")]
        public void ParseNominal(string input)
        {
            var template = TemplateParser.Parse(input);

            Assert.False(template.IsPositional);
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
            var template = TemplateParser.Parse(input);

            Assert.Equal(name, template.Holes[0].Name);
        }

        [Theory]
        [InlineData("{aaa}", CaptureType.Normal)]
        [InlineData("{@a}", CaptureType.Destructuring)]
        [InlineData("{@A}", CaptureType.Destructuring)]
        [InlineData("{@8}", CaptureType.Destructuring)]
        [InlineData("{@aaa}", CaptureType.Destructuring)]
        [InlineData("{$a}", CaptureType.Stringification)]
        [InlineData("{$A}", CaptureType.Stringification)]
        [InlineData("{$9}", CaptureType.Stringification)]
        [InlineData("{$aaa}", CaptureType.Stringification)]
        public void ParseHoleType(string input, CaptureType holeType)
        {
            var template = TemplateParser.Parse(input);

            Assert.Equal(1, template.Holes.Length);
            Assert.Equal(holeType, template.Holes[0].CaptureType);
        }

        [Theory]
        [InlineData(" {0,-10:nl-nl} ", -10, "nl-nl")]
        [InlineData(" {0,-10} ", -10, null)]
        [InlineData("{0,  36  }", 36, null)]
        [InlineData("{0,-36  :x}", -36, "x")]
        [InlineData(" {0:nl-nl} ", 0, "nl-nl")]
        [InlineData(" {0} ", 0, null)]
        public void ParseFormatAndAlignment_numeric(string input, int aligment, string format)
        {
            var templates = TemplateParser.Parse(input);
            var hole = templates.Holes[0];

            Assert.Equal("0", hole.Name);
            Assert.Equal(0, hole.Index);
            Assert.Equal(aligment, hole.Alignment);
            Assert.Equal(format, hole.Format);
        }

        [Theory]
        [InlineData(" {car,-10:nl-nl} ", -10, "nl-nl")]
        [InlineData(" {car,-10} ", -10, null)]
        [InlineData(" {car:nl-nl} ", 0, "nl-nl")]
        [InlineData(" {car} ", 0, null)]
        public void ParseFormatAndAlignment_text(string input, int aligment, string format)
        {
            var template = TemplateParser.Parse(input);
            var hole = template.Holes[0];

            Assert.False(template.IsPositional);
            Assert.Equal("car", hole.Name);
            Assert.Equal(aligment, hole.Alignment);
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
        [InlineData("{a,{}")]        
        [InlineData("{a,d{e}")]        
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
