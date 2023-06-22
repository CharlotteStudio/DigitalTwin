using UnityEngine;

namespace TouchSample
{
     /// <summary>
     /// Full Screen Detect
     /// </summary>
     public class TouchRotation : MonoBehaviour
     {
          private enum RotationDirection
          {
               LeftRight = 0,
               TopDown = 1,
          }
          [SerializeField] private RotationDirection _rotationDirection = RotationDirection.LeftRight;
          [SerializeField] [Min(1)] private int _touchCount = 1;
          [SerializeField] private float _rotationSpeed = 50.0f;

          private void Update()
          {
               if (Input.touchCount < _touchCount) return; 
               if (Input.GetTouch(_touchCount - 1).phase != TouchPhase.Moved) return;
               
               //if (Input.GetTouch(_touchCount - 1).phase == TouchPhase.Moved && System.Math.Abs(_rotationSpeed - 600) > 0.1f)
               //     _rotationSpeed = 600;
               
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
                    direction * Input.GetAxis(mouseDirection) * -_rotationSpeed * Time.deltaTime, Space.World);
          }
     }
}
