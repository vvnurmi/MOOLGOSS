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

    public interface IProp
    {
        string Name { get; }
        Type Type { get; }
    }

    public class Property<T> : IProp
    {
        public string Name { get; private set; }
        public Type Type { get { return typeof(T); } }

        public Property(string name)
        {
            Name = name;
        }

        public override bool Equals(object obj)
        {
            var prop = obj as Property<T>;
            if (prop == null) return false;
            return prop.Name == Name && prop.Type == Type;
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode() ^ (Type.GetHashCode() * 13);
        }

        public override string ToString()
        {
            return string.Format("{0}[{1}]", Name, Type.Name);
        }
    }
}
