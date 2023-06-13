[System.Serializable]
public class DeviceCurrentValueReceiver
{
    public DeviceMessage[] Items;
    public int Count;
    public int ScannedCount;
}

[System.Serializable]
public class DeviceMessage
{
    public string mac_address;
    public MessageContent message;
}

[System.Serializable]
public class MessageContent
{
    public int deviceType;
    public int value;
}