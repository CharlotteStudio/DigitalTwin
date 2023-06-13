using System.Collections;
using System.Collections.Generic;
using SquareBeam.AWS.Lambda.Models;
using UnityEngine;

public class DeviceManager : ManagerBase<DeviceManager>
{

    [Header("Devices")]
    public List<DeviceMessage> deviceList = new List<DeviceMessage>();
    
    [Header("Receiver")]
    public DeviceCurrentValueReceiver receivedMessage;
    private AWSManager aws => AWSManager.Instance;
    
    
    private void Start()
    {
        
    }

    private void UpdateAllDevice()
    {
        
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
                UpdateAllDevice();
            }
            else
                aws.ResponseFail(response);
        }
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

        if (GUILayout.Button("Try Get Current Device Value"))
        {
            AWSManager.Instance.SetUpLambdaClient();
            deviceManager.GetCurrentDeviceValue();
        }
    }
}
#endif

#endregion