using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Xunit.Extensions;

namespace Parser.Tests
{
    public class SerializationTests
    {
        /// <summary>Initializes a new instance of the <see cref="T:System.Object" /> class.</summary>
        public SerializationTests()
        {
            SerializationManager.DefaultSerializer = JsonNetSerializer.Instance;
        }


        public static Person John = new Person("John", 99);


        [Fact]
        public void TestJohn()
        {
            var actual = SerializationManager.Instance.SerializeObject(John);
            Assert.Equal("{\"Name\":\"John\",\"Age\":99}", actual);
        }

        [Fact]
        public void TestRenderJohn()
        {
            var template = TemplateParser.Parse("Hello {@Person}");

            var result = template.Render(CultureInfo.InvariantCulture, new object[] { John });
            Assert.Equal("Hello {\"Name\":\"John\",\"Age\":99}", result);
        }

        [Fact]
        public void TestUriCustom()
        {
            var uri = new Uri("https://www.test.com?q=v");
            var serializationManager = SerializationManager.Instance;
            serializationManager.SaveSerializerFunc<Uri>((u, f) => u.Host);
            var actual = serializationManager.SerializeObject(uri);
            Assert.Equal("www.test.com", actual);
        }

        [Fact]
        public void TestNullSerializer()
        {
            try
            {
                SerializationManager.DefaultSerializer = null;
                Assert.Throws<RenderException>(() => SerializationManager.Instance.SerializeObject(John));
            }
            finally
            {
                SerializationManager.DefaultSerializer = JsonNetSerializer.Instance;
            }
        }

        [Fact]
        public void TestNullFuncSerializer()
        {
            Assert.Throws<ArgumentNullException>(() => new FuncSerializer<int>(null));
        }

        public class Person
        {
            /// <summary>Initializes a new instance of the <see cref="T:System.Object" /> class.</summary>
            public Person(string name, int age)
            {
                Name = name;
                Age = age;
            }

            public string Name { get; set; }
            public int Age { get; set; }


        }

    }
}
