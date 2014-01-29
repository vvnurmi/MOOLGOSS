using Axiom.Math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    public enum MissionState { Open, Offering, Suppressed , Assigned, Completed };

    public class Mission
    {
        public string AssignMessage { get; set; }
        public Sphere AssignVolume { get; set; }
        public string CompleteMessage { get; set; }
        public Sphere CompleteVolume { get; set; }
        public MissionState State { get; private set; }

        public void Offer()
        {
            if (State != MissionState.Open && State != MissionState.Suppressed) throw new InvalidOperationException();
            State = MissionState.Offering;
        }

        public void Suppress()
        {
            if (State != MissionState.Offering) throw new InvalidOperationException();
            State = MissionState.Suppressed;
        }

        public void Assign()
        {
            if (State != MissionState.Offering) throw new InvalidOperationException();
            State = MissionState.Assigned;
        }

        public void Complete()
        {
            if (State != MissionState.Assigned) throw new InvalidOperationException();
            State = MissionState.Completed;
        }
    }
}
