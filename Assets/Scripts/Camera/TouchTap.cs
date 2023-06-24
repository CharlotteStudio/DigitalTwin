using UnityEngine;
using UnityEngine.Events;

namespace TouchSample
{
    // object 需要加 Collider & Rigidbody
    public class TouchTap : MonoBehaviour
    {
        [SerializeField] private string _targetTag;
        
        public UnityEvent OnTapUnityEvent;
        public event System.Action<GameObject> OnTapEvent;
            
        private Camera _mainCamera;

        private void OnEnable()
        {
            if (_mainCamera == null)
                _mainCamera = Camera.main;
        }
        private void OnDisable() => _mainCamera = null;

        private void Update()
        {
            if (_mainCamera == null)   return;
            if (Input.touchCount != 1) return;
        
            Ray touchRay = _mainCamera.ScreenPointToRay(Input.mousePosition);
            touchRay.origin.DrawDebugRay(touchRay.direction * 100, duration:0.1f);

            if (!Physics.Raycast(touchRay, out RaycastHit hit, 1000))   return;
            if (!_targetTag.Equals("") && !hit.transform.CompareTag(_targetTag))  return;

            if (Input.GetTouch(0).phase == TouchPhase.Began && Input.GetTouch(0).tapCount == 2)
            {
                OnTapUnityEvent?.Invoke();
                OnTapEvent?.Invoke(hit.collider.gameObject);
            }
        }
    }
}
