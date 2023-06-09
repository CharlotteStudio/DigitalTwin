using System.Collections;
using UnityEngine;
using UnityEngine.TestTools;
using SquareBeam.AWS.Lambda;
using System.IO;
using SquareBeam.AWS.Lambda.Models;
using System;

namespace SquareBeam.Tests.AWS.Lambda
{
    public class Credentials
    {
        public string accessKey;
        public string secretKey;
        public string region;
        public string function;
        public string payload;

        public Credentials(string accessKey, string secretKey, string region, string function, string payload)
        {
            this.accessKey = accessKey;
            this.secretKey = secretKey;
            this.region = region;
            this.function = function;
            this.payload = payload;
        }
    }

    public class LambdaInvokeUnit
    {
        [UnityTest]
        public IEnumerator LambdaInvokeTest()
        {
#if DEVELOPMENT
            var credentialsFile = "lambda.json";
            var pathToCredentials = Path.Combine(Directory.GetParent(Application.dataPath).Parent.FullName, "credentials", credentialsFile);
            var credentials = JsonUtility.FromJson<Credentials>(File.ReadAllText(pathToCredentials));
#else
            var credentials = new Credentials("YOUR_ACCESS_KEY", "YOUR_SECRET_KEY", "YOUR_REGION", "YOUR_FUNCTION_NAME", "YOUR_PAYLOAD");

#endif
            LambdaAPI api = new LambdaAPI();
            api.Setup(credentials.region, credentials.accessKey, credentials.secretKey);

            var request = new LambdaRequest(credentials.function, credentials.payload)
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

            yield return api.LambdaInvoke(request);
        }
    }
}
