using Axiom.Core;
using Axiom.Graphics;
using Axiom.Math;
using Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    public class SpaceVisualization
    {
        private int _entityIndex;
        private Dictionary<Guid, SceneNode> _nodes = new Dictionary<Guid, SceneNode>();

        public void Create(IEnumerable<Planet> planets)
        {
            Globals.Scene.AmbientLight = new ColorEx(0.1f, 0.1f, 0.1f);
            //Globals.Scene.ShadowTechnique = ShadowTechnique.StencilAdditive;

            var light1 = Globals.Scene.CreateLight("light 1");
            light1.Type = LightType.Directional;
            light1.Direction = new Vector3(1, 0.2f, 1);
            light1.Diffuse = new ColorEx(1, 1, 1);
            var light2 = Globals.Scene.CreateLight("light 2");
            light2.Type = LightType.Directional;
            light2.Direction = new Vector3(-1, 0.2f, -1);
            light2.Diffuse = new ColorEx(0.2f, 0.2f, 0.2f);

            Globals.Scene.SetSkyBox(true, "Skybox/Space", 1000);
            var z = 0f;
            foreach (var planet in planets)
                CreatePlanet(new Vector3(75 * planet.Name.Length, 0, z += 75));
        }

        public void UpdateShip(Ship ship)
        {
            SceneNode node;
            if (!_nodes.TryGetValue(ship.ID, out node))
                _nodes.Add(ship.ID, node = CreateShip());
            node.Position = ship.Pos;
            node.Orientation = ship.Orientation;
        }

        private void CreatePlanet(Vector3 pos)
        {
            var groundEnt = Globals.Scene.CreateEntity("ground entity " + _entityIndex++, "planet1.mesh");
            groundEnt.CastShadows = true;
            var node = Globals.Scene.RootSceneNode.CreateChildSceneNode(pos);
            node.AttachObject(groundEnt);
        }

        private SceneNode CreateShip()
        {
            var shipEnt = Globals.Scene.CreateEntity("ship entity " + _entityIndex++, "ship1.mesh");
            var node = Globals.Scene.RootSceneNode.CreateChildSceneNode();
            node.AttachObject(shipEnt);
            return node;
        }
    }
}
