using Core;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{
    [TestFixture]
    public class DictionaryValueEqualityTest
    {
        [Test]
        public void EmptysAreEqual()
        {
            AssertDictionaryValueEquality(true, new int[0], new int[0]);
        }

        [Test]
        public void SameKeysAndValuesAreEqual()
        {
            AssertDictionaryValueEquality(true, new[] { 1, 42, 2, 69, 9, 99 }, new[] { 2, 69, 9, 99, 1, 42 });
        }

        [Test]
        public void DifferentSizesNotEqual()
        {
            AssertDictionaryValueEquality(false, new int[0], new[] { 1, 1 });
            AssertDictionaryValueEquality(false, new[] { 1, 1, 2, 2 }, new[] { 1, 1 });
            AssertDictionaryValueEquality(false, new[] { 1, 1 }, new[] { 1, 1, 2, 2 });
        }

        [Test]
        public void DifferentKeysNotEqual()
        {
            AssertDictionaryValueEquality(false, new[] { 1, 1 }, new[] { 9, 1 });
        }

        [Test]
        public void DifferentValuesNotEqual()
        {
            AssertDictionaryValueEquality(false, new[] { 1, 1 }, new[] { 1, 2 });
        }

        [Test]
        public void TimeLargeEquals()
        {
            var size = 1000000;
            // Note: Building the dictionary from an ordered sequence of ints takes .NET 4.5 a really long of time.
            var keysValues = Enumerable.Range(0, size * 2).Select(n => n * 479001599).ToArray();
            var a = BuildDictionary(keysValues);
            var b = BuildDictionary(keysValues);
            var start = DateTime.Now;
            Assert.True(a.ValueEquals(b));
            var finish = DateTime.Now;
            Console.WriteLine("Value equality for identical dictionaries of {0} keys took {1} seconds",
                size, (finish - start).TotalSeconds);
        }

        private void AssertDictionaryValueEquality(bool expected, int[] keysValues1, int[] keysValues2)
        {
            Assert.AreEqual(expected, BuildDictionary(keysValues1).ValueEquals(BuildDictionary(keysValues2)));
        }

        private Dictionary<int, int> BuildDictionary(int[] keysValues)
        {
            if (keysValues.Length % 2 != 0) throw new ArgumentException();
            var dic = new Dictionary<int, int>();
            for (int i = 0; i < keysValues.Length; i += 2)
                dic.Add(keysValues[i], keysValues[i + 1]);
            return dic;
        }
    }
}
