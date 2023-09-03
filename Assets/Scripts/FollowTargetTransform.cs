using UnityEngine;

public class FollowTargetTransform : MonoBehaviour
{
    [SerializeField] private Transform _targetTransform;
    [SerializeField] private float _positionOffSet;
    [SerializeField] private float _rotationOffSet;
    
    private Vector3 _lastPosition = Vector3.zero;
    private Quaternion _lastRotation = Quaternion.identity;

    private void Start()
    {
        SetNewPosition();
        SetNewRotation();
    }

    private void Update()
    {
        var newPosition = _targetTransform.position;

        if (Vector3.Distance(newPosition, _lastPosition) > _positionOffSet)
            SetNewPosition();

        if (IsRotationOverOffset())
            SetNewRotation();
    }

    private void SetNewPosition()
    {
        _lastPosition = _targetTransform.position;
        transform.position = _lastPosition;
    }

    private bool IsRotationOverOffset()
    {
        var targetRotation = _targetTransform.rotation;

        if (Mathf.Abs(targetRotation.w - _lastRotation.w) > _rotationOffSet) return true;
        if (Mathf.Abs(targetRotation.x - _lastRotation.x) > _rotationOffSet) return true;
        if (Mathf.Abs(targetRotation.y - _lastRotation.y) > _rotationOffSet) return true;
        if (Mathf.Abs(targetRotation.z - _lastRotation.z) > _rotationOffSet) return true;
        
        return false;
    }
    
    private void SetNewRotation()
    {
        _lastRotation = _targetTransform.rotation;
        transform.rotation = _lastRotation;
    }
}
