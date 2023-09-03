using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ARManager : ManagerBase<ARManager>
{
    [Header("UI")]
    [SerializeField] private Button _backButton;

    [Header("Object")]
    [SerializeField] private Transform _spawnDeivceTransform;
    [SerializeField] private Transform _targetARTransform;
        
    private void Start()
    {
        _backButton.onClick.AddListener(
            ()=> SceneManager.LoadScene(MyConstant.MainScene));
    }

    public void SetDeviceObjectToARObject()
    {
        _spawnDeivceTransform.parent = _targetARTransform;
        _spawnDeivceTransform.transform.localPosition = Vector3.zero;
        _spawnDeivceTransform.transform.localRotation = Quaternion.identity;
    }
}
