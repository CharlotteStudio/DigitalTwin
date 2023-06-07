using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class ExtensionDebug
{
    #region Debug Log

    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    public static void DebugLog<T>(this List<T> list)
        => list.ToArray().DebugLog();

    
    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    public static void DebugLog<T>(this HashSet<T> list)
        => list.ToArray().DebugLog();

    
    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    public static void DebugLog<T>(this IEnumerable<T> list)
        => list.ToArray().DebugLog();

    
    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    public static void DebugLog<T>(this T[] array)
        => System.String.Join(", ", array).DebugLog();
    
    
    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    public static void DebugLog<TKey, THeader, TValue>(this Dictionary<TKey, Dictionary<THeader, TValue>> dictionary)
    {
        System.Text.StringBuilder builder = new System.Text.StringBuilder();
        foreach (var keyValuePair in dictionary)
        {
            builder.Append("<color=blue>");
            builder.Append(keyValuePair.Key);
            builder.Append("</color>");
            UnityEngine.Debug.Log(builder.ToString());
            builder.Clear();
            keyValuePair.Value.DebugLog();
        }
    }

    
    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    public static void DebugLog<TKey, TValue>(this Dictionary<TKey, TValue> dictionary)
    {
        System.Text.StringBuilder builder = new System.Text.StringBuilder();
        foreach (var keyValuePair in dictionary)
        {
            builder.Append(keyValuePair.Key);
            builder.Append(" : ");
            builder.Append(keyValuePair.Value);
            builder.Append("\n");
        }
        builder.ToString().DebugLog();
        builder.Clear();
    }


    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    public static void DebugLog(this int value) =>
        Debug.Log(value);
    
    
    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    public static void DebugLog(this float value) =>
        Debug.Log(value);
    

    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    public static void DebugLog(this Vector2 value) =>
        Debug.Log(value);
    
    
    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    public static void DebugLog(this Vector3 value) =>
        Debug.Log(value);
    
    
    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    public static void DebugLog(this bool value)
        => value.ToString().DebugLog();

    
    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    public static void DebugLog(this string str)
        => Debug.Log(str);
    
    
    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    public static void DebugLogWarning(this string str)
        => Debug.LogWarning(str);
    
    
    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    public static void DebugLogError(this string str)
        => Debug.LogError(str);

    
    #endregion

    #region Draw Debug Ray

    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    public static void DrawDebugRay(this Vector3 startPos, Vector3 direction, Color color = default, float duration = 0, bool depthTest = false)
    {
        if (color == default) color = Color.green;

        UnityEngine.Debug.DrawRay(startPos, direction, color, duration, depthTest);
    }

    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    public static void DrawDebugLine(this Vector3 startPos, Vector3 endPos, Color color = default, float duration = 0, bool depthTest = false)
    {
        if (color == default) color = Color.green;
        
        UnityEngine.Debug.DrawLine(startPos, endPos, color, duration, depthTest);
    }
    
    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    public static void DrawDebugArrow(this Vector3 startPos, Vector3 direction = default, Color color = default, float angle = 3f, float headLength = 0.1f, float duration = 0, bool depthTest = false)
    {
        if (color == default)     color = Color.green;
        if (direction == default) direction = Vector3.forward;
        
        angle = Mathf.Abs(angle);
        if (angle > 0f)
        {
            headLength = Mathf.Abs(headLength);

            float length = direction.magnitude;
            
            if (length < headLength) headLength = length;
            
            Vector3 headDir = direction.normalized * (-length * headLength);
            DrawDebugCone(startPos + direction, headDir, color, angle, duration, depthTest);
        }

        DrawDebugRay(startPos, direction, color, duration, depthTest);
    }
    
    #endregion
    
    #region Draw Debug Point
    
    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    public static void DrawDebugPoint(this GameObject obj, Color color = default, float scale = 1.0f, float duration = 0, bool depthTest = false)
        => obj.transform.position.DrawDebugPoint(color, scale, duration, depthTest);


    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    public static void DrawDebugPoint(this Transform trans, Color color = default, float scale = 1.0f, float duration = 0, bool depthTest = false)
        => trans.position.DrawDebugPoint(color, scale, duration, depthTest);


    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    public static void DrawDebugPoint(this Vector3 startPos, Color color = default, float scale = 1.0f, float duration = 0, bool depthTest = false)
    {
        if (color == default) color = Color.green;

        DrawDebugRay(startPos + (Vector3.up * (scale * 0.5f)),-Vector3.up * scale, color, duration, depthTest);
        DrawDebugRay(startPos + (Vector3.right * (scale * 0.5f)), -Vector3.right * scale, color, duration, depthTest);
        DrawDebugRay(startPos + (Vector3.forward * (scale * 0.5f)), -Vector3.forward * scale, color, duration, depthTest);
    }

    #endregion

    #region Draw Debug 2D Shape
    
    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    public static void DrawDebugSector(this Vector3 startPos, Vector3 targetPos, float cloneAngle, float radius, Color color = default, float duration = 0, bool depthTest = false)
    {
        if (color == default) color = Color.green;

        var currentAngle = Mathf.Abs(Vector3.SignedAngle(startPos - Vector3.forward, startPos - targetPos , Vector3.up));
        var rayCount     = cloneAngle / 2 > 4 ? cloneAngle / 2 : 5;
        var signalAngle  = cloneAngle / rayCount;
        var startAngle   = currentAngle - cloneAngle / 2;
        
        for (int i = 0; i < rayCount + 1; i++)
        {
            float angle = (startAngle + (signalAngle * i)) * Mathf.Deg2Rad;

            float x = Mathf.Sin(angle) * radius;
            float z = Mathf.Cos(angle) * radius;
            
            var anglePosition = startPos + new Vector3(x, targetPos.y, z);

            startPos.DrawDebugLine(anglePosition, color, duration, depthTest);
        }
        startPos.DrawDebugArrow(targetPos, Color.red, duration: duration, depthTest: depthTest );
    }

    #endregion
    
    #region Draw Debug 3D Shape

    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    public static void DrawDebugCircle(this Vector3 startPos, Vector3 direction = default, Color color = default, float radius = 1.0f, float duration = 0, bool depthTest = false)
    {
        if (color == default) color = Color.green;
        if (direction == default) direction = Vector3.up;

        direction = direction.normalized * radius;

        Vector3 forward = Vector3.Slerp(direction, -direction, 0.5f);
        Vector3 right   = Vector3.Cross(direction, forward).normalized * radius;

        Matrix4x4 matrix = new Matrix4x4()
        {
            m00 = right.x,
            m10 = right.y,
            m20 = right.z,
            
            m01 = direction.x,
            m11 = direction.y,
            m21 = direction.z,

            m02 = forward.x,
            m12 = forward.y,
            m22 = forward.z
        };

        Vector3 startPoint = startPos + matrix.MultiplyPoint3x4(
            new Vector3(
                Mathf.Cos(0),
                0, 
                Mathf.Sin(0)));
        
        for (int i = 0; i <= 90; i++)
        {
            Vector3 endPoint = startPos + matrix.MultiplyPoint3x4(
                new Vector3(
                    Mathf.Cos((i * 4) * Mathf.Deg2Rad),
                    0f,
                    Mathf.Sin((i * 4) * Mathf.Deg2Rad)));

            DrawDebugLine(startPoint, endPoint, color, duration, depthTest);
            startPoint = endPoint;
        }
    }

    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    public static void DrawDebugCone(this Vector3 startPos, Vector3 direction, Color color = default, float angle = 45f, float duration = 0, bool depthTest = false, bool flip = false)
    {
        if (color == default) color = Color.green;

        if (flip)
        {
            startPos += direction;
            direction *= -1f;
        }

        float length = direction.magnitude;
        angle = Mathf.Clamp(angle, 0f, 90f);

        Vector3 forward     = direction;
        Vector3 up          = Vector3.Slerp(forward, -forward, 0.5f);
        Vector3 right       = Vector3.Cross(forward, up).normalized * length;
        Vector3 slerpVector = Vector3.Slerp(forward, up, angle / 90.0f);
        Plane farPlane      = new Plane(-direction, startPos + forward);
        Ray distRay         = new Ray(startPos, slerpVector);

        float distance;
        farPlane.Raycast(distRay, out distance);

        DrawDebugRay(startPos, slerpVector.normalized * distance, color, duration, depthTest);
        DrawDebugRay(startPos, Vector3.Slerp(forward, -up, angle / 90.0f).normalized * distance, color, duration, depthTest);
        DrawDebugRay(startPos, Vector3.Slerp(forward, right, angle / 90.0f).normalized * distance, color, duration, depthTest);
        DrawDebugRay(startPos, Vector3.Slerp(forward, -right, angle / 90.0f).normalized * distance, color, duration, depthTest);

        DrawDebugCircle(
            startPos + forward,
            direction,
            color,
            (forward - (slerpVector.normalized * distance)).magnitude,
            duration,
            depthTest);
        
        DrawDebugCircle(
            startPos + (forward * 0.5f),
            direction,
            color,
            ((forward * 0.5f) - (slerpVector.normalized * (distance * 0.5f))).magnitude,
            duration,
            depthTest);
    }

    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    public static void DrawDebugWireSphere(this Vector3 startPos, float radius = 1.0f, Color color = default, float duration = 0, bool depthTest = false)
    {
        if (color == default) color = Color.green;

        float angle = 20.0f;

        Vector3 x = new Vector3(startPos.x, startPos.y + radius * Mathf.Sin(0), startPos.z + radius * Mathf.Cos(0));
        Vector3 y = new Vector3(startPos.x + radius * Mathf.Cos(0), startPos.y, startPos.z + radius * Mathf.Sin(0));
        Vector3 z = new Vector3(startPos.x + radius * Mathf.Cos(0), startPos.y + radius * Mathf.Sin(0), startPos.z);

        for (int i = 1; i <= 24; i++)
        {
            var new_x = new Vector3(startPos.x, startPos.y + radius * Mathf.Sin(angle * i * Mathf.Deg2Rad), startPos.z + radius * Mathf.Cos(angle * i * Mathf.Deg2Rad));
            var new_y = new Vector3(startPos.x + radius * Mathf.Cos(angle * i * Mathf.Deg2Rad), startPos.y, startPos.z + radius * Mathf.Sin(angle * i * Mathf.Deg2Rad));
            var new_z = new Vector3(startPos.x + radius * Mathf.Cos(angle * i * Mathf.Deg2Rad), startPos.y + radius * Mathf.Sin(angle * i * Mathf.Deg2Rad), startPos.z);

            DrawDebugLine(x, new_x, color, duration, depthTest);
            DrawDebugLine(y, new_y, color, duration, depthTest);
            DrawDebugLine(z, new_z, color, duration, depthTest);

            x = new_x;
            y = new_y;
            z = new_z;
        }
    }
    
    #endregion
    
}
