using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Dialog : MonoBehaviour
{
    [SerializeField] private TMP_Text _messageText;
    [SerializeField] private Button _closeButton;

    private void Start()
    {
        _closeButton.onClick.AddListener(()=>gameObject.SetActive(false));
    }

    public void SetActiveDialog(string message)
    {
        _messageText.text = message;
    }
}
