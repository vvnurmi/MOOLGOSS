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
    [System.Runtime.Serialization.DataContractAttribute(Name="Planet", Namespace="http://schemas.datacontract.org/2004/07/MOO.Types")]
    [System.SerializableAttribute()]
    public partial class Planet : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private int idField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private int maxPopulationField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string nameField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private int orbitField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private MOO.Client.MOOService.FSharpOptionOfstring playerField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private int populationField;
        
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
        public int id {
            get {
                return this.idField;
            }
            set {
                if ((this.idField.Equals(value) != true)) {
                    this.idField = value;
                    this.RaisePropertyChanged("id");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int maxPopulation {
            get {
                return this.maxPopulationField;
            }
            set {
                if ((this.maxPopulationField.Equals(value) != true)) {
                    this.maxPopulationField = value;
                    this.RaisePropertyChanged("maxPopulation");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string name {
            get {
                return this.nameField;
            }
            set {
                if ((object.ReferenceEquals(this.nameField, value) != true)) {
                    this.nameField = value;
                    this.RaisePropertyChanged("name");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int orbit {
            get {
                return this.orbitField;
            }
            set {
                if ((this.orbitField.Equals(value) != true)) {
                    this.orbitField = value;
                    this.RaisePropertyChanged("orbit");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public MOO.Client.MOOService.FSharpOptionOfstring player {
            get {
                return this.playerField;
            }
            set {
                if ((object.ReferenceEquals(this.playerField, value) != true)) {
                    this.playerField = value;
                    this.RaisePropertyChanged("player");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int population {
            get {
                return this.populationField;
            }
            set {
                if ((this.populationField.Equals(value) != true)) {
                    this.populationField = value;
                    this.RaisePropertyChanged("population");
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
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="FSharpOptionOfstring", Namespace="http://schemas.datacontract.org/2004/07/Microsoft.FSharp.Core")]
    [System.SerializableAttribute()]
    public partial class FSharpOptionOfstring : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        private string valueField;
        
        [global::System.ComponentModel.BrowsableAttribute(false)]
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData {
            get {
                return this.extensionDataField;
            }
            set {
                this.extensionDataField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(IsRequired=true)]
        public string value {
            get {
                return this.valueField;
            }
            set {
                if ((object.ReferenceEquals(this.valueField, value) != true)) {
                    this.valueField = value;
                    this.RaisePropertyChanged("value");
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
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="Formation", Namespace="http://schemas.datacontract.org/2004/07/MOO.Types")]
    [System.SerializableAttribute()]
    public partial class Formation : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private int idField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private MOO.Client.MOOService.Location locationField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string playerField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private int shipsField;
        
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
        public int id {
            get {
                return this.idField;
            }
            set {
                if ((this.idField.Equals(value) != true)) {
                    this.idField = value;
                    this.RaisePropertyChanged("id");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public MOO.Client.MOOService.Location location {
            get {
                return this.locationField;
            }
            set {
                if ((object.ReferenceEquals(this.locationField, value) != true)) {
                    this.locationField = value;
                    this.RaisePropertyChanged("location");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string player {
            get {
                return this.playerField;
            }
            set {
                if ((object.ReferenceEquals(this.playerField, value) != true)) {
                    this.playerField = value;
                    this.RaisePropertyChanged("player");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int ships {
            get {
                return this.shipsField;
            }
            set {
                if ((this.shipsField.Equals(value) != true)) {
                    this.shipsField = value;
                    this.RaisePropertyChanged("ships");
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
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="Location", Namespace="http://schemas.datacontract.org/2004/07/MOO.Types")]
    [System.SerializableAttribute()]
    public partial class Location : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        private int itemField;
        
        [global::System.ComponentModel.BrowsableAttribute(false)]
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData {
            get {
                return this.extensionDataField;
            }
            set {
                this.extensionDataField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(IsRequired=true)]
        public int item {
            get {
                return this.itemField;
            }
            set {
                if ((this.itemField.Equals(value) != true)) {
                    this.itemField = value;
                    this.RaisePropertyChanged("item");
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
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="MOOService.IMOOService", CallbackContract=typeof(MOO.Client.MOOService.IMOOServiceCallback))]
    public interface IMOOService {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IMOOService/Authenticate", ReplyAction="http://tempuri.org/IMOOService/AuthenticateResponse")]
        void Authenticate(string name);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IMOOService/Authenticate", ReplyAction="http://tempuri.org/IMOOService/AuthenticateResponse")]
        System.Threading.Tasks.Task AuthenticateAsync(string name);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IMOOService/GetPlanets", ReplyAction="http://tempuri.org/IMOOService/GetPlanetsResponse")]
        MOO.Client.MOOService.Planet[] GetPlanets();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IMOOService/GetPlanets", ReplyAction="http://tempuri.org/IMOOService/GetPlanetsResponse")]
        System.Threading.Tasks.Task<MOO.Client.MOOService.Planet[]> GetPlanetsAsync();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IMOOService/GetFormations", ReplyAction="http://tempuri.org/IMOOService/GetFormationsResponse")]
        MOO.Client.MOOService.Formation[] GetFormations();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IMOOService/GetFormations", ReplyAction="http://tempuri.org/IMOOService/GetFormationsResponse")]
        System.Threading.Tasks.Task<MOO.Client.MOOService.Formation[]> GetFormationsAsync();
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IMOOServiceCallback {
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="http://tempuri.org/IMOOService/Update")]
        void Update(System.DateTime now);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IMOOServiceChannel : MOO.Client.MOOService.IMOOService, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class MOOServiceClient : System.ServiceModel.DuplexClientBase<MOO.Client.MOOService.IMOOService>, MOO.Client.MOOService.IMOOService {
        
        public MOOServiceClient(System.ServiceModel.InstanceContext callbackInstance) : 
                base(callbackInstance) {
        }
        
        public MOOServiceClient(System.ServiceModel.InstanceContext callbackInstance, string endpointConfigurationName) : 
                base(callbackInstance, endpointConfigurationName) {
        }
        
        public MOOServiceClient(System.ServiceModel.InstanceContext callbackInstance, string endpointConfigurationName, string remoteAddress) : 
                base(callbackInstance, endpointConfigurationName, remoteAddress) {
        }
        
        public MOOServiceClient(System.ServiceModel.InstanceContext callbackInstance, string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(callbackInstance, endpointConfigurationName, remoteAddress) {
        }
        
        public MOOServiceClient(System.ServiceModel.InstanceContext callbackInstance, System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(callbackInstance, binding, remoteAddress) {
        }
        
        public void Authenticate(string name) {
            base.Channel.Authenticate(name);
        }
        
        public System.Threading.Tasks.Task AuthenticateAsync(string name) {
            return base.Channel.AuthenticateAsync(name);
        }
        
        public MOO.Client.MOOService.Planet[] GetPlanets() {
            return base.Channel.GetPlanets();
        }
        
        public System.Threading.Tasks.Task<MOO.Client.MOOService.Planet[]> GetPlanetsAsync() {
            return base.Channel.GetPlanetsAsync();
        }
        
        public MOO.Client.MOOService.Formation[] GetFormations() {
            return base.Channel.GetFormations();
        }
        
        public System.Threading.Tasks.Task<MOO.Client.MOOService.Formation[]> GetFormationsAsync() {
            return base.Channel.GetFormationsAsync();
        }
    }
}
