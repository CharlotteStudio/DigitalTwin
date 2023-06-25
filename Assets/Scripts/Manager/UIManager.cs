using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using DentedPixel;

public class UIManager : ManagerBase<UIManager>
{
    [SerializeField] private Camera _mainCamera;
    
    [Header("UI")]
    [SerializeField] private Button editorButton;
    [SerializeField] private Button settingButton;
    [SerializeField] private Button returnBackButton;
    [SerializeField] private Image _blackBlock;
    
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
        returnBackButton.gameObject.SetActive(false);
        
        editorBlock.SetActive(false);
        settingBlock.SetActive(false);
        _blackBlock.gameObject.SetActive(true);
        
        editorButton.onClick.AddListener(EnableEditorBlock);
        settingButton.onClick.AddListener(EnableSettingBlock);
        returnBackButton.onClick.AddListener(DisableDeviceView);
    }

    private void EnableEditorBlock()
    {
        editorButton.gameObject.SetActive(false);
        settingButton.gameObject.SetActive(false);
        editorBlock.SetActive(true);
        OnClickEditorButtonEvents?.Invoke();
        // TODO : Change Camera
    }
    
    private void EnableSettingBlock()
    {
        editorButton.gameObject.SetActive(false);
        settingButton.gameObject.SetActive(false);
        settingBlock.SetActive(true);
        OnClickSettingButtonEvents?.Invoke();
    }

    public void EnableDeviceView()
    {
        editorButton.gameObject.SetActive(false);
        settingButton.gameObject.SetActive(false);
        returnBackButton.gameObject.SetActive(true);
        _cameraLastPosition = _mainCamera.transform.position;
        _cameraLastQuat = _mainCamera.transform.rotation;
    }
    
    public void DisableDeviceView()
    {
        editorButton.gameObject.SetActive(true);
        settingButton.gameObject.SetActive(true);
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

    public void DisappearBlackBlock()
    {
        _blackBlock.rectTransform.LeanAlpha(0, 1.5f);
    }
}
