using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    private static SaveManager _instance;

    public static SaveManager Instance
    {
        get
        {
            if (!_instance)
            {
                _instance = FindObjectOfType<SaveManager>();
            }

            return _instance;
        }
    }

    public List<Vector3> TryGetPositionSave()
    {
        List<Vector3> vectors = new List<Vector3>();

        if (PlayerPrefs.HasKey("PlanePosition"))
        {

            string posString = PlayerPrefs.GetString("PlanePosition");

            string[] splitPos = posString.Split(':');

            foreach(var pos in splitPos)
            {
                string[] vector = pos.Split(',');
                
                if (vector.Length < 3) continue;

                Vector3 position = new Vector3(float.Parse(vector[0]), float.Parse(vector[1]), float.Parse(vector[2]));
                vectors.Add(position);
            }
        }
        return vectors;
    }

    public void SaveStringData(string key, string content)
    {
        PlayerPrefs.SetString(key, content);
        PlayerPrefs.Save(); 
    }
}
