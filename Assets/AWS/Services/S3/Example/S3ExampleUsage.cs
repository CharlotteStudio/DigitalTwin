using SquareBeam.AWS.S3;
using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class S3ExampleUsage : MonoBehaviour
{
    [Serializable]
    public class S3Credentials
    {
        public string accessKey;
        public string secretKey;
        public string region;
        public string bucket;
    }

    [SerializeField] private S3Credentials credentials;

    public Button ListBucketButton;
    public Button ListBucketsButton;
    public Button CreateBucketButton;
    public Button DeleteBucketButton;

    public Button GetObjectHeadButton;
    public Button DownloadObjectButton;

    public Button UploadObjectButton;
    public Button DeleteObjectButton;


    private S3API S3Client;
    void Awake()
    {
        ListBucketButton.onClick.AddListener(() => StartCoroutine(OnListBucket()));
        ListBucketsButton.onClick.AddListener(() => StartCoroutine(OnListBuckets()));
        CreateBucketButton.onClick.AddListener(() => StartCoroutine(OnCreateBucket()));
        DeleteBucketButton.onClick.AddListener(() => StartCoroutine(OnDeleteBucket()));

        GetObjectHeadButton.onClick.AddListener(() => StartCoroutine(OnHeadObject()));
        DownloadObjectButton.onClick.AddListener(() => StartCoroutine(OnDownloadObject()));
        UploadObjectButton.onClick.AddListener(() => StartCoroutine(OnUploadObject()));
        DeleteObjectButton.onClick.AddListener(() => StartCoroutine(OnDeleteObject()));

#if DEVELOPMENT
        var credentialsFile = "s3.json";
        var pathToCredentials = Path.Combine(Directory.GetParent(Application.dataPath).Parent.FullName, "credentials", credentialsFile);
        credentials = JsonUtility.FromJson<S3Credentials>(File.ReadAllText(pathToCredentials));
#endif

        S3Client = new GameObject("S3 Client").AddComponent<S3API>();
        S3Client.Setup(credentials.accessKey, credentials.secretKey, credentials.region);
    }

    private IEnumerator OnListBucket()
    {
        ListBucketButton.interactable = false;
        yield return ListBucket();
        ListBucketButton.interactable = true;
    }

    private IEnumerator OnListBuckets()
    {
        ListBucketsButton.interactable = false;
        yield return ListBuckets();
        ListBucketsButton.interactable = true;
    }

    private IEnumerator OnCreateBucket()
    {
        CreateBucketButton.interactable = false;
        yield return CreateBucket();
        CreateBucketButton.interactable = true;
    }

    private IEnumerator OnDeleteBucket()
    {
        DeleteBucketButton.interactable = false;
        yield return DeleteBucket();
        DeleteBucketButton.interactable = true;
    }

    private IEnumerator OnHeadObject()
    {
        GetObjectHeadButton.interactable = false;
        yield return Head();
        GetObjectHeadButton.interactable = true;
    }

    private IEnumerator OnDownloadObject()
    {
        DownloadObjectButton.interactable = false;
        yield return Download();
        DownloadObjectButton.interactable = true;
    }

    private IEnumerator OnUploadObject()
    {
        UploadObjectButton.interactable = false;
        yield return Upload();
        UploadObjectButton.interactable = true;
    }


    private IEnumerator OnDeleteObject()
    {
        DeleteObjectButton.interactable = false;
        yield return Delete();
        DeleteObjectButton.interactable = true;
    }

    IEnumerator Head()
    {
        string testFile = "test.json";

        var request = new HeadS3Request(credentials.bucket, testFile)
        {
            onDone = OnDone
        };

        void OnDone(S3Response baseResponse)
        {
            var response = (HeadS3Response)baseResponse;

            if (response.Success)
            {
                Debug.Log("Success: Object Head is Received");
                Debug.Log(response.ToString());
            }
            else
            {
                var exceptionType = response.Exception.GetType();
                Debug.LogError($"Failure: {exceptionType} was thrown!");

                if (exceptionType == typeof(S3Exception))
                {
                    var exception = (S3Exception)response.Exception;

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

        yield return S3Client.GetObjectHead(request);
    }

    IEnumerator Download()
    {
        string testFile = "test.png";
        string downloadPath = Path.Combine(Application.persistentDataPath, testFile);

        var request = new DownloadS3Request(credentials.bucket, testFile, new DownloadHandlerFile(downloadPath))
        {
            onProgress = (progress) => Debug.Log($"Progress: {progress.Progress}"),
            onDone = OnDone
        };

        void OnDone(S3Response baseResponse)
        {
            var response = (DownloadS3Response)baseResponse;

            if (response.Success)
            {
                Debug.Log($"Success: Downloaded File at {downloadPath}");
            }
            else
            {
                var exceptionType = response.Exception.GetType();
                Debug.LogError($"Failure: {exceptionType} was thrown!");

                if (exceptionType == typeof(S3Exception))
                {
                    var exception = (S3Exception)response.Exception;

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

        yield return S3Client.DownloadObject(request);
    }

    IEnumerator Upload()
    {
        string testFile = "test.json";
        string pathToFile = Path.Combine(Application.dataPath, "AWS", "Services", "S3", "Example", "TestFiles", testFile);

        var request = new UploadS3Request(credentials.bucket, testFile, new UploadHandlerFile(pathToFile))
        {
            onProgress = (progress) => Debug.Log($"Progress: {progress.Progress}"),
            onDone = OnDone
        };

        void OnDone(S3Response baseResponse)
        {
            var response = (UploadS3Response)baseResponse;

            if (response.Success)
            {
                Debug.Log($"Success: File {testFile} is uploaded as {testFile} to {credentials.bucket} Bucket");
            }
            else
            {
                var exceptionType = response.Exception.GetType();
                Debug.LogError($"Failure: {exceptionType} was thrown!");

                if (exceptionType == typeof(S3Exception))
                {
                    var exception = (S3Exception)response.Exception;

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

        yield return S3Client.UploadObject(request);
    }

    IEnumerator Delete()
    {
        string testFile = "test.png";
        string fileKey = "test_copy.png";
        string pathToFile = Path.Combine(Application.dataPath, "AWS", "Services", "S3", "Example", "TestFiles", testFile);

        var uploadRequest = new UploadS3Request(credentials.bucket, fileKey, new UploadHandlerFile(pathToFile))
        {
            onProgress = (progress) => Debug.Log($"Progress: {progress.Progress}"),
            onDone = OnDone
        };

        void OnDone(S3Response baseResponse)
        {
            var response = (UploadS3Response)baseResponse;

            if (response.Success)
            {
                Debug.Log($"Success: File {testFile} is uploaded as {fileKey} to {credentials.bucket} Bucket");
            }
            else
            {
                var exceptionType = response.Exception.GetType();
                Debug.LogError($"Failure: {exceptionType} was thrown!");

                if (exceptionType == typeof(S3Exception))
                {
                    var exception = (S3Exception)response.Exception;

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

        yield return S3Client.UploadObject(uploadRequest);

        var deleteRequest = new DeleteObjectS3Request(credentials.bucket, fileKey)
        {
            onDone = (response) => {
                if (response.Success)
                {
                    Debug.Log($"Success: File {fileKey} is is deleted from {credentials.bucket} Bucket!");
                }
                else
                {
                    var exceptionType = response.Exception.GetType();
                    Debug.LogError($"Failure: {exceptionType} was thrown!");

                    if (exceptionType == typeof(S3Exception))
                    {
                        var exception = (S3Exception)response.Exception;

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
        };

        yield return S3Client.DeleteObject(deleteRequest);
    }

    IEnumerator ListBucket()
    {
        var request = new ListBucketS3Request(credentials.bucket, MaxKeys: 5)
        {
            onDone = OnDone
        };

        yield return S3Client.ListBucket(request);

        void OnDone(S3Response baseResponse)
        {
            var response = (ListBucketS3Response)baseResponse;

            if (response.Success)
            {
                Debug.Log($"Success: List is done. Keys Count is {response.Result.Contents.Count}");
                response.Result.Contents.ForEach(x => Debug.Log(x.Key));
            }
            else
            {
                var exceptionType = response.Exception.GetType();
                Debug.LogError($"Failure: {exceptionType} was thrown!");

                if (exceptionType == typeof(S3Exception))
                {
                    var exception = (S3Exception)response.Exception;

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
    }

    IEnumerator ListBuckets()
    {
        var request = new ListBucketsS3Request()
        {
            onDone = OnDone
        };

        yield return S3Client.ListBuckets(request);

        void OnDone(S3Response baseResponse)
        {
            var response = (ListBucketsS3Response)baseResponse;

            if (response.Success)
            {
                Debug.Log($"Success: List Buckets is done. Total Buckets Count is {response.Result.Buckets.Count}");
                response.Result.Buckets.ForEach(x => Debug.Log(x.Name));
            }
            else
            {
                var exceptionType = response.Exception.GetType();
                Debug.LogError($"Failure: {exceptionType} was thrown!");

                if (exceptionType == typeof(S3Exception))
                {
                    var exception = (S3Exception)response.Exception;

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
    }

    IEnumerator CreateBucket()
    {
        var request = new CreateBucketS3Request("abgdeleteme", "eu-north-1");
        request.onDone = OnDone;

        yield return S3Client.CreateBucket(request);

        void OnDone(S3Response baseResponse)
        {
            var response = (CreateBucketS3Response)baseResponse;

            if (response.Success)
            {
                Debug.Log($"Success: Bucket {request.Bucket} is created.");
            }
            else
            {
                var exceptionType = response.Exception.GetType();
                Debug.LogError($"Failure: {exceptionType} was thrown!");

                if (exceptionType == typeof(S3Exception))
                {
                    var exception = (S3Exception)response.Exception;

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
    }

    IEnumerator DeleteBucket()
    {
        var request = new DeleteBucketS3Request("abgdeleteme", "eu-north-1");
        request.onDone = OnDone;

        yield return S3Client.DeleteBucket(request);

        void OnDone(S3Response baseResponse)
        {
            var response = (DeleteBucketS3Response)baseResponse;

            if (response.Success)
            {
                Debug.Log($"Success: Bucket {request.Bucket} is deleted.");
            }
            else
            {
                var exceptionType = response.Exception.GetType();
                Debug.LogError($"Failure: {exceptionType} was thrown!");

                if (exceptionType == typeof(S3Exception))
                {
                    var exception = (S3Exception)response.Exception;

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
    }

}
