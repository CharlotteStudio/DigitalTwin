using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PumpDeviceController : DeviceBase
{
    [SerializeField] private Button button_forceActive;
    [SerializeField] private TMP_Text text_forceActive;
    
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
        }
        
        if (activeState == 1)
        {
            text_forceActive.text = "On";
            text_forceActive.color = Color.green;
        }
    }
    
    private void ForceActivePumpDevice()
    {
        //DeviceManager.Instance.
    }
}
