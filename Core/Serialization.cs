using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public static class Serialization
    {
        private static BinaryFormatter g_formatter = new BinaryFormatter();

        public static byte[] Break(object obj)
        {
            using (var stream = new MemoryStream())
            {
                g_formatter.Serialize(stream, obj);
                return stream.ToArray();
            }
        }

        public static T Build<T>(byte[] data)
        {
            using (var stream = new MemoryStream(data))
                return (T)g_formatter.Deserialize(stream);
        }
    }
}
