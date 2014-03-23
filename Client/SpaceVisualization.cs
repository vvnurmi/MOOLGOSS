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
        private class IDComparer<T> : IEqualityComparer<T> where T : Wob
        {
            public bool Equals(T x, T y)
            {
                return x.ID == y.ID;
            }

            public int GetHashCode(T obj) { return obj.GetHashCode(); }
        }
        private static IDComparer<Ship> g_shipComparer = new IDComparer<Ship>();

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
            var shipsAdded = diff.Wobs.Added.Values.OfType<Ship>().ToArray();
            var shipsRemoved = diff.Wobs.Removed.Values.OfType<Ship>().ToArray();
            var shipsChanged = shipsAdded.Intersect(shipsRemoved, g_shipComparer).ToArray();
            var playerShipID = Globals.World.Value.GetPlayerShipID(Globals.PlayerID);
            foreach (var ship in shipsChanged)
                if (ship.ID != playerShipID) UpdateShip(ship, 1);
            foreach (var ship in shipsRemoved.Except(shipsChanged, g_shipComparer))
                if (ship.ID != playerShipID) RemoveShip(ship);
            foreach (var ship in shipsAdded.Except(shipsChanged, g_shipComparer))
                if (ship.ID != playerShipID) CreateShip(ship);
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

        public void UpdateShip(Ship ship, float updateInterval)
        {
            Debug.Assert(updateInterval >= 0);
            SceneNode node;
            if (!_nodes.TryGetValue(ship.ID, out node))
            {
                _nodes.Add(ship.ID, node = CreateShip(ship));
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

        private SceneNode CreateShip(Ship ship)
        {
            var mesh = ship is Droid ? "droid.mesh" : "ship1.mesh";
            var shipEnt = Globals.Scene.CreateEntity("ship entity " + _entityIndex++, mesh);
            var node = Globals.Scene.RootSceneNode.CreateChildSceneNode();
            node.AttachObject(shipEnt);
            node.Position = ship.Pos;
            node.Orientation = ship.Orientation;
            return node;
        }

        private void RemoveShip(Ship ship)
        {
            SceneNode node;
            if (!_nodes.TryGetValue(ship.ID, out node)) return;
            Globals.Scene.RootSceneNode.RemoveChild(node);
            _nodes.Remove(ship.ID);
        }
    }
}
