using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BlackBlockUI : MonoBehaviour
{
    [SerializeField] private Image _blackBlock;
    [SerializeField] private TMP_Text _loadingText;

    private Coroutine _loadingTextCoroutine;

    private void Start()
    {
        _blackBlock.gameObject.SetActive(true);
        _loadingTextCoroutine = StartCoroutine(LoadingTextCoroutine());
    }

    public void DisappearBlackBlock()
    {
        StopCoroutine(_loadingTextCoroutine);
        _loadingText.text = "Completed Loading Save";
        _blackBlock.rectTransform.LeanAlpha(0, 1.5f);
        Destroy(_blackBlock.gameObject, 1.5f);
    }

    private IEnumerator LoadingTextCoroutine()
    {
        _loadingText.text = "Loading Save ";
        while (true)
        {
            yield return new WaitForSeconds(0.5f);
            _loadingText.text += ".";
        }
    }
}
