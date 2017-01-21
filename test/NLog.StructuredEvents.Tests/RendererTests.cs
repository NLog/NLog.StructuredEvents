using System;
using System.Globalization;
using Xunit;

namespace NLog.StructuredEvents.Tests
{
    public class RendererTests
    {


        [Theory]
        [InlineData("{0}", new object[] { "a" }, "a")]
        [InlineData(" {0}", new object[] { "a" }, " a")]
        [InlineData(" {0} ", new object[] { "a" }, " a ")]
        [InlineData(" {0} {1} ", new object[] { "a", "b" }, " a b ")]
        [InlineData(" {1} {0} ", new object[] { "a", "b" }, " b a ")]
        [InlineData(" {1} {0} {0}", new object[] { "a", "b" }, " b a a")]
        [InlineData(" message {1} {0} {0}", new object[] { "a", "b" }, " message b a a")]
        [InlineData(" message {1} {0} {0}", new object[] { 'a', 'b' }, " message b a a")]
        [InlineData("char {one}", new object[] { 'X' }, "char \"X\"")]
        [InlineData("char {one:l}", new object[] { 'X' }, "char X")]
        [InlineData(" message {{{1}}} {0} {0}", new object[] { "a", "b" }, " message {b} a a")]
        [InlineData(" message {{{one}}} {two} {three}", new object[] { "a", "b", "c" }, " message {\"a\"} \"b\" \"c\"")]
        [InlineData(" message {{{1} {0} {0}}}", new object[] { "a", "b" }, " message {b a a}")]
        [InlineData(" completed in {time} sec", new object[] { 10 }, " completed in 10 sec")]
        [InlineData(" completed task {name} in {time} sec", new object[] { "test", 10 }, " completed task \"test\" in 10 sec")]
        [InlineData(" completed task {name:l} in {time} sec", new object[] { "test", 10 }, " completed task test in 10 sec")]
        [InlineData(" completed task {0} in {1} sec", new object[] { "test", 10 }, " completed task test in 10 sec")]
        [InlineData(" completed task {0} in {1:000} sec", new object[] { "test", 10 }, " completed task test in 010 sec")]
        [InlineData(" completed task {name} in {time:000} sec", new object[] { "test", 10 }, " completed task \"test\" in 010 sec")]
        [InlineData(" completed tasks {tasks} in {time:000} sec", new object[] { new [] { "parsing", "testing", "fixing"}, 10 }, " completed tasks \"parsing\", \"testing\", \"fixing\" in 010 sec")]
        [InlineData(" completed tasks {tasks:l} in {time:000} sec", new object[] { new [] { "parsing", "testing", "fixing"}, 10 }, " completed tasks parsing, testing, fixing in 010 sec")]
        [InlineData(" completed tasks {$tasks} in {time:000} sec", new object[] { new [] { "parsing", "testing", "fixing"}, 10 }, " completed tasks \"System.String[]\" in 010 sec")]
        [InlineData(" completed tasks {0} in {1:000} sec", new object[] { new [] { "parsing", "testing", "fixing"}, 10 }, " completed tasks System.String[] in 010 sec")]
        [InlineData("{{{0:d}}}", new object[] { 3 }, "{d}")] //format is here "d}" ... because escape from left-to-right 
        [InlineData("{{{0:d} }}", new object[] { 3 }, "{3 }")]
        [InlineData("{{{0:dd}}}", new object[] { 3 }, "{dd}")]
        [InlineData("{{{0:0{{}", new object[] { 3 }, "{3{")] //format is here "0{"
        [InlineData("hello {0}", new object[] { null }, "hello NULL")]
       
        public void RenderTest(string input, object[] args, string expected)
        {
            var culture = CultureInfo.InvariantCulture;

            RenderAndTest(input, culture, args, expected);
        }

        [Theory]
        [InlineData("test {0}", "nl", new object[] { 12.3 }, "test 12,3")]
        [InlineData("test {0}", "en-gb", new object[] { 12.3 }, "test 12.3")]
        public void RenderCulture(string input, string language, object[] args, string expected)
        {
            var culture = new CultureInfo(language);

            RenderAndTest(input, culture, args, expected);
        }

        private static void RenderAndTest(string input, CultureInfo culture, object[] args, string expected)
        {
            var template = TemplateParser.Parse(input);

            var result = template.Render(culture, args);
            Assert.Equal(expected, result);
        }
    }
}