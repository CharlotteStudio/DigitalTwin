using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using SquareBeam.AWS.Lambda.Models;

public class EditorModeManager : MonoBehaviour
{
    [SerializeField] private Transform farmingRoot;
    [SerializeField] private Transform spawnPosition;

    [Header("Template")]
    [SerializeField] private GameObject fieldGroup_1;
    
    [Header("UI")]
    [SerializeField] private Button createPlaneButton;
    [SerializeField] private Button saveButton;
    [SerializeField] private Button cancelButton;
    
    [SerializeField] private SaveItem _saveItem = new SaveItem();
    
    [Space(10)]
    [Header("Events")]
    public UnityEvent OnClickSaveButtonEvents;
    public UnityEvent OnClickCancelButtonEvents;
    public UnityEvent OnCompletedGetSaveEvents;

    [Space(10)] [Header("Testing")]
    [SerializeField] private bool _getNullData = false;

    private List<GameObject> _planeList = new List<GameObject>();

    private void Start()
    {
        createPlaneButton.onClick.AddListener(CreatePlaneObject);
        saveButton.onClick.AddListener(OnClickSaveButton);
        cancelButton.onClick.AddListener(OnClickCancelButton);
        
        if (DeviceManager.Instance != null)
            DeviceManager.Instance.OnSpawnedDeviceEvent += ReadSaveFromAWS;
    }
    
    private void OnClickSaveButton()
    {
        // WriteUserFarmlandSave();
        WriteSaveToAWS();
        OnClickSaveButtonEvents?.Invoke();
    }
    
    private void OnClickCancelButton()
    {
        OnClickCancelButtonEvents?.Invoke();
    }

    private void CreatePlaneObject()
    {
        _planeList.Add(Instantiate(fieldGroup_1, spawnPosition.position, spawnPosition.rotation, farmingRoot));
    }

    private void CreatePlaneObject(Vector3 vector3)
    {
        _planeList.Add(Instantiate(fieldGroup_1, vector3, Quaternion.identity, farmingRoot));
    }

    
    #region Read Write Save From AWS
    
    private void ReadSaveFromAWS()
    {
        string userName = "";
        if (UserProfile.Instance != null)
        {
            userName = UserProfile.Instance.GetUserName();
        }
        else
        {
#if UNITY_EDITOR
            userName = "testing@gmail.com";
            "Now is editor mode".DebugLog();
#endif
        }

        if (userName.Equals(""))
        {
            "No user name, please check".DebugLogError();
            OnCompletedGetSaveEvents?.Invoke();
            return;
        }

        SaveOperation saveOperation = new SaveOperation();
        saveOperation.operation = "get";
        saveOperation.payload.Item.UserName = userName;
        string payload = JsonUtility.ToJson(saveOperation);
        
        AWSManager.Instance.InvokeLambdaFunction(MyConstant.AWSService.LambdaFunction.GetSetUserSave, payload, OnReceivedEvent);
        
        void OnReceivedEvent(LambdaResponse response)
        {
            if (response.Success)
            {
                $"Success: Response is :\n{response.DownloadHandler.text.ToPrettyPrintJsonString()}".DebugLog();
                if (response.DownloadHandler.text.Equals("") || response.DownloadHandler.text.Equals("null"))
                {
                    "Can not get anything".DebugLogWarning();
                    OnCompletedGetSaveEvents?.Invoke();
                    return;
                }
                
                response.DownloadHandler.text.DebugLog();
                if (response.DownloadHandler.text.Equals("\"not find\"") || _getNullData)
                {
                    "No Save".DebugLog();
                }
                else
                {
                    _saveItem = JsonUtility.FromJson<SaveItem>(response.DownloadHandler.text);
                    var vectorList = SaveManager.Instance.StringToVector3(_saveItem.message.FarmlandPosition);
                    vectorList.DebugLog();
                    foreach (var pos in vectorList)
                    {
                        CreatePlaneObject(pos);
                    }
                    "Completed Set up farmland position".DebugLog();
                    ReadDeviceSaveFromAWS();
                }
            }
            else
                AWSManager.Instance.ResponseFail(response);
            
            OnCompletedGetSaveEvents?.Invoke();
        }
    }

    private void ReadDeviceSaveFromAWS()
    {
        if (_saveItem == null || _saveItem.UserName.Equals(""))
        {
            "No Device save".DebugLog();
            return;
        }

        var deviceDict = SaveManager.Instance.StringToDict(_saveItem.message.DevicePosition);
        
        if (deviceDict == null || deviceDict.Count == 0) return;

        deviceDict.DebugLog();

        if (DeviceManager.Instance == null)
        {
            "Can not find DeviceManager".DebugLogWarning();
            return;
        }

        foreach (var pair in deviceDict)
        {
            DeviceManager.Instance.SetDevicePosition(pair.Key, pair.Value);
        }
        
        "Completed Set up device position".DebugLog();
        deviceDict.DebugLog();
    }
    
    private void WriteSaveToAWS()
    {
        string userName = "";
        if (UserProfile.Instance != null)
        {
            userName = UserProfile.Instance.GetUserName();
        }
        else
        {
#if UNITY_EDITOR
            userName = "testing@gmail.com";
            "Now is editor mode".DebugLog();
#endif
        }

        if (userName.Equals(""))
        {
            "No user name, please check".DebugLogError();
            return;
        }
        
        SaveOperation saveOperation = new SaveOperation();
        saveOperation.operation = "create";
        saveOperation.payload.Item.UserName = userName;
        saveOperation.payload.Item.message.FarmlandPosition = GetFarmlandSaveString();
        saveOperation.payload.Item.message.DevicePosition = GetDeviceSaveString();
        string payload = JsonUtility.ToJson(saveOperation);
        payload.DebugLog();
        
        AWSManager.Instance.InvokeLambdaFunction(MyConstant.AWSService.LambdaFunction.GetSetUserSave, payload, OnReceivedEvent);

        void OnReceivedEvent(LambdaResponse response)
        {
            if (response.Success)
            {
                $"Success: Response is :\n{response.DownloadHandler.text.ToPrettyPrintJsonString()}".DebugLog();
            }
            else
                AWSManager.Instance.ResponseFail(response);
        }
    }
    
    #endregion
    
    #region Read Write Save
    
    private void ReadFarmlandPositionSave()
    {
        var vectorList = SaveManager.Instance.TryGetFarmlandPositionSave();
        
        if (vectorList.Equals("")) return;
        
        vectorList.DebugLog();
        
        foreach (var pos in vectorList)
        {
            CreatePlaneObject(pos);
        }
        
        "Completed Set up farmland position".DebugLog();
    }
    
    private void ReadDevicePositionSave()
    {
        var deviceDict = SaveManager.Instance.TryGetDevicePositionSave();
        
        if (deviceDict == null || deviceDict.Count == 0) return;

        deviceDict.DebugLog();

        if (DeviceManager.Instance == null)
        {
            "Can not find DeviceManager".DebugLogWarning();
            return;
        }

        foreach (var pair in deviceDict)
        {
            DeviceManager.Instance.SetDevicePosition(pair.Key, pair.Value);
        }
        
        "Completed Set up device position".DebugLog();
    }
    
    private void WriteUserFarmlandSave()
    {
        SaveManager.Instance.SaveStringData(MyConstant.SaveKey.FarmlandPosition, GetFarmlandSaveString());
        SaveManager.Instance.SaveStringData(MyConstant.SaveKey.DevicePosition, GetDeviceSaveString());
    }

    private string GetFarmlandSaveString()
    {
        if (_planeList == null || _planeList.Count <= 0) return "";
        
        string str = "";
        foreach(var plane in _planeList)
        {
            var pos = plane.transform.position;
            str += $"{pos.x:F},{pos.y:F},{pos.z:F}:";
        }
        str.DebugLog();
        return str;
    }

    private string GetDeviceSaveString()
    {
        if (DeviceManager.Instance == null || DeviceManager.Instance.deviceList.Count == 0) return "";
        
        string str = "";
        foreach(var deviceBase in DeviceManager.Instance.deviceList)
        {
            var pos = deviceBase.transform.position;
            str += $"{deviceBase.mac_Address},{pos.x:F},{pos.y:F},{pos.z:F}*";
        }
        str.DebugLog();
        return str;
    }
    
    #endregion
}
