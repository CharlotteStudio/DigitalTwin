using UnityEngine;

public class ManagerBase<T> : MonoBehaviour where T : UnityEngine.Object
{
    private static T _instance;
    public static T Instance
    {
        get
        {
            if (!_instance)
            {
                _instance = FindObjectOfType<T>();
            }

            return _instance;
        }
    }
}
