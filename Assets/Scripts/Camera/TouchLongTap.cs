using UnityEngine;
using UnityEngine.Events;

namespace TouchSample
{
    // object 需要加 Collider & Rigidbody
    public class TouchLongTap : MonoBehaviour
    {
        [SerializeField] private string _targetTag;
        [SerializeField] private float _clickDuration = 2f;
        
        public UnityEvent OnLongClickUnityEvent;
        public event System.Action<GameObject> OnLongClickEvent;

        private float _touchTime = 0;
        private bool _newTouch = false;
        
        private Camera _mainCamera;

        private void OnEnable()
        {
            if (_mainCamera == null)
                _mainCamera = Camera.main;
        }
        private void OnDisable() => _mainCamera = null;

        private void Update()
        {
            if (Input.touchCount != 1) return;
            
            Ray touchRay = _mainCamera.ScreenPointToRay(Input.mousePosition);
            touchRay.origin.DrawDebugRay(touchRay.direction * 100, duration:0.1f);

            if (!Physics.Raycast(touchRay, out RaycastHit hit, 1000))   return;
            if (!_targetTag.Equals("") && !hit.transform.CompareTag(_targetTag))  return;
            
            Touch touch = Input.GetTouch(0);
    
            if (touch.phase == TouchPhase.Began)
            {
                _newTouch = true;
                _touchTime = Time.time;
            }
            else if (touch.phase == TouchPhase.Stationary && _newTouch && Time.time - _touchTime >= _clickDuration)
            {
                _newTouch = false;
                OnLongClickUnityEvent?.Invoke();
                OnLongClickEvent?.Invoke(hit.collider.gameObject);
            }
            else if (touch.phase == TouchPhase.Ended)
                _newTouch = false;
        }
    }
}
