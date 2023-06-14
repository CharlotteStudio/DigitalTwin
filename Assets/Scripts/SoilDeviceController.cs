using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SoilDeviceController : DeviceBase
{
    [SerializeField] protected TMP_Text text_value;
    public override void OnDeviceInit()
    {
        text_value.text = value.ToString();
    }

    public override void OnValueChange()
    {
        text_value.text = value.ToString();
        
    }
}
