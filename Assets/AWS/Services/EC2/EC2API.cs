using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using SquareBeam.AWS.EC2.Models;
using UnityEngine.Networking;
using UnityEngine;

namespace SquareBeam.AWS.EC2
{
    public class EC2API : MonoBehaviour
    {
        internal readonly string awsEndpoint = "amazonaws.com";
        internal readonly string service = "ec2";

        internal string awsAccessKey;
        internal string awsSecretKey;

        internal string region;

        #region Initialization

        public virtual void Setup(string awsAccessKey, string awsSecretKey, string region)
        {
            if (string.IsNullOrEmpty(awsAccessKey) || string.IsNullOrEmpty(awsSecretKey))
            {
                Debug.LogError("Access Key or SecretKey is Null or Empty");
                return;
            }

           
            this.region = region;
            this.awsAccessKey = awsAccessKey;
            this.awsSecretKey = awsSecretKey;

        }


        #endregion

        #region API

        public IEnumerator RunInstance(RunEC2Instance request)
        {
            const string method = "POST";

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

            var payloadHash = AWSS4Signer.SHA256Hash(string.Empty);
            var canonicalQueryString = string.Join("&", request.Query);

            var headers = new Dictionary<string, string>()
            {
                { "host", host },
                { "x-amz-date", amzdate }
            };

            var uri = "https://" + string.Join("/", new string[] { host, "?" + canonicalQueryString });

            var downloadHandler = new DownloadHandlerBuffer();

            UnityWebRequest www = new UnityWebRequest(uri)
            {
                method = method,
                downloadHandler = downloadHandler,
                certificateHandler = new ForceAcceptAll()
            };

            www.SetRequestHeader("x-amz-date", amzdate);
            www.SetRequestHeader("Authorization", AWSS4Signer.SignRequest(awsAccessKey, awsSecretKey, service, region, method, string.Empty, headers, canonicalQueryString, payloadHash, dateStamp, amzdate));

            yield return www.SendWebRequest();

            var responseCode = (int)www.responseCode;

            if (www.isNetworkError || www.isHttpError)
            {
                var exception = new EC2Exception(
                    responseCode: (int)www.responseCode,
                    description: www.error,
                    functionName: request.FunctionName,
                    headers: www.GetResponseHeaders()
                );

                var response = new RunEC2Response(responseCode: (int)www.responseCode, exception: exception);

                request.onDone?.Invoke(response);
            }
            else
            {
                var response = new RunEC2Response(success: true, responseCode: (int)www.responseCode, headers: www.GetResponseHeaders(), downloadHandler: www.downloadHandler);
                request.onDone?.Invoke(response);
            }
        }

        public IEnumerator TerminateInstance(TerminateEC2Instance request)
        {
            const string method = "POST";

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

            var payloadHash = AWSS4Signer.SHA256Hash(string.Empty);
            var canonicalQueryString = string.Join("&", request.Query);

            var headers = new Dictionary<string, string>()
            {
                { "host", host },
                { "x-amz-date", amzdate }
            };

            var uri = "https://" + string.Join("/", new string[] { host, "?" + canonicalQueryString });

            var downloadHandler = new DownloadHandlerBuffer();

            UnityWebRequest www = new UnityWebRequest(uri)
            {
                method = method,
                downloadHandler = downloadHandler,
                certificateHandler = new ForceAcceptAll()
            };

            www.SetRequestHeader("x-amz-date", amzdate);
            www.SetRequestHeader("Authorization", AWSS4Signer.SignRequest(awsAccessKey, awsSecretKey, service, region, method, string.Empty, headers, canonicalQueryString, payloadHash, dateStamp, amzdate));

            yield return www.SendWebRequest();

            var responseCode = (int)www.responseCode;

            if (www.isNetworkError || www.isHttpError)
            {
                var exception = new EC2Exception(
                    responseCode: (int)www.responseCode,
                    description: www.error,
                    functionName: request.FunctionName,
                    headers: www.GetResponseHeaders()
                );

                var response = new TerminateEC2Response(responseCode: (int)www.responseCode, exception: exception);

                request.onDone?.Invoke(response);
            }
            else
            {
                var response = new TerminateEC2Response(success: true, responseCode: (int)www.responseCode, headers: www.GetResponseHeaders(), downloadHandler: www.downloadHandler);
                request.onDone?.Invoke(response);
            }
        }



        public IEnumerator StartInstance(StartEC2Instance request)
        {
            const string method = "POST";

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

            var payloadHash = AWSS4Signer.SHA256Hash(string.Empty);
            var canonicalQueryString = string.Join("&", request.Query);
            Debug.LogError(canonicalQueryString);
            var headers = new Dictionary<string, string>()
            {
                { "host", host },
                { "x-amz-date", amzdate }
            };

            var uri = "https://" + string.Join("/", new string[] { host, "?" + canonicalQueryString });

            var downloadHandler = new DownloadHandlerBuffer();

            UnityWebRequest www = new UnityWebRequest(uri)
            {
                method = method,
                downloadHandler = downloadHandler,
                certificateHandler = new ForceAcceptAll()
            };

            www.SetRequestHeader("x-amz-date", amzdate);
            www.SetRequestHeader("Authorization", AWSS4Signer.SignRequest(awsAccessKey, awsSecretKey, service, region, method, string.Empty, headers, canonicalQueryString, payloadHash, dateStamp, amzdate));
            Debug.LogError(AWSS4Signer.SignRequest(awsAccessKey, awsSecretKey, service, region, method, string.Empty, headers, canonicalQueryString, payloadHash, dateStamp, amzdate));
            
            yield return www.SendWebRequest();

            var responseCode = (int)www.responseCode;

            if (www.isNetworkError || www.isHttpError)
            {
                var exception = new EC2Exception(
                    responseCode: (int)www.responseCode,
                    description: www.error,
                    functionName: request.FunctionName,
                    headers: www.GetResponseHeaders()
                );

                var response = new StartEC2Response(responseCode: (int)www.responseCode, exception: exception);

                request.onDone?.Invoke(response);
            }
            else
            {
                var response = new StartEC2Response(success: true, responseCode: (int)www.responseCode, headers: www.GetResponseHeaders(), downloadHandler: www.downloadHandler);
                request.onDone?.Invoke(response);
            }
        }

        public IEnumerator StopInstance(StopEC2Instance request)
        {
            const string method = "POST";

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

            var payloadHash = AWSS4Signer.SHA256Hash(string.Empty); ;
            var canonicalQueryString = string.Join("&", request.Query);

            var headers = new Dictionary<string, string>()
            {
                { "host", host },
                { "x-amz-date", amzdate }
            };

            var uri = "https://" + string.Join("/", new string[] { host, "?" + canonicalQueryString });

            var downloadHandler = new DownloadHandlerBuffer();

            UnityWebRequest www = new UnityWebRequest(uri)
            {
                method = method,
                downloadHandler = downloadHandler,
                certificateHandler = new ForceAcceptAll()
            };

            www.SetRequestHeader("x-amz-date", amzdate);
            www.SetRequestHeader("Authorization", AWSS4Signer.SignRequest(awsAccessKey, awsSecretKey, service, region, method, string.Empty, headers, canonicalQueryString, payloadHash, dateStamp, amzdate));

            yield return www.SendWebRequest();

            var responseCode = (int)www.responseCode;

            if (www.isNetworkError || www.isHttpError)
            {
                var exception = new EC2Exception(
                    responseCode: (int)www.responseCode,
                    description: www.error,
                    functionName: request.FunctionName,
                    headers: www.GetResponseHeaders()
                );

                var response = new StopEC2Response(responseCode: (int)www.responseCode, exception: exception);

                request.onDone?.Invoke(response);
            }
            else
            {
                var response = new StopEC2Response(success: true, responseCode: (int)www.responseCode, headers: www.GetResponseHeaders(), downloadHandler: www.downloadHandler);
                request.onDone?.Invoke(response);
            }
        }

        public IEnumerator DescribeInstance(DescribeEC2Instance request)
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

            var payloadHash = AWSS4Signer.SHA256Hash(string.Empty); ;
            var canonicalQueryString = string.Join("&", request.Query);

            var headers = new Dictionary<string, string>()
            {
                { "host", host },
                { "x-amz-date", amzdate }
            };

            var uri = "https://" + string.Join("/", new string[] { host, "?" + canonicalQueryString });

            var downloadHandler = new DownloadHandlerBuffer();

            UnityWebRequest www = new UnityWebRequest(uri)
            {
                method = method,
                downloadHandler = downloadHandler,
                certificateHandler = new ForceAcceptAll()
            };

            www.SetRequestHeader("x-amz-date", amzdate);
            www.SetRequestHeader("Authorization", AWSS4Signer.SignRequest(awsAccessKey, awsSecretKey, service, region, method, string.Empty, headers, canonicalQueryString, payloadHash, dateStamp, amzdate));

            yield return www.SendWebRequest();

            var responseCode = (int)www.responseCode;

            if (www.isNetworkError || www.isHttpError)
            {
                var exception = new EC2Exception(
                    responseCode: (int)www.responseCode,
                    description: www.error,
                    functionName: request.FunctionName,
                    headers: www.GetResponseHeaders()
                );

                var response = new DescribeEC2Response(responseCode: (int)www.responseCode, exception: exception);

                request.onDone?.Invoke(response);
            }
            else
            {
                var response = new DescribeEC2Response(success: true, responseCode: (int)www.responseCode, headers: www.GetResponseHeaders(), downloadHandler: www.downloadHandler);
                request.onDone?.Invoke(response);
            }
        }

        #endregion

    }

}
