using UnityEngine;

public class DragObjectController : MonoBehaviour
{
    private const string TargetTag = "farmingItem";
    private const string DeviceTag = "device";
    [SerializeField] private Camera _mainCamera = null;
    private GameObject _selectedObject = null;

    private void Update()
    {
        if (_mainCamera == null) _mainCamera = Camera.main;

        if (_selectedObject == null)
            _selectedObject = GetOnClickSelectedObject();
        
        if (_selectedObject != null)
        {
            UpdateSelectedObjectPosition();
        }
        
        if (Input.GetMouseButtonUp(0))
        {
            _selectedObject = null;
        }
    }
    
    private GameObject GetOnClickSelectedObject()
    {
        if (Input.GetMouseButtonDown(0) || Input.touchCount == 1)
        {
            var onClickPosition = Input.GetMouseButtonDown(0) ?
                _mainCamera.ScreenPointToRay(Input.mousePosition) :
                _mainCamera.ScreenPointToRay(Input.touches[0].position);
            
            onClickPosition.origin.DrawDebugRay(onClickPosition.direction * 100);

            if (Physics.Raycast(onClickPosition, out RaycastHit hit, 1000))
            {
                if (hit.collider.gameObject.CompareTag(TargetTag) || hit.collider.gameObject.CompareTag(DeviceTag))
                    return hit.collider.gameObject;
            }
        }

        return null;
    }

    private void UpdateSelectedObjectPosition()
    {
        var onClickPosition = Input.touchCount > 0 ?
            _mainCamera.ScreenPointToRay(Input.touches[0].position) :
            _mainCamera.ScreenPointToRay(Input.mousePosition);
        
        onClickPosition.origin.DrawDebugRay(onClickPosition.direction * 100);
        
        if (Physics.Raycast(onClickPosition, out RaycastHit hit, 1000))
        {
            if (hit.transform.gameObject.name != "plane")
            {
                var pos = hit.point;
                pos.y = 0;
                _selectedObject.transform.position = pos;
            }
        }
    }
}