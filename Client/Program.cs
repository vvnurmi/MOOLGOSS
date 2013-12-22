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
        public static int Main(string[] args)
        {
            var ConfigurationManager = ConfigurationManagerFactory.CreateDefault();
            using (var root = new Root("MOOLGOSS.log"))
            using (var input = new Axiom.Platforms.Win32.Win32InputReader())
            {
                root.RenderSystem = root.RenderSystems[0];
                root.RenderSystem.IsVSync = true;
                if (args.Length > 0 && !ConfigurationManager.ShowConfigDialog(root)) return 1;
                RenderWindow window = root.Initialize(true, "MOOLGOSS");
                input.Initialize(window, true, true, false, false);
                TextureManager.Instance.DefaultMipmapCount = 5;
                ResourceGroupManager.Instance.AddResourceLocation("Media", "Folder", true);
                ResourceGroupManager.Instance.InitializeAllResourceGroups();
                TryoutTerrain(root, input, window);
                //TryoutEntities(root, window);
                root.StartRendering();
            }
            return 0;
        }

        private static void TryoutTerrain(Root root, InputReader input, RenderWindow window)
        {
            var scene = Root.Instance.CreateSceneManager(SceneType.Generic);
            scene.SetSkyBox(true, "Skybox/Space", 1000);

            var plane = MeshManager.Instance.CreatePlane("ground", ResourceGroupManager.DefaultResourceGroupName,
                new Plane(Vector3.UnitY, 0), 50, 50, 5, 5, true, 1, 5, 5, Vector3.UnitZ);
            var groundEnt = scene.CreateEntity("ground entity", "ground");
            //groundEnt.MaterialName = "Examples/Rockwall";
            var node1 = scene.RootSceneNode.CreateChildSceneNode(new Vector3(100, 0, 0));
            var node2 = scene.RootSceneNode.CreateChildSceneNode(new Vector3(50, 0, 100));
            node1.AttachObject(groundEnt);
            node2.AttachObject(scene.CreateEntity("ground entity 2", "ground"));

            var camera = CreateCamera(window, scene, "camera", 0, 0, 1, 1, 10);
            camera.Near = 1;
            camera.Far = 2000;
            camera.Position = new Vector3(50, 250, 0);
            camera.LookAt(new Vector3(50, 0, 50));

            root.FrameStarted += (sender, args) =>
            {
                input.Capture();
                var angle = -0.3f * input.RelativeMouseX;
                camera.Yaw(angle);
                camera.Pitch(-0.3f * input.RelativeMouseY);
                var move = Vector3.Zero;
                if (input.IsKeyPressed(KeyCodes.W)) move -= Vector3.UnitZ;
                if (input.IsKeyPressed(KeyCodes.S)) move += Vector3.UnitZ;
                if (input.IsKeyPressed(KeyCodes.A)) move -= Vector3.UnitX;
                if (input.IsKeyPressed(KeyCodes.D)) move += Vector3.UnitX;
                camera.MoveRelative(move * 50 * args.TimeSinceLastFrame);
                if (input.IsKeyPressed(KeyCodes.Escape)) args.StopRendering = true;
            };
        }

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

            var camera1 = CreateCamera(window, scene, "cam1", 0, 0, 0.5f, 1, 10);

            Entity penguin = scene.CreateEntity("bob", "penguin.mesh");
            SceneNode penguinNode = scene.RootSceneNode.CreateChildSceneNode();
            penguinNode.AttachObject(penguin);

            var ninja = scene.CreateEntity("ninja", "ninja.mesh");
            ninja.CastShadows = true;
            scene.RootSceneNode.CreateChildSceneNode().AttachObject(ninja);
            ninja.ParentNode.Translate(new Vector3(50, 0, -50));
            penguin.CastShadows = true;

            var foo = MeshManager.Instance.CreatePlane("ground", ResourceGroupManager.DefaultResourceGroupName,
                new Plane(Vector3.UnitY, 0), 1500, 1500, 20, 20, true, 1, 5, 5, Vector3.UnitZ);
            var groundEnt = scene.CreateEntity("ground entity", "ground");
            groundEnt.MaterialName = "Examples/Rockwall";
            groundEnt.CastShadows = false;
            scene.RootSceneNode.AttachObject(groundEnt);

            camera1.Move(new Vector3(0, 50, 300));
            camera1.LookAt(penguin.BoundingBox.Center);
        }

        private static Camera CreateCamera(RenderWindow window, SceneManager scene, string name,
            float left, float top, float width, float height, int z)
        {
            var camera = scene.CreateCamera(name);
            var viewport1 = window.AddViewport(camera, left, top, width, height, z);
            viewport1.BackgroundColor = ColorEx.CornflowerBlue;
            return camera;
        }

        private static void Connect()
        {
            var service = Marshal.Get<IService>(Invoke);
            var planets = service.GetPlanets();
            Console.WriteLine("Planets:");
            foreach (var planet in planets)
                Console.WriteLine("  {0}", planet.Name);
            Console.WriteLine("\nPress Enter to finish");
            Console.ReadLine();
        }

        private static object Invoke(string name, object[] args)
        {
            var data = Serialization.Break(Tuple.Create(name, args));
            var request = WebRequest.Create("http://localhost:8080/moolgoss/");
            request.Method = "POST";
            request.ContentLength = data.Length;
            using (var requestStream = request.GetRequestStream())
                requestStream.Write(data, 0, data.Length);
            using (var response = request.GetResponse())
            {
                var responseData = new byte[response.ContentLength];
                response.GetResponseStream().Read(responseData, 0, responseData.Length);
                return Serialization.Build<object>(responseData);
            }
        }
    }
}
