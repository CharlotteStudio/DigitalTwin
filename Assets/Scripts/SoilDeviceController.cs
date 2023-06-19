using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SoilDeviceController : DeviceBase
{
    [Header("UI - UpdateSpeed (View)")]
    [SerializeField] private GameObject _updateSpeedTextObject;
    [SerializeField] private Button _buttonUpdateSpeedText;
    [SerializeField] private TMP_Text _textUpdateSpeedText;
    
    [Header("UI - UpdateSpeed (Setting)")]
    [SerializeField] private GameObject _updateSpeedSettingObject;
    [SerializeField] private TMP_InputField _inputFieldActiveValue;
    [SerializeField] private Button _buttonUpdateSpeedConfirm;
    
    
    [SerializeField] protected TMP_Text text_value;
    
    
    
    public override void OnDeviceInit()
    {
        text_value.text = value.ToString();
        _textUpdateSpeedText.text = updateSpeed.ToString();
        InitUpdateSpeedButton();
    }

    public override void OnValueChange()
    {
        text_value.text = value.ToString();
    }
    
    public override void OnSettingChange()
    {
        _textUpdateSpeedText.text = updateSpeed.ToString();
    }
    
    private void InitUpdateSpeedButton()
    {
        _buttonUpdateSpeedText.onClick.AddListener(() =>
        {
            "OnClicked Update Speed Text Button.".DebugLog();
            _updateSpeedTextObject.SetActive(false);
            _updateSpeedSettingObject.SetActive(true);
        });

        _buttonUpdateSpeedConfirm.onClick.AddListener(OnClickedUpdateSpeedConfirmAction);
        
        _updateSpeedTextObject.SetActive(true);
        _updateSpeedSettingObject.SetActive(false);
    }
    
    private void OnClickedUpdateSpeedConfirmAction()
    {
        "OnClicked Update Speed Confirm Button.".DebugLog();
        _updateSpeedTextObject.SetActive(true);
        _updateSpeedSettingObject.SetActive(false);

        if (!int.TryParse(_inputFieldActiveValue.text, out int newValue) || newValue is < 0)
        {
            $"Wrong Input : [{newValue}]".DebugLogWarning();
                
            if (UIManager.Instance != null)
                UIManager.Instance.SetMessageDialog("Please input the number more then 0 ");
                
            return;
        }
        DeviceManager.Instance.SetDeviceUpdateSpeed(mac_Address, newValue, Callback);

        void Callback()
        {
            _deviceInfo.message.updateSpeed = newValue;
            OnSettingChange();
        }
    }
}
