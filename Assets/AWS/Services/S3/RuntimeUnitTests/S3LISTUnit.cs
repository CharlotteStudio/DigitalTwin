using System.Collections;
using UnityEngine;
using UnityEngine.TestTools;
using SquareBeam.AWS.S3;
using System.IO;
using System;

namespace SquareBeam.Tests.AWS.S3
{
    public class S3LISTUnit
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
        public IEnumerator S3ListBucketTest()
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

            var request = new ListBucketS3Request(credentials.bucket)
            {
                onDone = (baseResponse) =>
                {
                    var response = (ListBucketS3Response)baseResponse;

                    if (response.Success)
                    {
                        Debug.Log($"Success: List is done. Keys Count is {response.Result.Contents.Count}");
                    }
                    else
                    {
                        ProcessException(response.Exception);
                    }
                }
            };

            yield return api.ListBucket(request);
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
