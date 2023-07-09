using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ARManager : ManagerBase<ARManager>
{
    [SerializeField] private Button _backButton;

    private void Start()
    {
        _backButton.onClick.AddListener(
            ()=> SceneManager.LoadScene(MyConstant.MainScene));
    }
}
