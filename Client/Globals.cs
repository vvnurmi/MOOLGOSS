﻿using Axiom.Core;
using Axiom.Input;
using Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    internal static class Globals
    {
        public static SceneManager Scene { get; set; }
        public static Camera Camera { get; set; }
        public static Input Input { get; set; }
        public static UserInterface UI { get; set; }
        public static Guid PlayerShipID { get; set; }
        public static float TotalTime { get; set; }
    }
}
