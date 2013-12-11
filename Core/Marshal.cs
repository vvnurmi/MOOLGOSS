using ImpromptuInterface;
using Microsoft.CSharp.RuntimeBinder;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public interface IMarshalled
    {
        object Invoke(string name, params object[] args);
    }

    public class Marshalled : DynamicObject, IMarshalled
    {
        private object _target;
        private Func<string, object[], object> _invoke;

        internal Marshalled(object target)
        {
            Debug.Assert(target != null);
            _target = target;
        }

        public Marshalled(Func<string, object[], object> invoke)
        {
            Debug.Assert(invoke != null);
            _invoke = invoke;
        }

        public object Invoke(string name, params object[] args)
        {
            if (_target != null)
                try
                {
                    return Impromptu.InvokeMember(_target, name, args);
                }
                catch (RuntimeBinderException)
                {
                    Impromptu.InvokeMemberAction(_target, name, args);
                    return null;
                }
            else
                return _invoke(name, args);
        }

        public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
        {
            try
            {
                if (_target != null)
                    result = Impromptu.InvokeMember(_target, binder.Name, args);
                else
                    result = _invoke(binder.Name, args);
                return true;
            }
            catch (Exception)
            {
                result = null;
                return false;
            }
        }
    }

    public static class Marshal
    {
        public static IMarshalled Get(object obj)
        {
            dynamic marshalled = new Marshalled(obj);
            return Impromptu.ActLike<IMarshalled>(marshalled);
        }

        public static T Get<T>(Func<string, object[], object> invoke) where T : class
        {
            dynamic marshalled = new Marshalled(invoke);
            return Impromptu.ActLike<T>(marshalled, typeof(IMarshalled));
        }
    }
}
