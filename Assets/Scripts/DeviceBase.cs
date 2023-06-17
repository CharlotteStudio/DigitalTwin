using UnityEngine;
using UnityEngine.UI;
using TMPro;

public abstract class DeviceBase : MonoBehaviour
{
    [SerializeField, ReadOnly] protected DeviceInfo _deviceInfo;
    [Header("UI")]
    [SerializeField] protected TMP_Text text_macAddress;
    
    public string mac_Address  => _deviceInfo.mac_address;
    public int type            => _deviceInfo.message.deviceType;
    public int value           => _deviceInfo.message.value;
    public int updateSpeed     => _deviceInfo.message.updateSpeed;
    public string listenDevice => _deviceInfo.message.listenDevice;
    public int activeValue     => _deviceInfo.message.activeValue;
    public int activeDuration  => _deviceInfo.message.activeDuration;
    public int activeState     => _deviceInfo.message.activeState;

    public void Init(DeviceInfo deviceInfo)
    {
        _deviceInfo = deviceInfo;
        text_macAddress.text = mac_Address;
        OnDeviceInit();
    }

    public void UpdateSetting(DeviceInfo deviceInfo)
    {
        if (!mac_Address.Equals(deviceInfo.mac_address))
        {
            $"The mac address is not match !\nDevice Mac Address : {mac_Address}\nIncome Mac Address : {deviceInfo.mac_address}".DebugLogWarning();
            return;
        }
        
        _deviceInfo.message.listenDevice = deviceInfo.message.listenDevice;
        _deviceInfo.message.activeValue = deviceInfo.message.activeValue;
        _deviceInfo.message.activeDuration = deviceInfo.message.activeDuration;
        _deviceInfo.message.updateSpeed = deviceInfo.message.updateSpeed;
        OnSettingChange();
    }

    public void UpdateValue(DeviceInfo deviceInfo)
    {
        if (!mac_Address.Equals(deviceInfo.mac_address))
        {
            $"The mac address is not match !\nDevice Mac Address : {mac_Address}\nIncome Mac Address : {deviceInfo.mac_address}".DebugLogWarning();
            return;
        }
        _deviceInfo.message.value = deviceInfo.message.value;
        OnValueChange();
    }
    
    public abstract void OnDeviceInit();

    public abstract void OnValueChange();
    
    public abstract void OnSettingChange();
}
