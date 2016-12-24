using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Xunit.Extensions;

namespace Parser.Tests
{
    public class DestructureTests
    {
        public static Person John = new Person("John", 99);

        //// public IEnumerable<object RenderTestData

        //[Theory]
        //[MemberData(nameof(John))]
        ////[InlineData("{0}", new object[] {"a"}, "a")]

        //public void RenderTest(string input, object[] args, string expected)
        //{

        //}

        [Fact]
        public void TestJohn()
        {
            var destructer = new Destructurer();
            var actual = destructer.DestructureObject(John);
            Assert.Equal("{Name:\"John\", Age:99}", actual);
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
