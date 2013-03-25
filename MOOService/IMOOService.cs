using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace MOO.Service
{
    [ServiceContract]
    public interface IMOOService
    {
        [OperationContract]
        Planet[] GetPlanets();
    }

    [DataContract]
    public class Planet
    {
        [DataMember]
        public int ID { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public int Population { get; set; }

        [DataMember]
        public int MaxPopulation { get; set; }

        [DataMember]
        public int Orbit { get; set; }
    }
}
