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
    public string listenDevice;
    public int activeValue;
    public int activeDuration;
    public int activeState;
    public int value;
    public int setUpdateSpeed;
}

[System.Serializable]
public class SaveOperation
{
    public SavePayload payload;
    public string operation;

    public SaveOperation()
    {
        payload = new SavePayload();
    }
}

[System.Serializable]
public class SavePayload
{
    public string TableName;
    public SaveItem Item;
    
    public SavePayload()
    { 
        Item = new SaveItem();
    }
}

[System.Serializable]
public class SaveItem
{
    public string UserName;
    public MessageSave message;
    
    public SaveItem()
    { 
        message = new MessageSave();
    }
}

[System.Serializable]
public class MessageSave
{
    public string FarmlandPosition;
    public string DevicePosition;
}