﻿using Axiom.Math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    [Serializable]
    public class Station : IEquatable<Station>
    {
        private readonly Guid _id;
        private readonly Vector3 _pos;

        public Guid ID { get { return _id; } }
        public Vector3 Pos { get { return _pos; } }

        public Station(Guid id, Vector3 pos)
        {
            _id = id;
            _pos = pos;
        }

        public bool Equals(Station other)
        {
            return ID == other.ID && Pos == other.Pos;
        }

        public override string ToString()
        {
            return "Station at " + Pos;
        }
    }
}
