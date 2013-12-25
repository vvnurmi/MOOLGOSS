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

        public void Create(IEnumerable<Planet> planets)
        {
            Globals.Scene.AmbientLight = new ColorEx(0.1f, 0.1f, 0.1f);
            //Globals.Scene.ShadowTechnique = ShadowTechnique.StencilAdditive;

            var light = Globals.Scene.CreateLight("light");
            light.Type = LightType.Directional;
            light.Direction = new Vector3(1, 0.2f, 1);
            light.Diffuse = new ColorEx(1, 1, 1);

            Globals.Scene.SetSkyBox(true, "Skybox/Space", 1000);
            var z = 0f;
            foreach (var planet in planets)
                CreatePlanet(new Vector3(75 * planet.Name.Length, 0, z += 75));
        }

        private void CreatePlanet(Vector3 pos)
        {
            var groundEnt = Globals.Scene.CreateEntity("ground entity " + _entityIndex++, "planet1.mesh");
            groundEnt.CastShadows = true;
            var node = Globals.Scene.RootSceneNode.CreateChildSceneNode(pos);
            node.AttachObject(groundEnt);
        }
    }
}
