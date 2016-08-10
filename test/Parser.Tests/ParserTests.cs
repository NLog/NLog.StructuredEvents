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
        [InlineData(" {@stringify} ")] 
        public void ParseAndPrint(string input)
        {
            var parser = new TemplateParser();
            var parts = parser.Parse(input);

            //toString will reconstuct everthing
            var printed = parts.Print();
            Assert.Equal(input, printed);

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
            var parser = new TemplateParser();
            Assert.Throws<Exception>(() => parser.Parse(input));
        }

    }
}
