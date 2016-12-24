using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            var destructer = new DefaultDestructurer();
            var actual = destructer.DestructureObject(John);
            Assert.Equal("{Name:\"John\", Age:99}", actual);
        }

        [Fact]
        public void TestUri()
        {
            var uri = new Uri("https://www.test.com?q=v");
            var destructer = new DefaultDestructurer();
            var actual = destructer.DestructureObject(uri);
            
            
#if NETSTANDARD
            string expected = "{IsImplicitFile:False, IsUncOrDosPath:False, IsDosPath:False, IsUncPath:False, HostType:HostNotParsed, DnsHostType, Syntax:System.UriParser+BuiltInUriParser, IsNotAbsoluteUri:False, AllowIdn:False, UserDrivenParsing:False, SecuredPathIndex:0, AbsolutePath:\"/\", PrivateAbsolutePath:\"/\", AbsoluteUri:\"https://www.test.com/?q=v\", LocalPath:\"/\", Authority:\"www.test.com\", HostNameType:Dns, IsDefaultPort:True, IsFile:False, IsLoopback:False, PathAndQuery:\"/?q=v\", Segments:\"/\", IsUnc:False, Host:\"www.test.com\", Port:443, Query:\"?q=v\", Fragment:\"\", Scheme:\"https\", OriginalStringSwitched:False, OriginalString:\"https://www.test.com?q=v\", DnsSafeHost:\"www.test.com\", IdnHost:\"www.test.com\", IsAbsoluteUri:True, UserEscaped:False, UserInfo:\"\", HasAuthority:True}";
#else
            string expected = "{AbsolutePath:\"/\", AbsoluteUri:\"https://www.test.com/?q=v\", LocalPath:\"/\", Authority:\"www.test.com\", HostNameType:Dns, IsDefaultPort:True, IsFile:False, IsLoopback:False, PathAndQuery:\"/?q=v\", Segments:\"/\", IsUnc:False, Host:\"www.test.com\", Port:443, Query:\"?q=v\", Fragment:\"\", Scheme:\"https\", OriginalString:\"https://www.test.com?q=v\", DnsSafeHost:\"www.test.com\", IdnHost:\"www.test.com\", IsAbsoluteUri:True, UserEscaped:False, UserInfo:\"\"}";
#endif    
            
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void TestUriCustom()
        {
            var uri = new Uri("https://www.test.com?q=v");
            var destructorManager = DestructorManager.Instance;
            destructorManager.SaveDestructurFunc<Uri>(u => u.Host);
            var actual = destructorManager.DestructureObject(uri);
            Assert.Equal("www.test.com", actual);
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
