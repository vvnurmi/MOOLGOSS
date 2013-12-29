using Axiom.Math;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace Core.Serial
{
    public static class Serialization
    {
        private static StreamingContext g_allContexts = new StreamingContext(StreamingContextStates.All);
        private static Lazy<BinaryFormatter> g_formatter = new Lazy<BinaryFormatter>(() =>
        {
            var selector = new SurrogateSelector();
            selector.AddSurrogate(typeof(Vector3), g_allContexts, new Vector3Surrogate());
            return new BinaryFormatter(selector, new StreamingContext(StreamingContextStates.CrossMachine));
        });
        private static BinaryFormatter Formatter { get { return g_formatter.Value; } }

        public static byte[] Break(object obj)
        {
            using (var stream = new MemoryStream())
            {
                Formatter.Serialize(stream, obj);
                return stream.ToArray();
            }
        }

        public static T Build<T>(byte[] data)
        {
            using (var stream = new MemoryStream(data))
                return (T)Formatter.Deserialize(stream);
        }
    }
}
