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
        ReadUserSave();
    }

    private void CreatePlaneObject()
    {
        _planeList.Add(Instantiate(fieldGroup_1, spawnPosition.position, spawnPosition.rotation, farmingRoot));
    }

    private void CreatePlaneObject(Vector3 vector3)
    {
        _planeList.Add(Instantiate(fieldGroup_1, vector3, Quaternion.identity, farmingRoot));
    }

    private void ReadUserSave()
    {
        var vectorList = SaveManager.Instance.TryGetPositionSave();
        vectorList.DebugLog();
        
        foreach (var pos in vectorList)
        {
            CreatePlaneObject(pos);
        }
    }

    private void OnClickSaveButton()
    {
        WriteSave();
        OnClickSaveButtonEvents?.Invoke();
    }

    private void WriteSave()
    {
        if (_planeList == null || _planeList.Count <= 0) return;
        
        string str = "";
        foreach(var plane in _planeList)
        {
            var pos = plane.transform.position;
            str += $"{pos.x.ToString("F")},{pos.y.ToString("F")},{pos.z.ToString("F")}:";
        }
        str.DebugLog();
        SaveManager.Instance.SaveStringData("PlanePosition", str);
    }
}
