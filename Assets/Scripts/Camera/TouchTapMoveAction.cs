using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace TouchSample
{
    /// <summary>
    /// You can use with <see cref="TouchTap"/>> or <see cref="TouchLongTap"/>>
    /// </summary>
    public class TouchTapMoveAction : MonoBehaviour
    {
        [SerializeField] private Vector3 _targetOffset = new Vector3(0,2,-2);
        [SerializeField] private Quaternion _targetQuat = Quaternion.identity;
        [SerializeField] private float _moveDuration = 2;

        public UnityEvent OnClickEvent;
        
        private Coroutine _cameraMoveCoroutine = null;

        private void Awake()
        {
            if (TryGetComponent(out TouchTap tap))
            {
                tap.OnTapEvent += MouseClickMoveToObject;
            }

            if (TryGetComponent(out TouchLongTap longTap))
            {
                longTap.OnLongClickEvent += MouseClickMoveToObject;
            }
        }
        
        public void MouseClickMoveToObject(GameObject targetObject)
        {
            _cameraMoveCoroutine = StartCoroutine(
                CameraMoveCoroutine(targetObject.transform.position + _targetOffset));
            OnClickEvent?.Invoke();
        }
        
        private IEnumerator CameraMoveCoroutine(Vector3 targetPosition)
        {
            float start = 0;
            while (start < _moveDuration)
            {
                transform.position = Vector3.Lerp(transform.position, targetPosition, start / _moveDuration);
                transform.rotation = Quaternion.Lerp(transform.rotation, _targetQuat, start / _moveDuration);
                start += Time.deltaTime;
                yield return null;
            }
        }
    }
}