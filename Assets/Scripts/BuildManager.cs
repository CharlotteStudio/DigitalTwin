using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildManager : MonoBehaviour
{
    [SerializeField] private Transform farmingRoot;
    [SerializeField] private Transform spawnPosition;

    [Header("Template")]
    [SerializeField] private GameObject fieldGroup_1;
    
    [Header("UI")]
    [SerializeField] private Button createPlaneButton;
    [SerializeField] private Button saveButton;

    private List<GameObject> _planeList = new List<GameObject>();

    private void Start()
    {
        createPlaneButton.onClick.AddListener(CreatePlaneObject);
        saveButton.onClick.AddListener(WriteSave);
        ReadSave();
    }

    private void CreatePlaneObject()
    {
        _planeList.Add(Instantiate(fieldGroup_1, spawnPosition.position, spawnPosition.rotation, farmingRoot));
    }

    private void CreatePlaneObject(Vector3 vector3)
    {
        _planeList.Add(Instantiate(fieldGroup_1, vector3, Quaternion.identity, farmingRoot));
    }

    private void ReadSave()
    {
        if (PlayerPrefs.HasKey("PlanePosition"))
        {
            string posString = PlayerPrefs.GetString("PlanePosition");

            string[] splitPos = posString.Split(':');

            foreach(var pos in splitPos)
            {
                string[] vector = pos.Split(',');
                
                if (vector.Length < 3) continue;

                Vector3 position = new Vector3(float.Parse(vector[0]), float.Parse(vector[1]), float.Parse(vector[2]));
                Debug.Log($"Spawn at {position}");
                CreatePlaneObject(position);
            }
        }
    }

    private void WriteSave()
    {
        if (_planeList == null || _planeList.Count <= 0) return;
        
        string str = "";
        foreach(var plane in _planeList)
        {
            var pos = plane.transform.position;
            str += $"{pos.x.ToString("F")},{pos.y.ToString("F")},{pos.z.ToString("F")}:";
        }
        Debug.Log(str);
        
        PlayerPrefs.SetString("PlanePosition", str);
    
        PlayerPrefs.Save(); 
    }
}
