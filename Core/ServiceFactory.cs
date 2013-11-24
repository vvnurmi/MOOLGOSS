using ImpromptuInterface;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public class ServiceFactory
    {
        private class Proxy : DynamicObject
        {
            private ITransport _transport;

            public Proxy(ITransport transport)
            {
                _transport = transport;
            }

            public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
            {
                result = _transport.Invoke(binder.Name, args);
                return true;
            }
        }

        private ITransport _transport;

        public ServiceFactory(ITransport transport)
        {
            _transport = transport;
        }

        public T GetProxy<T>() where T : class
        {
            return new Proxy(_transport).ActLike<T>();
        }
    }
}
