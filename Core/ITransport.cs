using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public interface ITransport
    {
        object Invoke(string method, params object[] args);
        void InvokeAction(string method, params object[] args);
    }
}
