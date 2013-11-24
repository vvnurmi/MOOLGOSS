using ImpromptuInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public class InstanceTransport : ITransport
    {
        private dynamic _target;

        public InstanceTransport(object target)
        {
            _target = target;
        }

        public object Invoke(string method, params object[] args)
        {
            return Impromptu.InvokeMember(_target, method, args);
        }
    }
}
