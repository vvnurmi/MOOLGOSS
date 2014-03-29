using Axiom.Core;
using Axiom.Graphics;
using Axiom.Math;
using Core;
using Core.Wobs;
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
        private class IDComparer : IEqualityComparer<Wob>
        {
            public bool Equals(Wob x, Wob y)
            {
                return x.ID == y.ID;
            }

            public int GetHashCode(Wob wob) { return wob.ID.GetHashCode(); }
        }
        private static IDComparer g_idComparer = new IDComparer();

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

        public void Update()
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

        public void Update(WorldDiff diff, float updateInterval)
        {
            foreach (var planet in diff.Wobs.Removed.Values.OfType<Planet>()) RemovePlanet(planet);
            foreach (var planet in diff.Wobs.Added.Values.OfType<Planet>()) CreatePlanet(planet);
            foreach (var station in diff.Wobs.Removed.Values.OfType<Station>()) RemoveStation(station);
            foreach (var station in diff.Wobs.Added.Values.OfType<Station>()) CreateStation(station);
            Func<Wob, bool> isVessel = w => w is Ship || w is Droid;
            var vesselsAdded = diff.Wobs.Added.Values.Where(isVessel).ToArray();
            var vesselsRemoved = diff.Wobs.Removed.Values.Where(isVessel).ToArray();
            var playerShipID = Globals.World.Value.GetPlayerShipID(Globals.PlayerID);
            foreach (var vessel in vesselsAdded)
                if (vessel.ID != playerShipID) UpdateVessel(vessel, 1);
            foreach (var vessel in vesselsRemoved.Except(vesselsAdded, g_idComparer))
                if (vessel.ID != playerShipID) RemoveVessel(vessel);
        }

        public void CreateStaticThings()
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
        }

        public void UpdateVessel(Wob vessel, float updateInterval)
        {
            Debug.Assert(updateInterval >= 0);
            var posed = (IPosed)vessel;
            SceneNode node;
            if (!_nodes.TryGetValue(vessel.ID, out node))
            {
                _nodes.Add(vessel.ID, node = CreateVessel(posed));
            }
            _nodeUpdates[vessel.ID] = new NodeUpdate
            {
                Start = new NodeState
                {
                    Pos = node.Position,
                    Orientation = node.Orientation,
                    Time = Globals.TotalTime,
                },
                End = new NodeState
                {
                    Pos = posed.Pose.Location,
                    Orientation = posed.Pose.Orientation,
                    Time = Globals.TotalTime + updateInterval,
                },
            };
        }

        private void CreatePlanet(Planet planet)
        {
            var groundEnt = Globals.Scene.CreateEntity("ground entity " + _entityIndex++, "planet1.mesh");
            groundEnt.CastShadows = true;
            var node = Globals.Scene.RootSceneNode.CreateChildSceneNode(planet.Pos);
            node.AttachObject(groundEnt);
        }

        private void RemovePlanet(Planet planet)
        {
            throw new NotImplementedException();
        }

        private void CreateStation(Station station)
        {
            var stationEnt = Globals.Scene.CreateEntity("station entity " + _entityIndex++, "station1.mesh");
            stationEnt.CastShadows = true;
            var node = Globals.Scene.RootSceneNode.CreateChildSceneNode(station.Pos);
            node.AttachObject(stationEnt);
        }

        private void RemoveStation(Station station)
        {
            throw new NotImplementedException();
        }

        private SceneNode CreateVessel(IPosed wob)
        {
            var mesh =
                wob is Ship ? "ship1.mesh" :
                wob is Droid ? "droid.mesh" :
                null;
            Debug.Assert(mesh != null);
            var shipEnt = Globals.Scene.CreateEntity("ship entity " + _entityIndex++, mesh);
            var node = Globals.Scene.RootSceneNode.CreateChildSceneNode();
            node.AttachObject(shipEnt);
            node.Position = wob.Pose.Location;
            node.Orientation = wob.Pose.Orientation;
            return node;
        }

        private void RemoveVessel(Wob vessel)
        {
            SceneNode node;
            if (!_nodes.TryGetValue(vessel.ID, out node)) return;
            Globals.Scene.RootSceneNode.RemoveChild(node);
            _nodes.Remove(vessel.ID);
        }
    }
}
