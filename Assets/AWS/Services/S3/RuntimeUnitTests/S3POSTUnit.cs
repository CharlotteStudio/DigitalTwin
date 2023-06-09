using System.Collections;
using UnityEngine;
using UnityEngine.TestTools;
using SquareBeam.AWS.S3;
using UnityEngine.Networking;
using System.IO;
using System;

namespace SquareBeam.Tests.AWS.S3
{
    public class S3POSTUnit
    {
        public class Credentials
        {
            public string accessKey;
            public string secretKey;
            public string region;
            public string bucket;

            public Credentials(string accessKey, string secretKey, string region, string bucket)
            {
                this.accessKey = accessKey;
                this.secretKey = secretKey;
                this.region = region;
                this.bucket = bucket;
            }
        }

        [UnityTest]
        public IEnumerator S3UploadObjectTest()
        {
#if DEVELOPMENT
            var credentialsFile = "s3.json";
            var pathToCredentials = Path.Combine(Directory.GetParent(Application.dataPath).Parent.FullName, "credentials", credentialsFile);
            var credentials = JsonUtility.FromJson<Credentials>(File.ReadAllText(pathToCredentials));
#else
            var credentials = new Credentials("YOUR_ACCESS_KEY", "YOUR_SECRET_KEY", "YOUR_REGION", "YOUR_BUCKET_NAME");

#endif

            S3API api = new GameObject("S3API").AddComponent<S3API>();
            api.Setup(credentials.accessKey, credentials.secretKey, credentials.region);

            string testFile = "test.txt";

            var request = new UploadS3Request(credentials.bucket, testFile, new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes("Hello")))
            {
                onDone = (baseResponse) =>
                {
                    var response = (UploadS3Response)baseResponse;

                    if (response.Success)
                    {
                        Debug.Log("Success: Uploaded File");
                    }
                    else
                    {
                        ProcessException(response.Exception);
                    }
                }
            };

            yield return api.UploadObject(request);
        }

        [UnityTest]
        public IEnumerator S3UploadFileTest()
        {
#if DEVELOPMENT
            var credentialsFile = "s3.json";
            var pathToCredentials = Path.Combine(Directory.GetParent(Application.dataPath).Parent.FullName, "credentials", credentialsFile);
            var credentials = JsonUtility.FromJson<Credentials>(File.ReadAllText(pathToCredentials));
#else
            var credentials = new Credentials("YOUR_ACCESS_KEY", "YOUR_SECRET_KEY", "YOUR_REGION", "YOUR_BUCKET_NAME");

#endif

            S3API api = new GameObject("S3API").AddComponent<S3API>();
            api.Setup(credentials.accessKey, credentials.secretKey, credentials.region);

            string testFile = "test.json";
            string pathToFile = Path.Combine(Application.dataPath, "AWS", "Example", "TestFiles", testFile);

            var request = new UploadS3Request(credentials.bucket, testFile, new UploadHandlerFile(pathToFile))
            {
                onDone = (baseResponse) =>
                {
                    var response = (UploadS3Response)baseResponse;

                    if (response.Success)
                    {
                        Debug.Log("Success: Uploaded File");
                    }
                    else
                    {
                        ProcessException(response.Exception);
                    }
                }
            };

            yield return api.UploadObject(request);
        }

        void ProcessException(Exception e)
        {
            var exceptionType = e.GetType();
            Debug.LogError($"Failure: {exceptionType} was thrown!");

            if (exceptionType == typeof(S3Exception))
            {
                var exception = (S3Exception)e;

                Debug.LogError(exception.ToString());
            }
            else if (exceptionType == typeof(Exception))
            {
                Debug.LogException(e);
            }
            else
            {
                Debug.LogError($"Unsupported exception type {exceptionType}");
            }
        }
    }
}
