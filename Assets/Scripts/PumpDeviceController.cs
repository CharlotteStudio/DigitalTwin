using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PumpDeviceController : DeviceBase
{
    [Header("UI - Listen Device (View)")]
    [SerializeField] private GameObject _listenDeviceTextObject;
    [SerializeField] private Button _buttonlistenDeviceText;
    [SerializeField] private TMP_Text _textlistenDeviceText;
    
    [Header("UI - Listen Device (Setting)")]
    [SerializeField] private GameObject _listenDeviceSettingObject;
    [SerializeField] private TMP_Dropdown _listenDeviceDropdown;
    
    [Header("UI - Active Value (View)")]
    [SerializeField] private GameObject _activeValueTextObject;
    [SerializeField] private Button _buttonActiveValueText;
    [SerializeField] private TMP_Text _textActiveValueText;
    
    [Header("UI - Active Value (Setting)")]
    [SerializeField] private GameObject _activeValueSettingObject;
    [SerializeField] private TMP_InputField _inputFieldActiveValue;
    [SerializeField] private Button _buttonActiveValueConfirm;
    
    [Header("UI - Duration (View)")]
    [SerializeField] private GameObject _durationTextObject;
    [SerializeField] private Button _buttonDurationText;
    [SerializeField] private TMP_Text _textDurationText;
    
    [Header("UI - Duration (Setting)")]
    [SerializeField] private GameObject _durationSettingObject;
    [SerializeField] private TMP_InputField _inputFieldDuration;
    [SerializeField] private Button _buttonDurationConfirm;
    
    [Header("UI - Active State")]
    [SerializeField] private Button _buttonActiveState;
    [SerializeField] private TMP_Text _textActiveState;

    [Header("Camera")]
    public Transform _cameraTran;
    
    [Header("Object")]
    [SerializeField] private GameObject vfxObject;
    
    private void Start()
    {
        InitActiveValueButton();
        InitDurationButton();
    }

    public override void OnDeviceInit()
    {
        _buttonActiveState.onClick.AddListener(ForceActivePumpDevice);
        _textlistenDeviceText.text = listenDevice;
        _textActiveValueText.text = activeValue.ToString();
        UpdateActiveStateText();
        
        DeviceManager.Instance.OnSpawnedDeviceEvent += InitListenDeviceButton;
    }

    public override void OnValueChange()
    {
        UpdateActiveStateText();
        UpdateVFX();
        _buttonActiveState.interactable = true;
    }

    public override void OnSettingChange()
    {
        _textlistenDeviceText.text = listenDevice;
        _textActiveValueText.text = activeValue.ToString();
        _textDurationText.text = activeDuration + " s";
    }

    private void InitListenDeviceButton()
    {
        _buttonlistenDeviceText.onClick.AddListener(() =>
        {
            "OnClicked Listen Device Text Button.".DebugLog();
            _listenDeviceTextObject.SetActive(false);
            _listenDeviceSettingObject.SetActive(true);
        });
        
        _listenDeviceDropdown.AddOptions(DeviceManager.Instance.GetAllSoilDeviceMacAddress());
        _listenDeviceDropdown.onValueChanged.AddListener(delegate
        {
            DropdownValueChanged(_listenDeviceDropdown);
        });

        _listenDeviceTextObject.SetActive(true);
        _listenDeviceSettingObject.SetActive(false);
    }
    
    private void DropdownValueChanged(TMP_Dropdown change)
    {
        $"Change the mac address is {change.name}".DebugLog();
                
        if (change.name.Equals(listenDevice)) return;
                
        DeviceManager.Instance.SetDeviceListenDevice(mac_Address, change.name, Callback);
                
        _listenDeviceTextObject.SetActive(true);
        _listenDeviceSettingObject.SetActive(false);

        void Callback()
        {
            _deviceInfo.message.listenDevice = change.name;
            OnSettingChange();
        }
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
    
    #region Duration Button
    
    private void InitDurationButton()
    {
        _buttonDurationText.onClick.AddListener(() =>
        {
            "OnClicked Duration Text Button.".DebugLog();
            _durationTextObject.SetActive(false);
            _durationSettingObject.SetActive(true);
        });

        _buttonDurationConfirm.onClick.AddListener(OnClickedDurationConfirmAction);
        
        _durationTextObject.SetActive(true);
        _durationSettingObject.SetActive(false);
    }

    private void OnClickedDurationConfirmAction()
    {
        "OnClicked Duration Confirm Button.".DebugLog();
        _durationTextObject.SetActive(true);
        _durationSettingObject.SetActive(false);

        if (!int.TryParse(_inputFieldDuration.text, out int newValue) || newValue < 1)
        {
            $"Wrong Input : [{newValue}]".DebugLogWarning();
                
            if (UIManager.Instance != null)
                UIManager.Instance.SetMessageDialog("Please input the value bigger than 0");
                
            return;
        }
        DeviceManager.Instance.SetDeviceDuration(mac_Address, newValue, Callback);

        void Callback()
        {
            _deviceInfo.message.activeDuration = newValue;
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