using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PumpDeviceController : DeviceBase
{
    [Header("UI - Active Value (View)")]
    [SerializeField] private GameObject _activeValueTextObject;
    [SerializeField] private Button _button_activeValueText;
    
    [Header("UI - Active Value (Setting)")]
    [SerializeField] private GameObject _activeValueSettingObject;
    [SerializeField] private Button _button_activeValueConfirm;
    
    [SerializeField] private TMP_Text text_forceActive;
    [SerializeField] private Button button_forceActive;
    
    [Header("Object")]
    [SerializeField] private GameObject vfxObject;
    
    private void Start()
    {
        InitButton();
    }

    private void InitButton()
    {
        _button_activeValueText.onClick.AddListener(() =>
        {
            "OnClicked Active Value Text Button.".DebugLog();
            _activeValueTextObject.SetActive(false);
            _activeValueSettingObject.SetActive(true);
        });

        _button_activeValueConfirm.onClick.AddListener(() =>
        {
            "OnClicked Active Value Confirm Button.".DebugLog();
            _activeValueTextObject.SetActive(true);
            _activeValueSettingObject.SetActive(false);
        });
        
        _activeValueTextObject.SetActive(true);
        _activeValueSettingObject.SetActive(false);
    }
    
    private void Update()
    {
 
    }

    public override void OnDeviceInit()
    {
        button_forceActive.onClick.AddListener(ForceActivePumpDevice);
    }

    public override void OnValueChange()
    {
        if (activeState == 0)
        {
            text_forceActive.text = "Off";
            text_forceActive.color = Color.red;
            vfxObject.SetActive(false);
        }
        
        if (activeState == 1)
        {
            text_forceActive.text = "On";
            text_forceActive.color = Color.green;
            vfxObject.SetActive(true);
        }
    }

    private int _onOff = 0;
    
    public void ForceActivePumpDevice()
    {
        "On Click Force Active Button".DebugLog();
        //DeviceManager.Instance.SetDeviceActiveState(mac_Address, activeState == 0 ? 1 : 0);
        DeviceManager.Instance.SetDeviceActiveState(mac_Address, _onOff);
        _deviceInfo.message.activeState = _onOff;
        _onOff = _onOff == 1 ? 0 : 1; 
        OnValueChange();
    }
}

#region Editor Function

#if UNITY_EDITOR

[UnityEditor.CustomEditor(typeof(PumpDeviceController))]
public class PumpDeviceControllerEditor : UnityEditor.Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        var pumpDevice = (PumpDeviceController) target;

        GUILayout.Space(10);
        GUILayout.Label("Editor Function:");
        if (GUILayout.Button("Try Send out Force Active"))
        {
            pumpDevice.ForceActivePumpDevice();
        }
    }
}
#endif

#endregion