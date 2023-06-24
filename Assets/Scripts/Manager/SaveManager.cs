using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveManager : ManagerBase<SaveManager>
{
    public List<Vector3> TryGetFarmlandPositionSave()
    {
        List<Vector3> vectors = new List<Vector3>();

        if (PlayerPrefs.HasKey(MyConstant.SaveKey.FarmlandPosition))
        {
            string posString = PlayerPrefs.GetString(MyConstant.SaveKey.FarmlandPosition);

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

    public Dictionary<string, Vector3> TryGetDevicePositionSave()
    {
        Dictionary<string /*mac address*/, Vector3 /*position*/> dict = new Dictionary<string, Vector3>();
        
        if (PlayerPrefs.HasKey(MyConstant.SaveKey.DevicePosition))
        {
            string posString = PlayerPrefs.GetString(MyConstant.SaveKey.DevicePosition);

            string[] splitPos = posString.Split('*');

            foreach(var pos in splitPos)
            {
                string[] vector = pos.Split(',');
                
                if (vector.Length < 4) continue;

                Vector3 position = new Vector3(float.Parse(vector[1]), float.Parse(vector[2]), float.Parse(vector[3]));
                dict.Add(vector[0], position);
            }
        }
        return dict;
    }

    public void SaveStringData(string key, string content)
    {
        PlayerPrefs.SetString(key, content);
        PlayerPrefs.Save(); 
    }

    public string ReadStringData(string key)
    {
        if (!PlayerPrefs.HasKey(key)) return "";

        return PlayerPrefs.GetString(key);
    }

    public void DeleteSave(string key)
    {
        if (!PlayerPrefs.HasKey(key)) return;
        PlayerPrefs.DeleteKey(key);
    }
    
    public void DeleteSave() => PlayerPrefs.DeleteAll();
    
}

#region Editor Function

#if UNITY_EDITOR

[UnityEditor.CustomEditor(typeof(SaveManager))]
public class SaveManagerEditor : UnityEditor.Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        var saveManager = (SaveManager) target;

        GUILayout.Space(10);
        GUILayout.Label("Editor Function:");
        if (GUILayout.Button("Remove All Save"))
        {
            saveManager.DeleteSave();
        }
    }
}
#endif

#endregion
