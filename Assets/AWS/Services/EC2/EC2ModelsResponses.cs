using System.Collections.Generic;
using System.Xml.Serialization;

namespace SquareBeam.AWS.EC2.Models.ParsedResponses
{

    [XmlRoot(ElementName = "RunInstancesResponse", Namespace = "http://ec2.amazonaws.com/doc/2016-11-15/")]
    public class RunEC2InstanceResponse
    {
        [XmlElement(ElementName = "requestId")]
        public string RequestId { get; set; }
        [XmlArray(ElementName = "instancesSet")]
        [XmlArrayItem("item")]
        public List<InstanceItem> Instances { get; set; }

        public class InstanceItem
        {
            [XmlElement(ElementName = "instanceId")]
            public string InstanceId { get; set; }

            [XmlElement(ElementName = "imageId")]
            public string ImageId { get; set; }
            [XmlElement(ElementName = "instanceState")]
            public State InstanceState { get; set; }
            [XmlElement(ElementName = "privateDnsName")]
            public string PrivateDnsName { get; set; }
            [XmlElement(ElementName = "instanceType")]
            public string InstanceType { get; set; }
            [XmlElement(ElementName = "subnetId")]
            public string SubnetUd { get; set; }
            [XmlElement(ElementName = "vpcId")]
            public string VpcId { get; set; }
            [XmlElement(ElementName = "privateIpAddress")]
            public string PrivateIdAdress { get; set; }
            [XmlElement(ElementName = "stateReason")]
            public StateReason StateReason;
            [XmlElement(ElementName = "architecture")]
            public string Architecture { get; set; }
            [XmlElement(ElementName = "rootDeviceType")]
            public string RootDeviceType { get; set; }
            [XmlElement(ElementName = "rootDeviceName")]
            public string RootDeviceName { get; set; }
            [XmlArray(ElementName = "networkInterfaceSet")]
            [XmlArrayItem("item")]
            public List<NetworkInterface> NetworkInterfaceSet { get; set; }
        }

        public class StateReason
        {
            [XmlElement(ElementName = "code")]
            public string Code { get; set; }
            [XmlElement(ElementName = "message")]
            public string Message { get; set; }
        }

        public class NetworkInterface
        {
            [XmlElement(ElementName = "networkInterfaceId")]
            public string NetworkInterfaceId { get; set; }
            [XmlElement(ElementName = "subnetId")]
            public string SubnetId { get; set; }
            [XmlElement(ElementName = "vpcId")]
            public string VpcId { get; set; }
            [XmlElement(ElementName = "ownerId")]
            public string OwnerId { get; set; }
            [XmlElement(ElementName = "status")]
            public string Status { get; set; }
            [XmlElement(ElementName = "macAddress")]
            public string MacAddress { get; set; }
            [XmlElement(ElementName = "privateIpAddress")]
            public string PrivateIpAddress { get; set; }
            [XmlElement(ElementName = "privateDnsName")]
            public string PrivateDnsName { get; set; }
        }
    }


    [XmlRoot(ElementName = "DescribeInstancesResponse", Namespace = "http://ec2.amazonaws.com/doc/2016-11-15/")]
    public class DescribeEC2InstanceResponse
    {
        [XmlElement(ElementName = "requestId")]
        public string RequestId { get; set; }
        [XmlArray(ElementName = "reservationSet")]
        [XmlArrayItem("item")]
        public List<ReservationItem> ReservationSet { get; set; }

        public class ReservationItem
        {
            [XmlElement(ElementName = "reservationId")]
            public string ReservationId { get; set; }
            [XmlElement(ElementName = "ownerId")]
            public string OwnerId { get; set; }
            [XmlArray(ElementName = "instancesSet")]
            [XmlArrayItem("item")]
            public List<InstanceItem> Instances { get; set; }

        }

        public class InstanceItem
        {
            [XmlElement(ElementName = "instanceId")]
            public string InstanceId { get; set; }

            [XmlElement(ElementName = "imageId")]
            public string ImageId { get; set; }
            [XmlElement(ElementName = "instanceState")]
            public State InstanceState { get; set; }
            [XmlElement(ElementName = "privateDnsName")]
            public string PrivateDnsName { get; set; }
            [XmlElement(ElementName = "instanceType")]
            public string InstanceType { get; set; }
            [XmlElement(ElementName = "subnetId")]
            public string SubnetUd { get; set; }
            [XmlElement(ElementName = "vpcId")]
            public string VpcId { get; set; }
            [XmlElement(ElementName = "privateIpAddress")]
            public string PrivateIdAdress { get; set; }
            [XmlElement(ElementName = "architecture")]
            public string Architecture { get; set; }
            [XmlElement(ElementName = "rootDeviceType")]
            public string RootDeviceType { get; set; }
            [XmlElement(ElementName = "rootDeviceName")]
            public string RootDeviceName { get; set; }
            [XmlArray(ElementName = "networkInterfaceSet")]
            [XmlArrayItem("item")]
            public List<NetworkInterface> NetworkInterfaceSet { get; set; }
        }

        public class StateReason
        {
            [XmlElement(ElementName = "code")]
            public string Code { get; set; }
            [XmlElement(ElementName = "message")]
            public string Message { get; set; }
        }

        public class NetworkInterface
        {
            [XmlElement(ElementName = "networkInterfaceId")]
            public string NetworkInterfaceId { get; set; }
            [XmlElement(ElementName = "subnetId")]
            public string SubnetId { get; set; }
            [XmlElement(ElementName = "vpcId")]
            public string VpcId { get; set; }
            [XmlElement(ElementName = "ownerId")]
            public string OwnerId { get; set; }
            [XmlElement(ElementName = "status")]
            public string Status { get; set; }
            [XmlElement(ElementName = "macAddress")]
            public string MacAddress { get; set; }
            [XmlElement(ElementName = "privateIpAddress")]
            public string PrivateIpAddress { get; set; }
            [XmlElement(ElementName = "privateDnsName")]
            public string PrivateDnsName { get; set; }
        }
    }


    [XmlRoot(ElementName = "StartInstancesResponse", Namespace = "http://ec2.amazonaws.com/doc/2016-11-15/")]
    public class StartEC2InstanceResponse
    {
        [XmlElement(ElementName = "requestId")]
        public string RequestId { get; set; }
        [XmlArray(ElementName = "instancesSet")]
        [XmlArrayItem("item")]
        public List<InstanceItem> Instances { get; set; }

        public class InstanceItem
        {
            [XmlElement(ElementName = "instanceId")]
            public string InstanceId { get; set; }
            [XmlElement(ElementName = "currentState")]
            public State CurrentState { get; set; }
            [XmlElement(ElementName = "previousState")]
            public State PreviousState { get; set; }
        }


    }


    [XmlRoot(ElementName = "StopInstancesResponse", Namespace = "http://ec2.amazonaws.com/doc/2016-11-15/")]
    public class StopEC2InstanceResponse
    {
        [XmlElement(ElementName = "requestId")]
        public string RequestId { get; set; }
        [XmlArray(ElementName = "instancesSet")]
        [XmlArrayItem("item")]
        public List<InstanceItem> Instances { get; set; }

        public class InstanceItem
        {
            [XmlElement(ElementName = "instanceId")]
            public string InstanceId { get; set; }
            [XmlElement(ElementName = "currentState")]
            public State CurrentState { get; set; }
            [XmlElement(ElementName = "previousState")]
            public State PreviousState { get; set; }
        }
    }

    [XmlRoot(ElementName = "TerminateInstancesResponse", Namespace = "http://ec2.amazonaws.com/doc/2016-11-15/")]
    public class TerminateEC2InstanceResponse
    {
        [XmlElement(ElementName = "requestId")]
        public string RequestId { get; set; }
        [XmlArray(ElementName = "instancesSet")]
        [XmlArrayItem("item")]
        public List<InstanceItem> Instances { get; set; }

        public class InstanceItem
        {
            [XmlElement(ElementName = "instanceId")]
            public string InstanceId { get; set; }
            [XmlElement(ElementName = "currentState")]
            public State CurrentState { get; set; }
            [XmlElement(ElementName = "previousState")]
            public State PreviousState { get; set; }
        }
    }

    public class State
    {
        [XmlElement(ElementName = "code")]
        public int Code { get; set; }
        [XmlElement(ElementName = "name")]
        public string Name { get; set; }
    }
}