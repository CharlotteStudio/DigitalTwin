using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PumpDeviceController : DeviceBase
{
    [SerializeField] private Button button_forceActive;
    [SerializeField] private TMP_Text text_forceActive;
    
    private Camera _mainCamera = null;

    private bool _down = false;
    
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (_down) return;
            
            _down = true;
                
            if (_mainCamera == null) _mainCamera = Camera.main;
        
            var onClickPosition = _mainCamera.ScreenPointToRay(Input.mousePosition);
            Debug.DrawRay(onClickPosition.origin, onClickPosition.direction, Color.green);
        
            if (Physics.Raycast(onClickPosition, out RaycastHit hit, 1000))
            {
                if (hit.transform.gameObject.name == "ForceButton")
                {
                    ForceActivePumpDevice();
                }
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (!_down) return;
            _down = false;
        }
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
        }
        
        if (activeState == 1)
        {
            text_forceActive.text = "On";
            text_forceActive.color = Color.green;
        }
    }
    
    public void ForceActivePumpDevice()
    {
        "On Click Force Active Button".DebugLog();
        DeviceManager.Instance.SetDeviceActiveState(mac_Address, activeState == 0 ? 1 : 0);
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