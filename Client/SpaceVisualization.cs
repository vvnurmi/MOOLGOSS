using Axiom.Core;
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
        private Mesh _planetMesh;
        private int _entityIndex;

        public SpaceVisualization()
        {
            _planetMesh = MeshManager.Instance.CreatePlane("ground", ResourceGroupManager.DefaultResourceGroupName,
                new Plane(Vector3.UnitY, 0), 50, 50, 5, 5, true, 1, 5, 5, Vector3.UnitZ);
        }

        public void Create(IEnumerable<Planet> planets)
        {
            Globals.Scene.SetSkyBox(true, "Skybox/Space", 1000);
            var z = 0f;
            foreach (var planet in planets)
                CreatePlanet(new Vector3(50 * planet.Name.Length, 0, 50 * z++));
        }

        private void CreatePlanet(Vector3 pos)
        {
            var groundEnt = Globals.Scene.CreateEntity("ground entity " + _entityIndex++, "ground");
            //groundEnt.MaterialName = "Examples/Rockwall";
            var node = Globals.Scene.RootSceneNode.CreateChildSceneNode(pos);
            node.AttachObject(groundEnt);
        }
    }
}
