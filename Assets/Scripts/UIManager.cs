using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Button editorButton;

    [Header("Dialog")]
    [SerializeField] private GameObject editorBlock;
    
    [Space(10)]
    [Header("Events")]
    public UnityEvent OnClickEditorButtonEvents;

    private void Start()
    {
        editorButton.gameObject.SetActive(true);
        editorBlock.SetActive(false);
        editorButton.onClick.AddListener(EnableEditorBlock);
    }

    private void EnableEditorBlock()
    {
        OnClickEditorButtonEvents?.Invoke();
        // TODO : Change Camera
    }
}
