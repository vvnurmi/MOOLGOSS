using Axiom.Core;
using Axiom.Graphics;
using Axiom.Math;
using Core;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    public class SpaceVisualization
    {
        private struct NodeState
        {
            public Vector3 Pos;
            public Quaternion Orientation;
            public float Time;
        }

        private struct NodeUpdate
        {
            public NodeState Start;
            public NodeState End;
        }

        private int _entityIndex;
        private Dictionary<Guid, SceneNode> _nodes = new Dictionary<Guid, SceneNode>();
        private ConcurrentDictionary<Guid, NodeUpdate> _nodeUpdates = new ConcurrentDictionary<Guid, NodeUpdate>();

        public void FrameStartHandler(object sender, FrameEventArgs args)
        {
            foreach (var update in _nodeUpdates)
            {
                var start = update.Value.Start;
                var end = update.Value.End;
                var t = end.Time > start.Time
                    ? Math.Min(1, (Globals.TotalTime - start.Time) / (end.Time - start.Time))
                    : 1;
                var node = _nodes[update.Key];
                node.Position = t * end.Pos + (1 - t) * start.Pos;
                node.Orientation = Util.SlerpShortest(t, start.Orientation, end.Orientation);
            }
            foreach (var oldTarget in _nodeUpdates.Where(x => x.Value.End.Time <= Globals.TotalTime).ToArray())
            {
                NodeUpdate _;
                _nodeUpdates.TryRemove(oldTarget.Key, out _);
            }
        }

        public void Create(IEnumerable<Planet> planets, IEnumerable<Station> stations)
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
            foreach (var station in stations)
                CreateStation(station.Pos);
        }

        public void UpdateShip(Ship ship, float updateInterval)
        {
            Debug.Assert(updateInterval >= 0);
            SceneNode node;
            if (!_nodes.TryGetValue(ship.ID, out node))
            {
                _nodes.Add(ship.ID, node = CreateShip());
                node.Position = ship.Pos;
                node.Orientation = ship.Orientation;
            }
            _nodeUpdates[ship.ID] = new NodeUpdate
            {
                Start = new NodeState
                {
                    Pos = node.Position,
                    Orientation = node.Orientation,
                    Time = Globals.TotalTime,
                },
                End = new NodeState
                {
                    Pos = ship.Pos,
                    Orientation = ship.Orientation,
                    Time = Globals.TotalTime + updateInterval,
                },
            };
        }

        private void CreatePlanet(Vector3 pos)
        {
            var groundEnt = Globals.Scene.CreateEntity("ground entity " + _entityIndex++, "planet1.mesh");
            groundEnt.CastShadows = true;
            var node = Globals.Scene.RootSceneNode.CreateChildSceneNode(pos);
            node.AttachObject(groundEnt);
        }

        private void CreateStation(Vector3 pos)
        {
            var stationEnt = Globals.Scene.CreateEntity("station entity " + _entityIndex++, "station1.mesh");
            stationEnt.CastShadows = true;
            var node = Globals.Scene.RootSceneNode.CreateChildSceneNode(pos);
            node.AttachObject(stationEnt);
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
