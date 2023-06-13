using System;
using System.IO;
using System.Collections;
using UnityEngine;
using SquareBeam.AWS.Lambda;
using SquareBeam.AWS.Lambda.Models;

public class AWSManager : ManagerBase<AWSManager>
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

    private void TestingLambdaFunction(string lambdaFunctionName)
    {
        string testFile = "lambda_payload.json";
        string payload = File.ReadAllText(
            Path.Combine(Application.dataPath, "AWS", "Services", "Lambda", "Example", "TestFiles", testFile));

        InvokeLambdaFunction(lambdaFunctionName, payload, OnReceivedEvent);

        void OnReceivedEvent(LambdaResponse response)
        {
            if (response.Success)
                $"Success: Response is :\n{response.DownloadHandler.text.ToPrettyPrintJsonString()}".DebugLog();
            else
                ResponseFail(response);
        }
    }
    
    public void InvokeLambdaFunction(string lambdaFunctionName, string payload, Action<LambdaResponse> onReceivedEvent)
    {
        StartCoroutine(InvokeLambda(
            lambdaFunctionName,
            payload,
            onReceivedEvent
        ));
    }

    private IEnumerator InvokeLambda(string lambdaFunctionName, string payload, Action<LambdaResponse> onReceivedEvent)
    {
        var request = new LambdaRequest(lambdaFunctionName, payload, onReceivedEvent);
        yield return _LambdaClient.LambdaInvoke(request);
    }

    public void ResponseFail(LambdaResponse response)
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

    #region Editor Testing Methods
    
    public void TryGetDeviceState() =>
        TestingLambdaFunction(MyConstant.AWSService.LambdaFunction.GetDeviceState);
    
    public void TryGetDeviceActive() =>
        TestingLambdaFunction(MyConstant.AWSService.LambdaFunction.GetDeviceActive);

    public void TryGetDeviceCurrentValue() =>
        TestingLambdaFunction(MyConstant.AWSService.LambdaFunction.GetDeviceCurrentValue);
    
    public void TryGetDeviceAllValue() => 
        TestingLambdaFunction(MyConstant.AWSService.LambdaFunction.GetDeviceValue);
    
    #endregion
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

        if (GUILayout.Button("Try Get Device State"))
        {
            awsManager.SetUpLambdaClient();
            awsManager.TryGetDeviceState();
        }

        if (GUILayout.Button("Try Get Device Active"))
        {
            awsManager.SetUpLambdaClient();
            awsManager.TryGetDeviceActive();
        }
        
        if (GUILayout.Button("Try Get Device Current Value"))
        {
            awsManager.SetUpLambdaClient();
            awsManager.TryGetDeviceCurrentValue();
        }
        
        if (GUILayout.Button("Try Get Device All Value"))
        {
            awsManager.SetUpLambdaClient();
            awsManager.TryGetDeviceAllValue();
        }
    }
}
#endif

#endregion