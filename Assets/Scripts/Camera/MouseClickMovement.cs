using UnityEngine;

namespace InputSample
{
    /// <summary>
    /// 需要用到 Raycast 去 trigger 地面
    /// 所以要加一個 plane 才可以 move
    /// </summary>
    public class MouseClickMovement : MonoBehaviour
    {
        [SerializeField] private MouseClickType _clickType = MouseClickType.LeftClick;
        [SerializeField] private float _cameraSpeed = 0.1f;
        
        private Camera _mainCamera = null;
        private Vector3 _startPosition; 
        
        private void OnEnable()
        {
            if (_mainCamera == null)
                _mainCamera = Camera.main;
        }
        private void OnDisable() => _mainCamera = null;

        private void Update()
        {
            if (_mainCamera == null) return;
            if (!Input.GetMouseButton((int) _clickType)) return;
            
            var onClickRay = _mainCamera.ScreenPointToRay(Input.mousePosition);
            onClickRay.origin.DrawDebugRay(onClickRay.direction * 100, duration:0.1f);

            if (Physics.Raycast(onClickRay, out RaycastHit hit, 1000))
            {
                var newPosition = transform.position;

                if (Input.GetMouseButtonDown((int) _clickType))
                    _startPosition = hit.point;
                
                if (Input.GetMouseButton((int) _clickType))
                {
                    var newPoint = hit.point;
                    newPosition.x += (_startPosition.x - newPoint.x) * _cameraSpeed;
                    newPosition.z += (_startPosition.z - newPoint.z) * _cameraSpeed;
                }
                    
                transform.position = newPosition;
            }
        }
    }
}
