﻿using MOO.Client.MOOService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace MOO.Client
{
    [CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Single, UseSynchronizationContext = false)]
    public class MOOCallbackHandler : IMOOServiceCallback
    {
        private State _state;

        public MOOCallbackHandler(State state)
        {
            _state = state;
        }

        public void Update(DateTime now)
        {
            _state.Now = now;
        }
    }
}
