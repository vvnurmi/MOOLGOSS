using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace MOO.Service
{
    [ServiceContract(CallbackContract = typeof(IMOOCallbackContract))]
    public interface IMOOService
    {
        [OperationContract]
        Planet[] GetPlanets();
    }

    public interface IMOOCallbackContract
    {
        [OperationContract(IsOneWay = true)]
        void Update(DateTime now);
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
