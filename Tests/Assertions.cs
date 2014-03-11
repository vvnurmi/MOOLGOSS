using Core;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{
    internal static class Assertions
    {
        public static void WorldsEqual(World expected, World actual)
        {
            var diff = new WorldDiff(expected, actual);
            var message = new StringBuilder("Worlds differ");
            AppendSeqDiff(message, ", Wobs", diff.Wobs);
            Assert.True(diff.IsEmpty, message.ToString());
        }

        private static void AppendSeqDiff<T>(StringBuilder str, string name, Diff<T> seqDiff) where T : IEquatable<T>
        {
            str.AppendFormat("{0} +{1} -{2}", name, seqDiff.Added.Count, seqDiff.Removed.Count);
        }
    }
}
