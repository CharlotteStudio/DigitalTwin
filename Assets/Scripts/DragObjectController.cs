using UnityEngine;

public class DragObjectController : MonoBehaviour
{
    [SerializeField] private GameObject cameraMoveObject;
    [SerializeField] private string targetTag = "";
    
    private Camera _mainCamera = null;
    private GameObject _selectedObject = null;

    private void Update()
    {
        if (_mainCamera == null) _mainCamera = Camera.main;

        if (_selectedObject == null)
            _selectedObject = GetOnClickSelectedObject();
        
        if (_selectedObject != null)
        {
            UpdateSelectedObjectPosition();
            
            if (cameraMoveObject.activeSelf)
                cameraMoveObject.SetActive(false);
        }


        if (Input.GetMouseButtonUp(0))
        {
            cameraMoveObject.SetActive(true);
            _selectedObject = null;
        }
    }

    private void UpdateSelectedObjectPosition()
    {
        var onClickPosition = _mainCamera.ScreenPointToRay(Input.mousePosition);
        Debug.DrawRay(onClickPosition.origin, onClickPosition.direction, Color.green);
        
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

    private GameObject GetOnClickSelectedObject()
    {
        if (Input.GetMouseButtonDown(0))
        {
            var onClickPosition = _mainCamera.ScreenPointToRay(Input.mousePosition);
            
            Debug.DrawRay(onClickPosition.origin, onClickPosition.direction, Color.green);
            
            if (Physics.Raycast(onClickPosition, out RaycastHit hit, 1000))
            {
                if (targetTag == "") return hit.collider.gameObject;
                if (hit.collider.gameObject.CompareTag(targetTag)) return hit.collider.gameObject;
            }
        }

        return null;
    }
}