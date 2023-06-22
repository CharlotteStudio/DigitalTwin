using UnityEngine;

namespace TouchSample
{
    public class TouchMovement : MonoBehaviour
    {
        private Camera _mainCamera;
        [SerializeField] [Min(1)] private int _touchCount = 1;
        [SerializeField] private float _moveSpeed = 0.05f;

        private Vector3 _startPosition; 

        private void OnEnable()
        {
            if (_mainCamera == null)
                _mainCamera = Camera.main;
        }
        private void OnDisable() => _mainCamera = null;

        private void Update()
        {
            if (Input.touchCount != _touchCount) return;
            
            Ray touchRay = _mainCamera.ScreenPointToRay(GetVectorCenter());
            touchRay.origin.DrawDebugRay(touchRay.direction * 100, duration:0.1f);

            if (!Physics.Raycast(touchRay, out RaycastHit hit, 1000)) return;
            
            var newPosition = transform.position;
            
            Touch touch = Input.GetTouch(_touchCount - 1);
            
            if (touch.phase == TouchPhase.Began)
                _startPosition = hit.point;
            
            if (touch.phase == TouchPhase.Moved)
            {
                var newPoint = hit.point;
                newPosition.x += (_startPosition.x - newPoint.x) * _moveSpeed;
                newPosition.z += (_startPosition.z - newPoint.z) * _moveSpeed;
            }
                
            transform.position = newPosition;
        }

        private Vector2 GetVectorCenter()
        {
            Vector2 pos = Vector2.zero;
            foreach (var touch in Input.touches)
            {
                pos += touch.position;
            }
            return pos / Input.touchCount;
        }
    }
}