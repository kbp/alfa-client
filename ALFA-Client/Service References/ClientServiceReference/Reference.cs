﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.1
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ALFA_Client.ClientServiceReference {
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="ClientServiceReference.IClientService", CallbackContract=typeof(ALFA_Client.ClientServiceReference.IClientServiceCallback), SessionMode=System.ServiceModel.SessionMode.Required)]
    public interface IClientService {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IClientService/SetKey", ReplyAction="http://tempuri.org/IClientService/SetKeyResponse")]
        bool SetKey(byte[] key, byte number, string portName, byte controllerNumber, string name, byte type, System.DateTime endDate);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IClientService/UnsetKey", ReplyAction="http://tempuri.org/IClientService/UnsetKeyResponse")]
        bool UnsetKey(string portName, byte controllerNumber, byte number);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IClientService/ReadKey", ReplyAction="http://tempuri.org/IClientService/ReadKeyResponse")]
        byte[] ReadKey(string portName);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IClientService/SetRoomToProtect", ReplyAction="http://tempuri.org/IClientService/SetRoomToProtectResponse")]
        bool SetRoomToProtect(string portName, byte controllerNumber, bool isProtected);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IClientService/SetLight", ReplyAction="http://tempuri.org/IClientService/SetLightResponse")]
        bool SetLight(string portName, byte controllerNumber, bool lightOn);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IClientService/Join", ReplyAction="http://tempuri.org/IClientService/JoinResponse")]
        bool Join(string portName);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IClientService/SetMasterKey", ReplyAction="http://tempuri.org/IClientService/SetMasterKeyResponse")]
        bool SetMasterKey(byte[] key);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IClientService/AddRoomToFloor", ReplyAction="http://tempuri.org/IClientService/AddRoomToFloorResponse")]
        bool AddRoomToFloor(string portName, int roomNumber, string roomClass, byte controllerNumber, bool onLine, int roomCategory, bool isProtected);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IClientService/AddFloor", ReplyAction="http://tempuri.org/IClientService/AddFloorResponse")]
        bool AddFloor(string portName, string floorName);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IClientService/StopFloorPolling", ReplyAction="http://tempuri.org/IClientService/StopFloorPollingResponse")]
        bool StopFloorPolling(string portName);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IClientService/StartFloorPolling", ReplyAction="http://tempuri.org/IClientService/StartFloorPollingResponse")]
        bool StartFloorPolling(string portName);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IClientService/SetDataBaseConnectionString", ReplyAction="http://tempuri.org/IClientService/SetDataBaseConnectionStringResponse")]
        bool SetDataBaseConnectionString(string name, string ip, string login, string password);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IClientServiceCallback {
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="http://tempuri.org/IClientService/AlertAboutControllerNotResponsible")]
        void AlertAboutControllerNotResponsible(string portName, byte controllerNumber);
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="http://tempuri.org/IClientService/AlertAboutControllerBeganRespond")]
        void AlertAboutControllerBeganRespond(string portName, byte controllerNumber);
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="http://tempuri.org/IClientService/AlertComPortNotResponsible")]
        void AlertComPortNotResponsible(string portName);
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="http://tempuri.org/IClientService/AlertComPortBeganRespond")]
        void AlertComPortBeganRespond(string portName);
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="http://tempuri.org/IClientService/AlertGerkon")]
        void AlertGerkon(long roomId);
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="http://tempuri.org/IClientService/AlertUnsetKey")]
        void AlertUnsetKey(string portName, byte controllerNumber);
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="http://tempuri.org/IClientService/AlertChangeRoomsOnTheFloor")]
        void AlertChangeRoomsOnTheFloor(string portName);
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="http://tempuri.org/IClientService/AlertChangeFloors")]
        void AlertChangeFloors(string portName);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IClientServiceChannel : ALFA_Client.ClientServiceReference.IClientService, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class ClientServiceClient : System.ServiceModel.DuplexClientBase<ALFA_Client.ClientServiceReference.IClientService>, ALFA_Client.ClientServiceReference.IClientService {
        
        public ClientServiceClient(System.ServiceModel.InstanceContext callbackInstance) : 
                base(callbackInstance) {
        }
        
        public ClientServiceClient(System.ServiceModel.InstanceContext callbackInstance, string endpointConfigurationName) : 
                base(callbackInstance, endpointConfigurationName) {
        }
        
        public ClientServiceClient(System.ServiceModel.InstanceContext callbackInstance, string endpointConfigurationName, string remoteAddress) : 
                base(callbackInstance, endpointConfigurationName, remoteAddress) {
        }
        
        public ClientServiceClient(System.ServiceModel.InstanceContext callbackInstance, string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(callbackInstance, endpointConfigurationName, remoteAddress) {
        }
        
        public ClientServiceClient(System.ServiceModel.InstanceContext callbackInstance, System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(callbackInstance, binding, remoteAddress) {
        }
        
        public bool SetKey(byte[] key, byte number, string portName, byte controllerNumber, string name, byte type, System.DateTime endDate) {
            return base.Channel.SetKey(key, number, portName, controllerNumber, name, type, endDate);
        }
        
        public bool UnsetKey(string portName, byte controllerNumber, byte number) {
            return base.Channel.UnsetKey(portName, controllerNumber, number);
        }
        
        public byte[] ReadKey(string portName) {
            return base.Channel.ReadKey(portName);
        }
        
        public bool SetRoomToProtect(string portName, byte controllerNumber, bool isProtected) {
            return base.Channel.SetRoomToProtect(portName, controllerNumber, isProtected);
        }
        
        public bool SetLight(string portName, byte controllerNumber, bool lightOn) {
            return base.Channel.SetLight(portName, controllerNumber, lightOn);
        }
        
        public bool Join(string portName) {
            return base.Channel.Join(portName);
        }
        
        public bool SetMasterKey(byte[] key) {
            return base.Channel.SetMasterKey(key);
        }
        
        public bool AddRoomToFloor(string portName, int roomNumber, string roomClass, byte controllerNumber, bool onLine, int roomCategory, bool isProtected) {
            return base.Channel.AddRoomToFloor(portName, roomNumber, roomClass, controllerNumber, onLine, roomCategory, isProtected);
        }
        
        public bool AddFloor(string portName, string floorName) {
            return base.Channel.AddFloor(portName, floorName);
        }
        
        public bool StopFloorPolling(string portName) {
            return base.Channel.StopFloorPolling(portName);
        }
        
        public bool StartFloorPolling(string portName) {
            return base.Channel.StartFloorPolling(portName);
        }
        
        public bool SetDataBaseConnectionString(string name, string ip, string login, string password) {
            return base.Channel.SetDataBaseConnectionString(name, ip, login, password);
        }
    }
}
