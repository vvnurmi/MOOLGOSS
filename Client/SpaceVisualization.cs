using Axiom.Core;
using Axiom.Math;
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

        public void Create()
        {
            Globals.Scene.SetSkyBox(true, "Skybox/Space", 1000);
            CreatePlanet(new Vector3(100, 0, 0));
            CreatePlanet(new Vector3(150, 0, 100));
            CreatePlanet(new Vector3(200, 0, 50));
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
