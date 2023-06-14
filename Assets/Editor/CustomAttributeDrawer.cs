using UnityEditor;
using UnityEngine;

/// <summary>
/// it must push in Editor folder
/// <see cref="https://answers.unity.com/questions/489942/how-to-make-a-readonly-property-in-inspector.html"/>Ref web>
/// </summary>
public class CustomAttributeDrawer : MonoBehaviour { }

#region Readonly Attribute

[CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
public class ReadOnlyDrawer : PropertyDrawer
{
    public override float GetPropertyHeight( SerializedProperty property, GUIContent label )
    {
        return EditorGUI.GetPropertyHeight( property, label, true );
    }
 
    public override void OnGUI( Rect position, SerializedProperty property, GUIContent label )
    {
        using (var scope = new EditorGUI.DisabledGroupScope(true))
        {
            EditorGUI.PropertyField( position, property, label, true );
        }
    }
}

// // Function 2 version
// [CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
// public class ReadOnlyDrawer : PropertyDrawer
// {
//     public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
//     {
//         return EditorGUI.GetPropertyHeight(property, label, true);
//     }
//  
//     public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
//     {
//         GUI.enabled = false;
//         EditorGUI.PropertyField(position, property, label, true);
//         GUI.enabled = true;
//     }
// }
//
// // Function 3 version
// [CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
// public class ReadOnlyDrawer : PropertyDrawer
// {
//     public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
//     { 
//         EditorGUI.BeginProperty(position, label, property);
//         EditorGUI.BeginDisabledGroup(true);
//         EditorGUI.PropertyField(position, property, label, true);
//         EditorGUI.EndDisabledGroup();
//         EditorGUI.EndProperty();
//     }
// }

#endregion

#region Showonly Attribute

[CustomPropertyDrawer(typeof(ShowOnlyAttribute))]
public class ShowOnlyDrawer : PropertyDrawer 
{
    public override void OnGUI(Rect position, SerializedProperty prop, GUIContent label)
    {
        string valueStr;
 
        switch (prop.propertyType)
        {
            case SerializedPropertyType.Integer:
                valueStr = prop.intValue.ToString();
                break;
            case SerializedPropertyType.Boolean:
                valueStr = prop.boolValue.ToString();
                break;
            case SerializedPropertyType.Float:
                valueStr = prop.floatValue.ToString("0.00000");
                break;
            case SerializedPropertyType.String:
                valueStr = prop.stringValue;
                break;
            default:
                valueStr = "(not supported)";
                break;
        }
 
        EditorGUI.LabelField(position,label.text, valueStr);
    }
}

#endregion

#region Readonly Group Attribute

[CustomPropertyDrawer( typeof( BeginReadOnlyGroupAttribute ) )]
public class BeginReadOnlyGroupDrawer : DecoratorDrawer
{
    public override float GetHeight() => 0;
 
    public override void OnGUI( Rect position )
    {
        // 同 ref website 唔同, 多了這一句
        GUI.enabled = false;
        EditorGUI.BeginDisabledGroup( true );
    }
}


[CustomPropertyDrawer( typeof( EndReadOnlyGroupAttribute ) )]
public class EndReadOnlyGroupDrawer : DecoratorDrawer
{
    public override float GetHeight() => 0;
 
    public override void OnGUI( Rect position )
    {
        EditorGUI.EndDisabledGroup();
        // 同 ref website 唔同, 多了這一句
        // 這句放前 下一個 variable 都會 readonly
        //    放後 下一個 variable 不會 readonly
        GUI.enabled = true;
    }
}

#endregion