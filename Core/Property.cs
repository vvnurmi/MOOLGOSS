using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public class PropertyException : Exception
    {
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class PropAttribute : Attribute
    {
    }

    public class Prop
    {
        public string Name { get; private set; }
        public Type Type { get; private set; }

        public Prop(string name, Type type)
        {
            Name = name;
            Type = type;
        }

        public override bool Equals(object obj)
        {
            var prop = obj as Prop;
            if (prop == null) return false;
            return prop.Name == Name && prop.Type == Type;
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode() ^ (Type.GetHashCode() * 13);
        }

        public override string ToString()
        {
            return string.Format("[{0}:{1}]", Name, Type.Name);
        }
    }
}
