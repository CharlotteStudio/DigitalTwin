using System.Collections;
using UnityEngine;
using UnityEngine.TestTools;
using SquareBeam.AWS.S3;
using UnityEngine.Networking;
using System.IO;
using System;

namespace SquareBeam.Tests.AWS.S3
{
    public class S3GETUnit
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
        public IEnumerator S3GetObjectHeadTest()
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

            var request = new HeadS3Request(credentials.bucket, "test.file")
            {
                onDone = (baseResponse) =>
                {
                    var response = (HeadS3Response)baseResponse;

                    if (response.Success)
                    {
                        Debug.Log($"Success: Requested File Size is {response.Headers["Content-Length"]}");
                    }
                    else
                    {
                        ProcessException(response.Exception);
                    }
                }
            };

            yield return api.GetObjectHead(request);
        }


        [UnityTest]
        public IEnumerator S3GetObjectTest()
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

            var downloadObjectBuffer = new DownloadHandlerBuffer();

            var request = new DownloadS3Request(credentials.bucket, testFile, downloadObjectBuffer)
            {
                onProgress = (progress) => Debug.Log(progress.Progress),
                onDone = (baseResponse) =>
                {
                    var response = (DownloadS3Response)baseResponse;

                    if (response.Success)
                    {
                        Debug.Log($"Success: Object is received. Length is {downloadObjectBuffer.data.Length}");
                    }
                    else
                    {
                        ProcessException(response.Exception);
                    }
                }
            };

            yield return api.DownloadObject(request);
        }

        [UnityTest]
        public IEnumerator S3DownloadObjectToFileTest()
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
            string downloadPath = Path.Combine(Application.dataPath, "AWS", "Example", "TestFiles", testFile);

            var request = new DownloadS3Request(credentials.bucket, testFile, new DownloadHandlerFile(downloadPath))
            {
                onProgress = (progress) => Debug.Log(progress.Progress),
                onDone = (baseResponse) =>
                {
                    var response = (DownloadS3Response)baseResponse;

                    if (response.Success)
                    {
                        Debug.Log($"Success: Downloaded File at {downloadPath}");
                    }
                    else
                    {
                        ProcessException(response.Exception);
                    }
                }
            };

            yield return api.DownloadObject(request);
        }
        
        [UnityTest]
        public IEnumerator S3DownloadObjectToAudioAndPlayTest()
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

            string testFile = "test.wav";

            var audioClipBuffer = new DownloadHandlerAudioClip(string.Empty, AudioType.WAV);

            var request = new DownloadS3Request(credentials.bucket, testFile, audioClipBuffer)
            {
                onProgress = (progress) => Debug.Log(progress.Progress),
                onDone = (baseResponse) =>
                {
                    var response = (DownloadS3Response)baseResponse;

                    if (response.Success)
                    {
                        Debug.Log($"Success: Downloaded Audio File");
                    }
                    else
                    {
                        ProcessException(response.Exception);
                    }
                }
            };

            yield return api.DownloadObject(request);
            new GameObject("AudioListener").AddComponent<AudioListener>();
            AudioSource.PlayClipAtPoint(audioClipBuffer.audioClip, Vector3.zero);

            yield return new WaitForSecondsRealtime(audioClipBuffer.audioClip.length);
        }

        [UnityTest]
        public IEnumerator S3DownloadObjectToTextureTest()
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

            string testFile = "test.png";

            var textureBuffer = new DownloadHandlerTexture();

            var request = new DownloadS3Request(credentials.bucket, testFile, textureBuffer)
            {
                onProgress = (progress) => Debug.Log(progress),
                onDone = (response) => Debug.Log($"Downloaded Texture, Texture size is {textureBuffer.texture.width}x{textureBuffer.texture.height}, Format is {textureBuffer.texture.format}!")
            };

            yield return api.DownloadObject(request);

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
