using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UIManager : ManagerBase<UIManager>
{
    [Header("UI")]
    [SerializeField] private Button editorButton;
    [SerializeField] private Button settingButton;

    [Header("Dialog")]
    [SerializeField] private GameObject editorBlock;
    [SerializeField] private GameObject settingBlock;
    [SerializeField] private Dialog _messageDialog;
    
    
    [Space(10)]
    [Header("Events")]
    public UnityEvent OnClickEditorButtonEvents;
    public UnityEvent OnClickSettingButtonEvents;


    private void Start()
    {
        editorButton.gameObject.SetActive(true);
        settingButton.gameObject.SetActive(true);
        editorBlock.SetActive(false);
        settingBlock.SetActive(false);
        editorButton.onClick.AddListener(EnableEditorBlock);
        settingButton.onClick.AddListener(EnableSettingBlock);
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

    public void SetMessageDialog(string str)
    {
        _messageDialog.SetActiveDialog(str);
        _messageDialog.gameObject.SetActive(true);
    }
}
