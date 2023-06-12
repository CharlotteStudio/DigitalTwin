using System;
using System.IO;
using System.Collections;
using UnityEngine;
using SquareBeam.AWS.Lambda;
using SquareBeam.AWS.Lambda.Models;

public class AWSManager : MonoBehaviour
{
    [SerializeField] private LambdaAPI _LambdaClient;
    
    void Awake()
    {
#if DEVELOPMENT
        var credentialsFile = "lambda.json";
        var pathToCredentials = Path.Combine(Directory.GetParent(Application.dataPath).Parent.FullName, "credentials", credentialsFile);
        credentials = JsonUtility.FromJson<LambdaCredentials>(File.ReadAllText(pathToCredentials));
#endif

        SetUpLambdaClient();
    }

    public void SetUpLambdaClient()
    {
        _LambdaClient.Setup(
            MyConstant.AWSService.AccessKey,
            MyConstant.AWSService.SecretKey,
            MyConstant.AWSService.Region);
    }

    public void TryGetDeviceCurrentValue()
    {
        StartCoroutine(InvokeLambda(MyConstant.AWSService.LambdaFunction.GetDeviceCurrentValue));
    }
    
    private IEnumerator InvokeLambda(string lambdaFunctionName)
    {
        string testFile = "lambda_payload.json";
        string payload = File.ReadAllText(
            Path.Combine(Application.dataPath, "AWS", "Services", "Lambda", "Example", "TestFiles", testFile));
        
        var request = new LambdaRequest(lambdaFunctionName, payload)
        {
            onDone = OnDone
        };
        
        void OnDone(LambdaResponse baseResponse)
        {
            var response = (LambdaResponse)baseResponse;

            if (response.Success)
            {
                $"Success: Response is {response.DownloadHandler.text}".DebugLog();
            }
            else
            {
                var exceptionType = response.Exception.GetType();
                $"Failure: {exceptionType} was thrown!".DebugLogError();

                if (exceptionType == typeof(LambdaException))
                {
                    var exception = (LambdaException)response.Exception;

                    exception.ToString().DebugLogError();
                }
                else if (exceptionType == typeof(Exception))
                {
                    Debug.LogException(response.Exception);
                }
                else
                    $"Unsupported exception type {exceptionType}".DebugLogError();
            }
        }
        yield return _LambdaClient.LambdaInvoke(request);
    }
}

#region Editor Function

#if UNITY_EDITOR

[UnityEditor.CustomEditor(typeof(AWSManager))]
public class AWSManagerEditor : UnityEditor.Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        var awsManager = (AWSManager) target;

        if (GUILayout.Button("Try Get Device Current Value"))
        {
            awsManager.SetUpLambdaClient();
            awsManager.TryGetDeviceCurrentValue();
        }
    }
}
#endif

#endregion