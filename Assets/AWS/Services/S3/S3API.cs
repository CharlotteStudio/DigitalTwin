using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace SquareBeam.AWS.S3
{
    public class S3API : MonoBehaviour
    {
        internal readonly string awsEndpoint = "amazonaws.com";
        internal readonly string service = "s3";


        private string awsAccessKey;
        private string awsSecretKey;

        internal string region;

        #region Initialization

        public virtual void Setup(string awsAccessKey, string awsSecretKey, string region)
        {
            if (string.IsNullOrEmpty(awsAccessKey) || string.IsNullOrEmpty(awsSecretKey))
            {
                Debug.LogError("Access Key or SecretKey is Null or Empty");
                return;
            }

            this.awsAccessKey = awsAccessKey;
            this.awsSecretKey = awsSecretKey;
            this.region = region;
        }


        #endregion

        #region API

        public IEnumerator GetObjectHead(HeadS3Request request)
        {
            const string method = "HEAD";

            var accessKey = awsAccessKey;
            var secretKey = awsSecretKey;

            if (!string.IsNullOrEmpty(request.awsAccessKey) && !string.IsNullOrEmpty(request.awsSecretKey))
            {
                accessKey = request.awsAccessKey;
                secretKey = request.awsSecretKey;
            }

            if (!string.IsNullOrEmpty(request.region))
            {
                region = request.region;
            }

            if (string.IsNullOrEmpty(accessKey) || string.IsNullOrEmpty(secretKey))
            {
                Debug.LogError("Access Key or SecretKey is Null or Empty. Use Setup() or provide credential in S3Request");
                yield break;
            }

            var host = string.Join(".", new string[] { request.Bucket, service, region, awsEndpoint });
            var timestamp = DateTime.UtcNow;
            var amzdate = timestamp.ToString("yyyyMMddTHHmmssZ");
            var dateStamp = timestamp.ToString("yyyyMMdd");

            var payloadHash = "UNSIGNED-PAYLOAD";
            var canonicalQueryString = string.Empty;

            var headers = new Dictionary<string, string>()
            {
                { "host", host },
                { "x-amz-content-sha256", payloadHash },
                { "x-amz-date", amzdate }
            };

            var urlEncodedKey = Uri.EscapeUriString(request.Key);

            var uri = "https://" + string.Join("/", new string[] { host, urlEncodedKey.Replace("%20", "+") });

            UnityWebRequest www = new UnityWebRequest(uri)
            {
                method = method,
                certificateHandler = new ForceAcceptAll(),
                disposeCertificateHandlerOnDispose = true
            };

            www.SetRequestHeader("x-amz-content-sha256", payloadHash);
            www.SetRequestHeader("x-amz-date", amzdate);
            www.SetRequestHeader("Authorization", AWSS4Signer.SignRequest(awsAccessKey, awsSecretKey, service, region, method, urlEncodedKey, headers, canonicalQueryString, payloadHash, dateStamp, amzdate));

            yield return www.SendWebRequest();

            var responseCode = (int)www.responseCode;

            if (www.isNetworkError || www.isHttpError)
            {
                var exception = new S3Exception(
                    description: www.error,
                    bucket: request.Bucket,
                    key: request.Key,
                    headers: www.GetResponseHeaders()
                );

                var response = new HeadS3Response(responseCode: responseCode, exception: exception);

                request.onDone?.Invoke(response);
            }
            else
            {
                var response = new HeadS3Response(success: true, responseCode: responseCode, headers: www.GetResponseHeaders());
                request.onDone?.Invoke(response);
            }
        }

        public IEnumerator DownloadObject(DownloadS3Request request)
        {
            const string method = "GET";

            var accessKey = awsAccessKey;
            var secretKey = awsSecretKey;

            if (!string.IsNullOrEmpty(request.awsAccessKey) && !string.IsNullOrEmpty(request.awsSecretKey))
            {
                accessKey = request.awsAccessKey;
                secretKey = request.awsSecretKey;
            }

            if (!string.IsNullOrEmpty(request.region))
            {
                region = request.region;
            }

            if (string.IsNullOrEmpty(accessKey) || string.IsNullOrEmpty(secretKey))
            {
                Debug.LogError("Access Key or SecretKey is Null or Empty. Use Setup() or provide credential in S3Request");
                yield break;
            }

            var host = string.Join(".", new string[] { request.Bucket, service, region, awsEndpoint });
            var timestamp = DateTime.UtcNow;
            var amzdate = timestamp.ToString("yyyyMMddTHHmmssZ");
            var dateStamp = timestamp.ToString("yyyyMMdd");

            var payloadHash = "UNSIGNED-PAYLOAD";
            var canonicalQueryString = string.Empty;

            var headers = new Dictionary<string, string>()
            {
                { "host", host },
                { "x-amz-content-sha256", payloadHash },
                { "x-amz-date", amzdate }
            };

            var urlEncodedKey = Uri.EscapeUriString(request.Key);

            var uri = "https://" + string.Join("/", new string[] { host, urlEncodedKey.Replace("%20", "+") });

            UnityWebRequest www = new UnityWebRequest(uri)
            {
                method = method,
                downloadHandler = request.downloadHandler,
                certificateHandler = new ForceAcceptAll(),
                disposeCertificateHandlerOnDispose = true
            };

            www.SetRequestHeader("x-amz-content-sha256", payloadHash);
            www.SetRequestHeader("x-amz-date", amzdate);
            www.SetRequestHeader("Authorization", AWSS4Signer.SignRequest(awsAccessKey, awsSecretKey, service, region, method, urlEncodedKey, headers, canonicalQueryString, payloadHash, dateStamp, amzdate));

            www.SendWebRequest();

            while (!www.isDone)
            {
                request.onProgress?.Invoke(new OpertationProgress(www.downloadProgress, www.downloadedBytes));
                yield return null;
            }

            request.onProgress?.Invoke(new OpertationProgress(1, www.downloadedBytes));

            var responseCode = (int)www.responseCode;

            if (www.isNetworkError || www.isHttpError)
            {
                var exception = new S3Exception(
                    description: www.error,
                    bucket: request.Bucket,
                    key: request.Key,
                    headers: www.GetResponseHeaders()
                );

                var response = new DownloadS3Response(responseCode: responseCode, exception: exception);

                request.onDone?.Invoke(response);
            }
            else
            {
                var response = new DownloadS3Response(success: true, responseCode: responseCode);
                request.onDone?.Invoke(response);
            }
        }

        public IEnumerator UploadObject(UploadS3Request request)
        {
            const string method = "PUT";

            var accessKey = awsAccessKey;
            var secretKey = awsSecretKey;

            if (!string.IsNullOrEmpty(request.awsAccessKey) && !string.IsNullOrEmpty(request.awsSecretKey))
            {
                accessKey = request.awsAccessKey;
                secretKey = request.awsSecretKey;
            }

            if (!string.IsNullOrEmpty(request.region))
            {
                region = request.region;
            }

            if (string.IsNullOrEmpty(accessKey) || string.IsNullOrEmpty(secretKey))
            {
                Debug.LogError("Access Key or SecretKey is Null or Empty. Use Setup() or provide credential in S3Request");
                yield break;
            }

            var host = string.Join(".", new string[] { request.Bucket, service, region, awsEndpoint });
            var timestamp = DateTime.UtcNow;
            var amzdate = timestamp.ToString("yyyyMMddTHHmmssZ");
            var dateStamp = timestamp.ToString("yyyyMMdd");

            var payloadHash = "UNSIGNED-PAYLOAD";
            var canonicalQueryString = string.Empty;

            var headers = new Dictionary<string, string>()
            {
                { "host", host },
                { "x-amz-content-sha256", payloadHash },
                { "x-amz-date", amzdate }
            };

            var urlEncodedKey = Uri.EscapeUriString(request.Key);

            var uri = "https://" + string.Join("/", new string[] { host, urlEncodedKey.Replace("%20", "+") });

            UnityWebRequest www = new UnityWebRequest(uri)
            {
                method = method,
                uploadHandler = request.uploadHandler,
                certificateHandler = new ForceAcceptAll(),
                disposeCertificateHandlerOnDispose = true
            };

            www.SetRequestHeader("x-amz-content-sha256", payloadHash);
            www.SetRequestHeader("x-amz-date", amzdate);
            www.SetRequestHeader("Authorization", AWSS4Signer.SignRequest(awsAccessKey, awsSecretKey, service, region, method, urlEncodedKey, headers, canonicalQueryString, payloadHash, dateStamp, amzdate));

            www.SendWebRequest();

            while (!www.isDone)
            {
                request.onProgress?.Invoke(new OpertationProgress(www.uploadProgress, www.uploadedBytes));
                yield return null;
            }

            request.onProgress?.Invoke(new OpertationProgress(1, www.uploadedBytes));

            var responseCode = (int)www.responseCode;

            if (www.isNetworkError || www.isHttpError)
            {
                var exception = new S3Exception(
                    description: www.error,
                    bucket: request.Bucket,
                    key: request.Key,
                    headers: www.GetResponseHeaders()
                );

                var response = new UploadS3Response(responseCode: responseCode, exception: exception);

                request.onDone?.Invoke(response);

            }
            else
            {
                var response = new UploadS3Response(success: true, responseCode: responseCode);
                request.onDone?.Invoke(response);
            }
        }

        public IEnumerator DeleteObject(DeleteObjectS3Request request)
        {
            const string method = "DELETE";

            var accessKey = awsAccessKey;
            var secretKey = awsSecretKey;

            if (!string.IsNullOrEmpty(request.awsAccessKey) && !string.IsNullOrEmpty(request.awsSecretKey))
            {
                accessKey = request.awsAccessKey;
                secretKey = request.awsSecretKey;
            }

            if (!string.IsNullOrEmpty(request.region))
            {
                region = request.region;
            }

            if (string.IsNullOrEmpty(accessKey) || string.IsNullOrEmpty(secretKey))
            {
                Debug.LogError("Access Key or SecretKey is Null or Empty. Use Setup() or provide credential in S3Request");
                yield break;
            }

            var host = string.Join(".", new string[] { request.Bucket, service, region, awsEndpoint });
            var timestamp = DateTime.UtcNow;
            var amzdate = timestamp.ToString("yyyyMMddTHHmmssZ");
            var dateStamp = timestamp.ToString("yyyyMMdd");

            var payloadHash = "UNSIGNED-PAYLOAD";
            var canonicalQueryString = string.Empty;

            var headers = new Dictionary<string, string>()
            {
                { "host", host },
                { "x-amz-content-sha256", payloadHash },
                { "x-amz-date", amzdate }
            };

            var urlEncodedKey = Uri.EscapeUriString(request.Key);

            var uri = "https://" + string.Join("/", new string[] { host, urlEncodedKey.Replace("%20", "+") });

            UnityWebRequest www = new UnityWebRequest(uri)
            {
                method = method,
                certificateHandler = new ForceAcceptAll(),
                disposeCertificateHandlerOnDispose = true
            };

            www.SetRequestHeader("x-amz-content-sha256", payloadHash);
            www.SetRequestHeader("x-amz-date", amzdate);
            www.SetRequestHeader("Authorization", AWSS4Signer.SignRequest(awsAccessKey, awsSecretKey, service, region, method, urlEncodedKey, headers, canonicalQueryString, payloadHash, dateStamp, amzdate));

            www.SendWebRequest();

            while (!www.isDone)
            {
                yield return null;
            }

            var responseCode = (int)www.responseCode;

            if (www.isNetworkError || www.isHttpError)
            {
                var exception = new S3Exception(
                    description: www.error,
                    bucket: request.Bucket,
                    key: request.Key,
                    headers: www.GetResponseHeaders()
                );

                var response = new DeleteObjectS3Response(responseCode: responseCode, exception: exception);

                request.onDone?.Invoke(response);
            }
            else
            {
                var response = new DeleteObjectS3Response(success: true, responseCode: responseCode);
                request.onDone?.Invoke(response);
            }
        }

        public IEnumerator ListBucket(ListBucketS3Request request)
        {
            const string method = "GET";

            var accessKey = awsAccessKey;
            var secretKey = awsSecretKey;

            if (!string.IsNullOrEmpty(request.awsAccessKey) && !string.IsNullOrEmpty(request.awsSecretKey))
            {
                accessKey = request.awsAccessKey;
                secretKey = request.awsSecretKey;
            }

            if (!string.IsNullOrEmpty(request.region))
            {
                region = request.region;
            }

            if (string.IsNullOrEmpty(accessKey) || string.IsNullOrEmpty(secretKey))
            {
                Debug.LogError("Access Key or SecretKey is Null or Empty. Use Setup() or provide credential in S3Request");
                yield break;
            }

            var host = string.Join(".", new string[] { request.Bucket, service, region, awsEndpoint });
            var timestamp = DateTime.UtcNow;
            var amzdate = timestamp.ToString("yyyyMMddTHHmmssZ");
            var dateStamp = timestamp.ToString("yyyyMMdd");

            var payloadHash = "UNSIGNED-PAYLOAD";
            var canonicalQueryString = string.Join("&", request.Query);

            var headers = new Dictionary<string, string>()
            {
                { "host", host },
                { "x-amz-content-sha256", payloadHash },
                { "x-amz-date", amzdate }
            };

            var uri = "https://" + string.Join("/", new string[] { host, "?" + canonicalQueryString });

            var downloadHandler = new DownloadHandlerBuffer();

            UnityWebRequest www = new UnityWebRequest(uri)
            {
                method = method,
                downloadHandler = downloadHandler,
                certificateHandler = new ForceAcceptAll(),
                disposeCertificateHandlerOnDispose = true
            };

            www.SetRequestHeader("x-amz-content-sha256", payloadHash);
            www.SetRequestHeader("x-amz-date", amzdate);
            www.SetRequestHeader("Authorization", AWSS4Signer.SignRequest(awsAccessKey, awsSecretKey, service, region, method, string.Empty, headers, canonicalQueryString, payloadHash, dateStamp, amzdate));

            www.SendWebRequest();

            while (!www.isDone)
            {
                yield return null;
            }

            var responseCode = (int)www.responseCode;

            if (www.isNetworkError || www.isHttpError)
            {
                var exception = new S3Exception(
                    description: www.error,
                    bucket: request.Bucket,
                    key: request.Key,
                    headers: www.GetResponseHeaders()

                );

                var response = new ListBucketS3Response(responseCode: responseCode, exception: exception);


                request.onDone?.Invoke(response);
            }
            else
            {
                var response = new ListBucketS3Response(success: true, responseCode: responseCode, downloadHandler: downloadHandler);
                request.onDone?.Invoke(response);
            }
        }

        public IEnumerator ListBuckets(ListBucketsS3Request request)
        {
            const string method = "GET";

            var accessKey = awsAccessKey;
            var secretKey = awsSecretKey;

            if (!string.IsNullOrEmpty(request.awsAccessKey) && !string.IsNullOrEmpty(request.awsSecretKey))
            {
                accessKey = request.awsAccessKey;
                secretKey = request.awsSecretKey;
            }

            if (!string.IsNullOrEmpty(request.region))
            {
                region = request.region;
            }

            if (string.IsNullOrEmpty(accessKey) || string.IsNullOrEmpty(secretKey))
            {
                Debug.LogError("Access Key or SecretKey is Null or Empty. Use Setup() or provide credential in S3Request");
                yield break;
            }

            var host = string.Join(".", new string[] { service, region, awsEndpoint });
            var timestamp = DateTime.UtcNow;
            var amzdate = timestamp.ToString("yyyyMMddTHHmmssZ");
            var dateStamp = timestamp.ToString("yyyyMMdd");

            var payloadHash = "UNSIGNED-PAYLOAD";
            var canonicalQueryString = string.Join("&", request.Query);

            var headers = new Dictionary<string, string>()
            {
                { "host", host },
                { "x-amz-content-sha256", payloadHash },
                { "x-amz-date", amzdate }
            };

            var uri = "https://" + string.Join("/", new string[] { host, "?" + canonicalQueryString });

            var downloadHandler = new DownloadHandlerBuffer();

            UnityWebRequest www = new UnityWebRequest(uri)
            {
                method = method,
                downloadHandler = downloadHandler,
                certificateHandler = new ForceAcceptAll(),
                disposeCertificateHandlerOnDispose = true
            };

            www.SetRequestHeader("x-amz-content-sha256", payloadHash);
            www.SetRequestHeader("x-amz-date", amzdate);
            www.SetRequestHeader("Authorization", AWSS4Signer.SignRequest(awsAccessKey, awsSecretKey, service, region, method, string.Empty, headers, canonicalQueryString, payloadHash, dateStamp, amzdate));

            www.SendWebRequest();

            while (!www.isDone)
            {
                yield return null;
            }

            var responseCode = (int)www.responseCode;

            if (www.isNetworkError || www.isHttpError)
            {
                var exception = new S3Exception(
                    description: www.error,
                    bucket: request.Bucket,
                    key: request.Key,
                    headers: www.GetResponseHeaders()

                );

                var response = new ListBucketsS3Response(responseCode: responseCode, exception: exception);


                request.onDone?.Invoke(response);
            }
            else
            {
                var response = new ListBucketsS3Response(success: true, responseCode: responseCode, downloadHandler: downloadHandler);
                request.onDone?.Invoke(response);
            }
        }

        public IEnumerator CreateBucket(CreateBucketS3Request request)
        {
            const string method = "PUT";

            var accessKey = awsAccessKey;
            var secretKey = awsSecretKey;

            if (!string.IsNullOrEmpty(request.awsAccessKey) && !string.IsNullOrEmpty(request.awsSecretKey))
            {
                accessKey = request.awsAccessKey;
                secretKey = request.awsSecretKey;
            }

            if (!string.IsNullOrEmpty(request.region))
            {
                region = request.region;
            }

            if (string.IsNullOrEmpty(accessKey) || string.IsNullOrEmpty(secretKey))
            {
                Debug.LogError("Access Key or SecretKey is Null or Empty. Use Setup() or provide credential in S3Request");
                yield break;
            }

            var host = string.Join(".", new string[] { request.Bucket, service, request.region, awsEndpoint });
            var timestamp = DateTime.UtcNow;
            var amzdate = timestamp.ToString("yyyyMMddTHHmmssZ");
            var dateStamp = timestamp.ToString("yyyyMMdd");

            var payloadHash = "UNSIGNED-PAYLOAD";
            var canonicalQueryString = string.Empty;

            var headers = new Dictionary<string, string>()
            {
                { "host", host },
                { "x-amz-content-sha256", payloadHash },
                { "x-amz-date", amzdate }
            };

            var uri = "https://" + string.Join("/", new string[] { host });

            var downloadHandler = new DownloadHandlerBuffer();
            var uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(request.BucketConfigXML));
            uploadHandler.contentType = "application/xml";

            UnityWebRequest www = new UnityWebRequest(uri)
            {
                method = method,
                downloadHandler = downloadHandler,
                uploadHandler = uploadHandler,
                certificateHandler = new ForceAcceptAll(),
                disposeCertificateHandlerOnDispose = true
            };

            www.SetRequestHeader("x-amz-content-sha256", payloadHash);
            www.SetRequestHeader("x-amz-date", amzdate);
            www.SetRequestHeader("Authorization", AWSS4Signer.SignRequest(awsAccessKey, awsSecretKey, service, region, method, string.Empty, headers, canonicalQueryString, payloadHash, dateStamp, amzdate));

            www.SendWebRequest();

            while (!www.isDone)
            {
                yield return null;
            }

            var responseCode = (int)www.responseCode;

            if (www.isNetworkError || www.isHttpError)
            {
                var exception = new S3Exception(
                    description: www.error,
                    bucket: request.Bucket,
                    key: request.Key,
                    headers: www.GetResponseHeaders()

                );

                var response = new CreateBucketS3Response(responseCode: responseCode, exception: exception);


                request.onDone?.Invoke(response);
            }
            else
            {
                var response = new CreateBucketS3Response(success: true, responseCode: responseCode, downloadHandler: downloadHandler);
                request.onDone?.Invoke(response);
            }
        }
        
        public IEnumerator DeleteBucket(DeleteBucketS3Request request)
        {
            const string method = "DELETE";

            var accessKey = awsAccessKey;
            var secretKey = awsSecretKey;

            if (!string.IsNullOrEmpty(request.awsAccessKey) && !string.IsNullOrEmpty(request.awsSecretKey))
            {
                accessKey = request.awsAccessKey;
                secretKey = request.awsSecretKey;
            }

            if (!string.IsNullOrEmpty(request.region))
            {
                region = request.region;
            }

            if (string.IsNullOrEmpty(accessKey) || string.IsNullOrEmpty(secretKey))
            {
                Debug.LogError("Access Key or SecretKey is Null or Empty. Use Setup() or provide credential in S3Request");
                yield break;
            }

            var host = string.Join(".", new string[] { request.Bucket, service, request.region, awsEndpoint });
            var timestamp = DateTime.UtcNow;
            var amzdate = timestamp.ToString("yyyyMMddTHHmmssZ");
            var dateStamp = timestamp.ToString("yyyyMMdd");

            var payloadHash = "UNSIGNED-PAYLOAD";
            var canonicalQueryString = string.Empty;

            var headers = new Dictionary<string, string>()
            {
                { "host", host },
                { "x-amz-content-sha256", payloadHash },
                { "x-amz-date", amzdate }
            };

            var uri = "https://" + string.Join("/", new string[] { host });

            var downloadHandler = new DownloadHandlerBuffer();

            UnityWebRequest www = new UnityWebRequest(uri)
            {
                method = method,
                downloadHandler = downloadHandler,
                certificateHandler = new ForceAcceptAll(),
                disposeCertificateHandlerOnDispose = true
            };

            www.SetRequestHeader("x-amz-content-sha256", payloadHash);
            www.SetRequestHeader("x-amz-date", amzdate);
            www.SetRequestHeader("Authorization", AWSS4Signer.SignRequest(awsAccessKey, awsSecretKey, service, region, method, string.Empty, headers, canonicalQueryString, payloadHash, dateStamp, amzdate));

            www.SendWebRequest();

            while (!www.isDone)
            {
                yield return null;
            }

            var responseCode = (int)www.responseCode;

            if (www.isNetworkError || www.isHttpError)
            {
                var exception = new S3Exception(
                    description: www.error,
                    bucket: request.Bucket,
                    key: request.Key,
                    headers: www.GetResponseHeaders()

                );

                var response = new DeleteBucketS3Response(responseCode: responseCode, exception: exception);


                request.onDone?.Invoke(response);
            }
            else
            {
                var response = new DeleteBucketS3Response(success: true, responseCode: responseCode, downloadHandler: downloadHandler);
                request.onDone?.Invoke(response);
            }


            #endregion

        }
    }
}