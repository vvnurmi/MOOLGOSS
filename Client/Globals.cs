using Axiom.Core;
using Axiom.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    public static class Globals
    {
        public static SceneManager Scene { get; set; }
        public static Camera Camera { get; set; }
        public static InputReader Input { get; set; }
    }
}
