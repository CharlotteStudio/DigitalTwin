using InputSample;
using TouchSample;
using UnityEngine;

public class CameraController : ManagerBase<CameraController>
{
    [SerializeField] private MouseClickMovement _mouseMovement;
    [SerializeField] private TouchMovement _touchMovement;

    public void LockCameraMovement()
    {
        _mouseMovement.enabled = false;
        _touchMovement.enabled = false;
    }
    
    public void ReleaseCameraMovement()
    {
        _mouseMovement.enabled = true;
        _touchMovement.enabled = true;
    }
}
