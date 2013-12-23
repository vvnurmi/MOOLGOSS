using Axiom.Core;
using Axiom.Framework.Configuration;
using Axiom.Graphics;
using Axiom.Input;
using Axiom.Math;
using Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var client = new Client();
            client.Start(args.Length > 0, "assaultwing.com");
        }

        // TODO: Remove
        private static void TryoutEntities(Root root, RenderWindow window)
        {
            SceneManager scene = root.CreateSceneManager(SceneType.Generic);
            scene.AmbientLight = new ColorEx(0.5f, 0.5f, 0.5f);
            scene.ShadowTechnique = ShadowTechnique.StencilAdditive;

            var light = scene.CreateLight("spot");
            light.Type = LightType.Spotlight;
            light.Direction = new Vector3(0, -1, -1);
            light.SetSpotlightRange(30, 60);
            light.Position = new Vector3(0, 300, 300);
            light.Diffuse = new ColorEx(0, 1, 0);
            light.Specular = new ColorEx(0, 1, 0);

            Entity penguin = scene.CreateEntity("bob", "penguin.mesh");
            SceneNode penguinNode = scene.RootSceneNode.CreateChildSceneNode();
            penguinNode.AttachObject(penguin);

            var ninja = scene.CreateEntity("ninja", "ninja.mesh");
            ninja.CastShadows = true;
            scene.RootSceneNode.CreateChildSceneNode().AttachObject(ninja);
            ninja.ParentNode.Translate(new Vector3(50, 0, -50));
            penguin.CastShadows = true;
        }
    }
}
