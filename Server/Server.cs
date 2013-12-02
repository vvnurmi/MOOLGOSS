using Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public class Server
    {
        private Service _service = new Service();
        private ITransport _transport;

        public Server(ITransport transport)
        {
            _transport = transport;
        }
    }
}
