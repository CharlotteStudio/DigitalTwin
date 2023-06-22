using UnityEngine;

namespace InputSample
{
    /// <summary>
    /// Full Screen Detect
    /// </summary>
    public class MouseClickRotation : MonoBehaviour
    {
        private enum RotationDirection
        {
            LeftRight = 0,
            TopDown = 1,
        }
        
        [SerializeField] private MouseClickType _clickType = MouseClickType.LeftClick;
        [SerializeField] private RotationDirection _rotationDirection = RotationDirection.LeftRight;
        [SerializeField] private float _speed = 600.0f;

        private void Update()
        {
            if (!Input.GetMouseButton((int) _clickType)) return;

            var direction = _rotationDirection switch
            {
                RotationDirection.LeftRight => Vector3.up,
                RotationDirection.TopDown => Vector3.right,
                _ => Vector3.up
            };
            
            var mouseDirection = _rotationDirection switch
            {
                RotationDirection.LeftRight => "Mouse X",
                RotationDirection.TopDown => "Mouse Y",
                _ => "Mouse X"
            };

            transform.Rotate(
                direction * Input.GetAxis(mouseDirection) * -_speed * Time.deltaTime, Space.World);
        }
    }
}