using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Xunit;

namespace Parser.Tests
{
   
    public class ParserTestJsonTests
    {
///<summary>Todo, read from .json file</summary>
        private string jsonTest =
            @"{
  ""Hello, world!"":  [{""text"": ""Hello, world!""}],
  ""{Name}"":         [{""property"": ""Name""}],
  ""{0}"":            [{""property"": ""0"", ""positional"": true}],
  ""Hello, {Name}!"": [{""text"": ""Hello, ""}, {""property"": ""Name""}, {""text"": ""!""}]
}";

        [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Local")]
        class TestCaseItem
        {
            public string Text { get; set; }
            public string Property { get; set; }
            public bool? Positional { get; set; }
        }

        class TestCaseItems : List<TestCaseItem>
        {

            [SuppressMessage("ReSharper", "EmptyConstructor", Justification = "needed for deser")]
            public TestCaseItems()
            {
            }

            public int TextCount => this.Count(it => it.Text != null);
            public int HoleCount => this.Count(it => it.Property != null);

        }

        [Fact]
        public void Deser()
        {
            var dict = JsonConvert.DeserializeObject<Dictionary<string, TestCaseItems>>(jsonTest);

 Assert.Equal(4, dict.Count);
            foreach (var kvp in dict)
            {

                var template = TemplateParser.Parse(kvp.Key);
                var item = kvp.Value;
               // Assert.Equal(item.Count, template.Holes.Length + template.Literals.Length);
                Assert.Equal(item.TextCount, template.Literals.Length);
                Assert.Equal(item.HoleCount, template.Holes.Length);
            }


           
        }





    }
}
