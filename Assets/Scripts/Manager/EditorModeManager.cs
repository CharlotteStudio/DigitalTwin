using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class EditorModeManager : MonoBehaviour
{
    [SerializeField] private Transform farmingRoot;
    [SerializeField] private Transform spawnPosition;

    [Header("Template")]
    [SerializeField] private GameObject fieldGroup_1;
    
    [Header("UI")]
    [SerializeField] private Button createPlaneButton;
    [SerializeField] private Button saveButton;
    
    [Space(10)]
    [Header("Events")]
    public UnityEvent OnClickSaveButtonEvents;

    private List<GameObject> _planeList = new List<GameObject>();

    private void Start()
    {
        createPlaneButton.onClick.AddListener(CreatePlaneObject);
        saveButton.onClick.AddListener(OnClickSaveButton);
        ReadFarmlandPositionSave();
        
        if (DeviceManager.Instance != null)
            DeviceManager.Instance.OnSpawnedDeviceEvent += ReadDevicePositionSave;
    }
    
    private void OnClickSaveButton()
    {
        WriteFarmlandSave();
        WriteDeviceSave();
        OnClickSaveButtonEvents?.Invoke();
    }

    private void CreatePlaneObject()
    {
        _planeList.Add(Instantiate(fieldGroup_1, spawnPosition.position, spawnPosition.rotation, farmingRoot));
    }

    private void CreatePlaneObject(Vector3 vector3)
    {
        _planeList.Add(Instantiate(fieldGroup_1, vector3, Quaternion.identity, farmingRoot));
    }

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

    private void WriteFarmlandSave()
    {
        if (_planeList == null || _planeList.Count <= 0) return;
        
        string str = "";
        foreach(var plane in _planeList)
        {
            var pos = plane.transform.position;
            str += $"{pos.x:F},{pos.y:F},{pos.z:F}:";
        }
        str.DebugLog();
        SaveManager.Instance.SaveStringData(MyConstant.SaveKey.FarmlandPosition, str);
    }

    private void WriteDeviceSave()
    {
        if (DeviceManager.Instance == null || DeviceManager.Instance.deviceList.Count == 0) return;
        
        string str = "";
        foreach(var deviceBase in DeviceManager.Instance.deviceList)
        {
            var pos = deviceBase.transform.position;
            str += $"{deviceBase.mac_Address},{pos.x:F},{pos.y:F},{pos.z:F}*";
        }
        str.DebugLog();
        SaveManager.Instance.SaveStringData(MyConstant.SaveKey.DevicePosition, str);
    }
    
    #endregion
}
