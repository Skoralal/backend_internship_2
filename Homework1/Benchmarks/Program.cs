using System.Text;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Order;
using BenchmarkDotNet.Running;
/*
 Метод с интернированными строками надёжно лидирует на около 15-20%
при равном потреблении памяти по сравнению с методом, использующим константные строки,
также у оптимизированного метода меньшая вариация по времени выполнения.
Оптимизированный метод тратит одинкаовое время на поиск слова будь оно в начале, конце или не в списке,
 */

BenchmarkRunner.Run<StringInternBenchmark>();

[MemoryDiagnoser(displayGenColumns: true)]
[Orderer(SummaryOrderPolicy.FastestToSlowest)]
[RankColumn]
public class StringInternBenchmark
{
    private readonly List<string> _words = new();
    public StringInternBenchmark()
    {
       foreach (var word in File.ReadLines(@".\SpellingDictionaries\ru_RU.dic"))
           _words.Add(string.Intern(word));
    }

    [Benchmark(Baseline = true)]
    [ArgumentsSource(nameof(SampleData))]
    public bool WordIsExists(string word)
        => _words.Any(item => word.Equals(item, StringComparison.Ordinal));

    [Benchmark]
    [ArgumentsSource(nameof(SampleData))]
    public bool WordIsExistsIntern(string word)
    {
        var internedWord = string.Intern(word);
        return _words.Any(item => ReferenceEquals(internedWord, item));
    }

    public IEnumerable<string> SampleData()
    {
        List<string> words = new List<string>();
        words.Add("анекдотцем");
        words.Add(new StringBuilder("анек"+"дот").ToString());
        words.Add("проём");
        words.Add(new StringBuilder("пр" + "о").ToString());
        words.Add("Стёпка");
        words.Add(new StringBuilder("Стал" + "ин").ToString());
        words.Add("каво");
        words.Add(new StringBuilder("Бер" + "ия").ToString());
        foreach (var word in words)
        {
            yield return word;
        }
    }
}