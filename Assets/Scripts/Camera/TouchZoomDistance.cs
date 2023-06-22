using UnityEngine;

namespace TouchSample
{
    public class TouchZoomDistance : MonoBehaviour
    {
        [SerializeField] private float _zoomSpeed = 0.08f;
        
        private Vector2 _oldPost1;
        private Vector2 _oldPost2;

        private void Update()
        {
            if (Input.touchCount != 2) return ;
            if (Input.GetTouch(0).phase != TouchPhase.Moved || Input.GetTouch(1).phase != TouchPhase.Moved) return;
            
            Vector2 temPos1 = Input.GetTouch(0).position;
            Vector2 temPos2 = Input.GetTouch(1).position;

            float oldScale = transform.position.y;
            float newScale;
            if (isEnLarge(_oldPost1, _oldPost2, temPos1, temPos2))
                newScale = oldScale * (1 - _zoomSpeed);
            else
                newScale = oldScale / (1 - _zoomSpeed);
            
            transform.position = new Vector3(
                transform.position.x,
                newScale,
                transform.position.z);

            _oldPost1 = temPos1;
            _oldPost2 = temPos2;
        }
        
        private bool isEnLarge(Vector2 oP1, Vector2 oP2, Vector2 nP1, Vector2 nP2)
        {
            float length1 = Mathf.Sqrt((oP1.x - oP2.x) * (oP1.x - oP2.x) + (oP1.y - oP2.y) * (oP1.y - oP2.y));
            float length2 = Mathf.Sqrt((nP1.x - nP2.x) * (nP1.x - nP2.x) + (nP1.y - nP2.y) * (nP1.y - nP2.y));

            return length1 < length2;
        }
    }
}