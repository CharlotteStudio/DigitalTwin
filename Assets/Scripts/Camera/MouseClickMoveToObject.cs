using System.Collections;
using UnityEngine;

namespace InputSample
{
    public class MouseClickMoveToObject : MonoBehaviour
    {
        [SerializeField] private MouseClickType _clickType = MouseClickType.LeftClick;
        [SerializeField] private Vector3 _targetOffset = new Vector3(0,2,-2);
        [SerializeField] private float _moveDuration = 2;
        [SerializeField] private string _targetTag;
        
        private bool _getTarget = false;
        
        private Vector3 _targetPosition = Vector3.zero;
        
        private Camera _mainCamera = null;
        private Coroutine _cameraMoveCoroutine = null;
        
        private void OnEnable()
        {
            _getTarget = false;
            _targetPosition = Vector3.zero;
            
            if (_mainCamera == null)
                _mainCamera = Camera.main;
        }
        private void OnDisable() => _mainCamera = null;
        
        private void Update()
        {
            if (_mainCamera == null)   return;

            if (Input.GetMouseButtonDown((int)_clickType))
            {
                var onClickRay = _mainCamera.ScreenPointToRay(Input.mousePosition);
                onClickRay.origin.DrawDebugRay(onClickRay.direction * 100, duration:0.1f);
                
                if (!Physics.Raycast(onClickRay, out RaycastHit hit, 1000)) return;
                if (!_targetTag.Equals("") && !hit.transform.CompareTag(_targetTag))  return;

                _targetPosition = hit.point + _targetOffset;
                _getTarget = true;
            }

            if (Input.GetMouseButtonUp((int)_clickType))
            {
                if (_getTarget)
                {
                    _cameraMoveCoroutine = StartCoroutine(CameraMoveCoroutine());
                    _getTarget = false;
                }
            }
        }
        
        private IEnumerator CameraMoveCoroutine()
        {
            float start = 0;
            while (start < _moveDuration)
            {
                transform.position = Vector3.Lerp(transform.position, _targetPosition, start / _moveDuration);
                start += Time.deltaTime;
                yield return null;
            }
        }
    }
}