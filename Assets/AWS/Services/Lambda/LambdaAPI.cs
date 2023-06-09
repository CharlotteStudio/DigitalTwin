using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using SquareBeam.AWS.Lambda.Models;
using UnityEngine.Networking;
using UnityEngine;

namespace SquareBeam.AWS.Lambda
{
    public class LambdaAPI : MonoBehaviour
    {
        internal readonly string service = "lambda";

        internal string awsAccessKey;
        internal string awsSecretKey;

        internal string region;

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

        public IEnumerator LambdaInvoke(LambdaRequest request)
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

            var host = $"lambda.{region}.amazonaws.com";

            var endpoint = $"2015-03-31/functions/{request.FunctionName}/invocations";

            var timestamp = DateTime.UtcNow;
            var amzdate = timestamp.ToString("yyyyMMddTHHmmssZ");
            var dateStamp = timestamp.ToString("yyyyMMdd");

            var payloadHash = AWSS4Signer.SHA256Hash(request.Payload);
            var canonicalQueryString = string.Empty;

            var headers = new Dictionary<string, string>()
            {
                { "host", host },
                { "x-amz-content-sha256", payloadHash },
                { "x-amz-date", amzdate }
            };

            byte[] jsonToSend = new UTF8Encoding().GetBytes(request.Payload);

            var www = new UnityWebRequest(string.Join("/", new string[] { "https:/", host, endpoint }), method);
            www.uploadHandler = new UploadHandlerRaw(jsonToSend);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.certificateHandler = new ForceAcceptAll();

            www.SetRequestHeader("x-amz-content-sha256", payloadHash);
            www.SetRequestHeader("x-amz-date", amzdate);
            www.SetRequestHeader("Authorization", AWSS4Signer.SignRequest(awsAccessKey, awsSecretKey, service, region, method, endpoint, headers, canonicalQueryString, payloadHash, dateStamp, amzdate));

            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                var exception = new LambdaException(
                    responseCode: (int)www.responseCode,
                    description: www.error,
                    functionName: request.FunctionName,
                    headers: www.GetResponseHeaders()
                );

                var response = new LambdaResponse(responseCode: (int)www.responseCode, exception: exception);

                request.onDone?.Invoke(response);
            }
            else
            {
                var response = new LambdaResponse(success: true, responseCode: (int)www.responseCode, headers: www.GetResponseHeaders(), downloadHandler: www.downloadHandler);
                request.onDone?.Invoke(response);
            }

        }

    }

}
