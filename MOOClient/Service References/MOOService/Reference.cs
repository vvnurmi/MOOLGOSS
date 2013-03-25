﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18034
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MOO.Client.MOOService {
    using System.Runtime.Serialization;
    using System;
    
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="Planet", Namespace="http://schemas.datacontract.org/2004/07/MOO.Service")]
    [System.SerializableAttribute()]
    public partial class Planet : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private int IDField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private int MaxPopulationField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string NameField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private int OrbitField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private int PopulationField;
        
        [global::System.ComponentModel.BrowsableAttribute(false)]
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData {
            get {
                return this.extensionDataField;
            }
            set {
                this.extensionDataField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int ID {
            get {
                return this.IDField;
            }
            set {
                if ((this.IDField.Equals(value) != true)) {
                    this.IDField = value;
                    this.RaisePropertyChanged("ID");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int MaxPopulation {
            get {
                return this.MaxPopulationField;
            }
            set {
                if ((this.MaxPopulationField.Equals(value) != true)) {
                    this.MaxPopulationField = value;
                    this.RaisePropertyChanged("MaxPopulation");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Name {
            get {
                return this.NameField;
            }
            set {
                if ((object.ReferenceEquals(this.NameField, value) != true)) {
                    this.NameField = value;
                    this.RaisePropertyChanged("Name");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int Orbit {
            get {
                return this.OrbitField;
            }
            set {
                if ((this.OrbitField.Equals(value) != true)) {
                    this.OrbitField = value;
                    this.RaisePropertyChanged("Orbit");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int Population {
            get {
                return this.PopulationField;
            }
            set {
                if ((this.PopulationField.Equals(value) != true)) {
                    this.PopulationField = value;
                    this.RaisePropertyChanged("Population");
                }
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="MOOService.IMOOService")]
    public interface IMOOService {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IMOOService/GetPlanets", ReplyAction="http://tempuri.org/IMOOService/GetPlanetsResponse")]
        MOO.Client.MOOService.Planet[] GetPlanets();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IMOOService/GetPlanets", ReplyAction="http://tempuri.org/IMOOService/GetPlanetsResponse")]
        System.Threading.Tasks.Task<MOO.Client.MOOService.Planet[]> GetPlanetsAsync();
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IMOOServiceChannel : MOO.Client.MOOService.IMOOService, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class MOOServiceClient : System.ServiceModel.ClientBase<MOO.Client.MOOService.IMOOService>, MOO.Client.MOOService.IMOOService {
        
        public MOOServiceClient() {
        }
        
        public MOOServiceClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public MOOServiceClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public MOOServiceClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public MOOServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public MOO.Client.MOOService.Planet[] GetPlanets() {
            return base.Channel.GetPlanets();
        }
        
        public System.Threading.Tasks.Task<MOO.Client.MOOService.Planet[]> GetPlanetsAsync() {
            return base.Channel.GetPlanetsAsync();
        }
    }
}
