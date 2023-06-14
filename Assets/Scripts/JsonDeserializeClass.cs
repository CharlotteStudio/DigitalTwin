[System.Serializable]
public class DeviceCurrentValueReceiver
{
    public DeviceInfo[] Items;
    public int Count;
    public int ScannedCount;
}

[System.Serializable]
public class DeviceInfo
{
    public string mac_address;
    public MessageContent message;
}

[System.Serializable]
public class MessageContent
{
    public int deviceType;
    public int activeState;
    public int value;
}