using System;
using System.Collections;
using System.Collections.Generic;
using SquareBeam.AWS.Lambda.Models;
using UnityEngine;

public class DeviceManager : ManagerBase<DeviceManager>
{
    [Header("Setting")]
    [Min(1)] public float deviceUpdateFrequency = 5;
    
    [Header("Devices Spawn Root")]
    [SerializeField] private Transform _deviceSpawnRoot;

    [Header("Device Object")]
    public List<DeviceBase> deviceList = new List<DeviceBase>();
    
    [Header("Devices Info")]
    public List<DeviceInfo> deviceInfoList = new List<DeviceInfo>();
    
    [Header("Receiver")]
    public DeviceCurrentValueReceiver receivedMessage;

    [Header("Devices Prefabs")]
    [SerializeField] private GameObject _soilDevicePrefab;
    [SerializeField] private GameObject _pumpDevicePrefab;
    
    private AWSManager aws => AWSManager.Instance;

    public event System.Action OnGetDeviceStateEvent;
    public event System.Action OnSpawnedDeviceEvent;
    public event System.Action OnGetCurrentDeviceValueEvent;

    private void Start()
    {
        OnGetDeviceStateEvent        += SpawnDevices;
        OnSpawnedDeviceEvent         += GetCurrentDeviceValue;
        OnGetCurrentDeviceValueEvent += WaitingNextUpdateDevice;
        GetDeviceState();
    }
    
    private void WaitingNextUpdateDevice()
    {
        StartCoroutine(co_UpdateDevice());
        
        IEnumerator co_UpdateDevice()
        {
            yield return new WaitForSeconds(deviceUpdateFrequency);
            GetCurrentDeviceValue();
        }
    }

    private void SetUpDeviceInfo()
    {
        foreach (var deviceData in receivedMessage.Items)
        {
            var isExisted = false;
            foreach (var deviceInfo in deviceInfoList)
            {
                if (deviceInfo.mac_address.Equals(deviceData.mac_address))
                    isExisted = true;
            }
            
            if (!isExisted)
                deviceInfoList.Add(deviceData);
        }
    }

    public void SpawnDevices()
    {
        if (deviceInfoList == null || deviceInfoList.Count == 0)
        {
            $"The {nameof(deviceInfoList)} is nothing.".DebugLogWarning();
            return;
        }
        
        foreach (var deviceInfo in deviceInfoList)
        {
            if (IsExistedDevice(deviceInfo)) continue;

            GameObject newDevice;
            switch (deviceInfo.message.deviceType)
            {
                case 1:
                    newDevice = Instantiate(_soilDevicePrefab, _deviceSpawnRoot);
                    newDevice.name = $"Soil Device - {deviceInfo.mac_address}";
                    break;
                
                case 2:
                    newDevice = Instantiate(_pumpDevicePrefab, _deviceSpawnRoot);
                    newDevice.name = $"Pump Device - {deviceInfo.mac_address}";

                    break;
                
                default:
                    $"Can not find the device type [{deviceInfo.message.deviceType}]".DebugLogError();
                    return;
            }
            
            var deviceBase = newDevice.GetComponent<DeviceBase>();
            deviceBase.Init(deviceInfo);
            deviceList.Add(deviceBase);
        }
        OnSpawnedDeviceEvent?.Invoke();
    }
    
    private bool IsExistedDevice(DeviceInfo deviceInfo)
    {
        var isExisted = false;
            
        foreach (var device in deviceList)
        {
            if (deviceInfo.mac_address.Equals(device.mac_Address))
                isExisted = true;
        }

        return isExisted;
    }

    private void UpdateDeviceValue()
    {
        if (deviceList == null || deviceList.Count == 0) return;

        foreach (var deviceData in receivedMessage.Items)
        {
            if (!IsExistedDevice(deviceData)) continue;
            
            foreach (var info in deviceInfoList)
            {
                if (info.mac_address.Equals(deviceData.mac_address))
                    info.message.value = deviceData.message.value;
            }
            foreach (var deviceBase in deviceList)
            {
                if (deviceBase.mac_Address.Equals(deviceData.mac_address))
                    deviceBase.UpdateValue(deviceData);
            }
        }
    }

    public void SetDevicePosition(string mac_address, Vector3 position)
    {
        if (deviceList == null || deviceList.Count == 0)
            $"The {nameof(deviceList)} is nothing.".DebugLogWarning();

        foreach (var deviceBase in deviceList)
        {
            if (!deviceBase.mac_Address.Equals(mac_address)) continue;

            deviceBase.transform.position = position;
        }
        
        $"Set Device [{mac_address}] to {position}".DebugLog();
    }

    public List<string> GetAllDeviceMacAddress()
    {
        List<string> macAddress = new List<string>();
        
        if (deviceList == null || deviceList.Count == 0)
            $"The {nameof(deviceList)} is nothing.".DebugLogWarning();

        foreach (var deviceBase in deviceList)
            macAddress.Add(deviceBase.mac_Address);
        
        return macAddress;
    }
    
    public List<string> GetAllSoilDeviceMacAddress()
    {
        List<string> macAddress = new List<string>();
        
        if (deviceList == null || deviceList.Count == 0)
            $"The {nameof(deviceList)} is nothing.".DebugLogWarning();

        foreach (var deviceBase in deviceList)
        {
            if (deviceBase.type == 1)
                macAddress.Add(deviceBase.mac_Address);
        }
        
        return macAddress;
    }
    
    #region Get Function

    public void GetDeviceState()
    {
        string payload = "";

        aws.InvokeLambdaFunction(MyConstant.AWSService.LambdaFunction.GetDeviceState, payload, OnReceivedEvent);

        void OnReceivedEvent(LambdaResponse response)
        {
            if (response.Success)
            {
                $"Success: Response is :\n{response.DownloadHandler.text.ToPrettyPrintJsonString()}".DebugLog();
                receivedMessage = JsonUtility.FromJson<DeviceCurrentValueReceiver>(response.DownloadHandler.text);
                SetUpDeviceInfo();
                OnGetDeviceStateEvent?.Invoke();
            }
            else
                aws.ResponseFail(response);
        }
    }

    public void GetCurrentDeviceValue()
    {
        string payload = "";

        aws.InvokeLambdaFunction(MyConstant.AWSService.LambdaFunction.GetDeviceCurrentValue, payload, OnReceivedEvent);

        void OnReceivedEvent(LambdaResponse response)
        {
            if (response.Success)
            {
                $"Success: Response is :\n{response.DownloadHandler.text.ToPrettyPrintJsonString()}".DebugLog();
                receivedMessage = JsonUtility.FromJson<DeviceCurrentValueReceiver>(response.DownloadHandler.text);
                UpdateDeviceValue();
                OnGetCurrentDeviceValueEvent?.Invoke();
            }
            else
                aws.ResponseFail(response);
        }
    }

    
    #endregion
    
    #region Send out Functions
    
    public void SetDeviceListenDevice(string macAddress, string targetMacAddress, Action onSuccessCallback = null)
    {
        string payload = "{\"DeviceMac\":\"";
        payload += macAddress;
        payload += "\",\"ListenDevice\":\"";
        payload += targetMacAddress;
        payload += "\"}";
        
        SendOutToAWSService(payload, onSuccessCallback);
    }
    
    public void SetDeviceActiveValue(string macAddress, int value, Action onSuccessCallback = null)
    {
        string payload = "{\"DeviceMac\":\"";
        payload += macAddress;
        payload += "\",\"ActiveValue\":";
        payload += value;
        payload += "}";
        
        SendOutToAWSService(payload, onSuccessCallback);
    }
    
    public void SetDeviceActiveState(string macAddress, int value, Action onSuccessCallback = null)
    {
        string payload = "{\"DeviceMac\":\"";
        payload += macAddress;
        payload += "\",\"ActiveState\":";
        payload += value;
        payload += "}";

        SendOutToAWSService(payload, onSuccessCallback);
    }

    public void SetDeviceUpdateSpeed(string macAddress, int speed, Action onSuccessCallback = null)
    {
        string payload = "{\"DeviceMac\":\"";
        payload += macAddress;
        payload += "\",\"UpdateSpeed\":";
        payload += speed;
        payload += "}";
        
        SendOutToAWSService(payload, onSuccessCallback);
    }
    
    private void SendOutToAWSService(string payload, Action onSuccessCallback = null)
    {
        $"Send out json : {payload}".DebugLog();
        
        aws.InvokeLambdaFunction(MyConstant.AWSService.LambdaFunction.SetDeviceSetting, payload, OnSendOutSuccessEvent);

        void OnSendOutSuccessEvent(LambdaResponse response)
        {
            if (response.Success)
            {
                $"Success send out :\n{payload}".DebugLog();
                onSuccessCallback?.Invoke();
            }
            else
                "Fail send out".DebugLog();
        }
    }

    #endregion
    
    public void ClearAllData()
    {
        deviceInfoList.Clear();
        receivedMessage = null;
    }

    public void ClearAllDevice()
    {
        foreach (var child in deviceList)
        {
            DestroyImmediate(child.gameObject);
        }
        deviceList.Clear();
    }
}

#region Editor Function

#if UNITY_EDITOR

[UnityEditor.CustomEditor(typeof(DeviceManager))]
public class DeviceManagerEditor : UnityEditor.Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        var deviceManager = (DeviceManager) target;

        GUILayout.Space(10);
        GUILayout.Label("Editor Function:");
        if (GUILayout.Button("Try Get Device State"))
        {
            AWSManager.Instance.SetUpLambdaClient();
            deviceManager.GetDeviceState();
        }
        
        if (GUILayout.Button("Spawn Devices"))
            deviceManager.SpawnDevices();
        
        if (GUILayout.Button("Get Current Device Value"))
        {
            AWSManager.Instance.SetUpLambdaClient();
            deviceManager.GetCurrentDeviceValue();
        }

        if (GUILayout.Button("Clear All Device Data"))
            deviceManager.ClearAllData();
        
        if (GUILayout.Button("Remove All Devices"))
            deviceManager.ClearAllDevice();
    }
}
#endif

#endregion