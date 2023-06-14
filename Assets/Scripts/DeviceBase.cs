using UnityEngine;

public class DeviceBase : MonoBehaviour
{
    public string macAddress => _deviceInfo.mac_address;
    public int type => _deviceInfo.message.deviceType;
    public int value => _deviceInfo.message.value;
    public int activeState=> _deviceInfo.message.activeState;
    
    [SerializeField] private DeviceInfo _deviceInfo;
    
    public void Init(DeviceInfo deviceInfo)
    {
        _deviceInfo = deviceInfo;
    }
}
