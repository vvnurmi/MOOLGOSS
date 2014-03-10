using System;

namespace Client
{
    internal class Atom<T>
    {
        public static implicit operator T(Atom<T> x) { return x.Value; }
        public Atom(T value) { Value = value; }
        public T Value { get; private set; }
        public void Set(Func<T, T> f) { lock (Value) Value = f(Value); }
    }
}
