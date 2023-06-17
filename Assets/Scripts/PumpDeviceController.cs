using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PumpDeviceController : DeviceBase
{
    [Header("UI - Active Value (View)")]
    [SerializeField] private GameObject _activeValueTextObject;
    [SerializeField] private Button _buttonActiveValueText;
    [SerializeField] private TMP_Text _textActiveValueText;
    
    [Header("UI - Active Value (Setting)")]
    [SerializeField] private GameObject _activeValueSettingObject;
    [SerializeField] private TMP_InputField _inputFieldActiveValue;
    [SerializeField] private Button _buttonActiveValueConfirm;
    
    [Header("UI - Active State")]
    [SerializeField] private Button _buttonActiveState;
    [SerializeField] private TMP_Text _textActiveState;
    
    [Header("Object")]
    [SerializeField] private GameObject vfxObject;
    
    private void Start()
    {
        InitActiveValueButton();
    }
    
    private void Update()
    {
 
    }

    public override void OnDeviceInit()
    {
        _buttonActiveState.onClick.AddListener(ForceActivePumpDevice);
        _textActiveValueText.text = activeValue.ToString();
    }

    public override void OnValueChange()
    {
        UpdateActiveStateText();
        UpdateVFX();
        _buttonActiveState.interactable = true;
    }

    public override void OnSettingChange()
    {
        _textActiveValueText.text = activeValue.ToString();
    }

    #region Active Value Button
    
    private void InitActiveValueButton()
    {
        _buttonActiveValueText.onClick.AddListener(() =>
        {
            "OnClicked Active Value Text Button.".DebugLog();
            _activeValueTextObject.SetActive(false);
            _activeValueSettingObject.SetActive(true);
        });

        _buttonActiveValueConfirm.onClick.AddListener(OnClickedActiveValueConfirmAction);
        
        _activeValueTextObject.SetActive(true);
        _activeValueSettingObject.SetActive(false);
    }

    private void OnClickedActiveValueConfirmAction()
    {
        "OnClicked Active Value Confirm Button.".DebugLog();
        _activeValueTextObject.SetActive(true);
        _activeValueSettingObject.SetActive(false);

        if (!int.TryParse(_inputFieldActiveValue.text, out int newValue) || newValue is < 0 or > 4000)
        {
            $"Wrong Input : [{newValue}]".DebugLogWarning();
                
            if (UIManager.Instance != null)
                UIManager.Instance.SetMessageDialog("Please input the value between 0 to 4000");
                
            return;
        }
        DeviceManager.Instance.SetDeviceActiveValue(mac_Address, newValue, Callback);

        void Callback()
        {
            _deviceInfo.message.activeValue = newValue;
            OnSettingChange();
        }
    }

    #endregion
    
    #region Active State Button

    public void OnPinterEnterButtonState()
    {
        if (!_buttonActiveState.interactable) return;
        
        _textActiveState.text  = activeState == 1 ? "Force OFF" : "Force ON";
        _textActiveState.color = activeState == 1 ? Color.red : Color.green;
    }

    public void OnPinterExitButtonState()
    {
        if (!_buttonActiveState.interactable) return;
        
        UpdateActiveStateText();
    }

    private void UpdateActiveStateText()
    {
        _textActiveState.text  = activeState == 1 ? "ON" : "OFF";
        _textActiveState.color = activeState == 1 ? Color.green : Color.red;
    }

    private void UpdateVFX() =>
        vfxObject.SetActive(activeState == 1);
    
    
    public void ForceActivePumpDevice()
    {
        "OnClicked Force Active Button".DebugLog();
        DeviceManager.Instance.SetDeviceActiveState(mac_Address, activeState == 1 ? 0 : 1);
        
        _textActiveState.text = "Loading...";
        _textActiveState.color = Color.white;
        _buttonActiveState.interactable = false;
    }

    #endregion
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