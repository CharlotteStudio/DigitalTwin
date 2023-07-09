using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class UIManager : ManagerBase<UIManager>
{
    [SerializeField] private Camera _mainCamera;
    
    [Header("UI - Normal")]
    [SerializeField] private Button editorButton;
    [SerializeField] private Button settingButton;
    [SerializeField] private Button logoutButton;
    [SerializeField] private Button returnBackButton;
    [SerializeField] private Button arButton;
    
    [Header("UI - Setting")]
    [SerializeField] private TMP_InputField updateSpeedInputField;
    [SerializeField] private Button confirmSettingButton;
    
    [Header("Dialog")]
    [SerializeField] private GameObject editorBlock;
    [SerializeField] private GameObject settingBlock;
    [SerializeField] private Dialog _messageDialog;

    [Space(10)]
    [Header("Events")]
    public UnityEvent OnClickEditorButtonEvents;
    public UnityEvent OnClickSettingButtonEvents;
    public UnityEvent OnClickReturnButtonEvents;
    
    private Vector3 _cameraLastPosition = Vector3.zero;
    private Quaternion _cameraLastQuat = Quaternion.identity;

    private void Start()
    {
        _mainCamera = Camera.main;
        editorButton.gameObject.SetActive(true);
        settingButton.gameObject.SetActive(true);
        logoutButton.gameObject.SetActive(true);
        returnBackButton.gameObject.SetActive(false);
        
        editorBlock.SetActive(false);
        settingBlock.SetActive(false);

        editorButton.onClick.AddListener(EnableEditorBlock);
        settingButton.onClick.AddListener(EnableSettingBlock);
        logoutButton.onClick.AddListener(()=> SceneManager.LoadScene(MyConstant.LoginScene));
        arButton.onClick.AddListener(()=> SceneManager.LoadScene(MyConstant.ARScene));
        returnBackButton.onClick.AddListener(DisableDeviceView);
        confirmSettingButton.onClick.AddListener(() =>
        {
            if (!int.TryParse(updateSpeedInputField.text, out int newValue) || newValue < 0)
            {
                $"Wrong Input : [{newValue}]".DebugLogWarning();
                SetMessageDialog("Please input the value bigger than 0");
                return;
            }
            DeviceManager.Instance.deviceUpdateFrequency = newValue;
            DisableSettingBlock();
        });
    }

    private void EnableEditorBlock()
    {
        editorButton.gameObject.SetActive(false);
        settingButton.gameObject.SetActive(false);
        logoutButton.gameObject.SetActive(false);
        editorBlock.SetActive(true);
        OnClickEditorButtonEvents?.Invoke();
        // TODO : Change Camera
    }
    
    private void EnableSettingBlock()
    {
        editorButton.gameObject.SetActive(false);
        settingButton.gameObject.SetActive(false);
        logoutButton.gameObject.SetActive(false);
        settingBlock.SetActive(true);
        OnClickSettingButtonEvents?.Invoke();
    }
    
    private void DisableSettingBlock()
    {
        editorButton.gameObject.SetActive(true);
        settingButton.gameObject.SetActive(true);
        logoutButton.gameObject.SetActive(true);
        settingBlock.SetActive(false);
    }

    public void EnableDeviceView()
    {
        editorButton.gameObject.SetActive(false);
        settingButton.gameObject.SetActive(false);
        logoutButton.gameObject.SetActive(false);
        returnBackButton.gameObject.SetActive(true);
        _cameraLastPosition = _mainCamera.transform.position;
        _cameraLastQuat = _mainCamera.transform.rotation;
    }
    
    public void DisableDeviceView()
    {
        editorButton.gameObject.SetActive(true);
        settingButton.gameObject.SetActive(true);
        logoutButton.gameObject.SetActive(true);
        returnBackButton.gameObject.SetActive(false);
        _mainCamera.transform.position = _cameraLastPosition;
        _mainCamera.transform.rotation = _cameraLastQuat;
        OnClickReturnButtonEvents?.Invoke();
    }
    
    public void SetMessageDialog(string str)
    {
        _messageDialog.SetActiveDialog(str);
        _messageDialog.gameObject.SetActive(true);
    }
}
