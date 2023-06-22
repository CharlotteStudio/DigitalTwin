using UnityEngine;

namespace InputSample
{
    public class MouseScrollZoom : MonoBehaviour
    {
        private Camera _mainCamera = null;
        [SerializeField] private float _scrollSpeed = 0.3f;
    
        private void OnEnable()
        {
            if (_mainCamera == null)
                _mainCamera = Camera.main;
        }
        private void OnDisable() => _mainCamera = null;

        private void Update()
        {
            var newPosition = transform.position;
            newPosition.y += Input.mouseScrollDelta.y * _scrollSpeed * -1;
        
            transform.position = newPosition;
        }
    }
}
