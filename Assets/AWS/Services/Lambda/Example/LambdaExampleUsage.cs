using SquareBeam.AWS.Lambda;
using SquareBeam.AWS.Lambda.Models;
using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class LambdaExampleUsage : MonoBehaviour
{
    [Serializable]
    public class LambdaCredentials
    {
        public string accessKey;
        public string secretKey;
        public string region;
        public string function;
    }

    [SerializeField] private LambdaCredentials credentials;

    public Button InvokeLambdaButton;

    private LambdaAPI LambdaClient;
    void Awake()
    {
        InvokeLambdaButton.onClick.AddListener(() => StartCoroutine(OnInvokeLambda()));

#if DEVELOPMENT
        var credentialsFile = "lambda.json";
        var pathToCredentials = Path.Combine(Directory.GetParent(Application.dataPath).Parent.FullName, "credentials", credentialsFile);
        credentials = JsonUtility.FromJson<LambdaCredentials>(File.ReadAllText(pathToCredentials));
#endif

        LambdaClient = new GameObject("Lambda Client").AddComponent<LambdaAPI>();
        LambdaClient.Setup(credentials.accessKey, credentials.secretKey, credentials.region);
    }

    private IEnumerator OnInvokeLambda()
    {
        InvokeLambdaButton.interactable = false;
        yield return InvokeLambda();
        InvokeLambdaButton.interactable = true;
    }

    IEnumerator InvokeLambda()
    {
        string testFile = "lambda_payload.json";
        string payload = File.ReadAllText(Path.Combine(Application.dataPath, "AWS", "Services", "Lambda", "Example", "TestFiles", testFile));

        var request = new LambdaRequest(credentials.function, payload)
        {
            onDone = OnDone
        };


        void OnDone(LambdaResponse baseResponse)
        {
            var response = (LambdaResponse)baseResponse;

            if (response.Success)
            {
                Debug.Log($"Success: Response is {response.DownloadHandler.text}");
            }
            else
            {
                var exceptionType = response.Exception.GetType();
                Debug.LogError($"Failure: {exceptionType} was thrown!");

                if (exceptionType == typeof(LambdaException))
                {
                    var exception = (LambdaException)response.Exception;

                    Debug.LogError(exception.ToString());
                }
                else if (exceptionType == typeof(Exception))
                {
                    Debug.LogException(response.Exception);
                }
                else
                {
                    Debug.LogError($"Unsupported exception type {exceptionType}");
                }

            }

        }

        yield return LambdaClient.LambdaInvoke(request);
    }

}
