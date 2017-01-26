using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using NLog.StructuredEvents;

public class Program 
{
    public static void Main()
    {
        BenchmarkRunner.Run<Program>();
    }

    [Params("Whatever {subject} you want to benchmark in {time}ms.",
            "{time:yyyy-MM-dd HH:mm:ss} {severity} {message}.")]
    public string Text { get; set; }

    [Benchmark]
    public Template Parse() => TemplateParser.Parse(Text);
}