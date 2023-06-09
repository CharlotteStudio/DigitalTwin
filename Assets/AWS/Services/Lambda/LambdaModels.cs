using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace SquareBeam.AWS.Lambda.Models
{
    public class LambdaRequest
    {
        public readonly string FunctionName;
        public readonly string Payload;
        public Action<LambdaResponse> onDone;

        internal string awsAccessKey;
        internal string awsSecretKey;
        internal string region;

        public LambdaRequest(string functionName, string payload)
        {
            FunctionName = functionName;
            Payload = payload;
        }

        public LambdaRequest(string functionName, string payload, Action<LambdaResponse> onDone)
        {
            FunctionName = functionName;
            Payload = payload;
            this.onDone = onDone;
        }

        public LambdaRequest SignRequestWithOwnCredentials(string awsAccessKey, string awsSecretKey, string awsRegion)
        {
            this.awsAccessKey = awsAccessKey;
            this.awsSecretKey = awsSecretKey;
            this.region = awsRegion;

            return this;
        }
    }
    
    public class LambdaResponse
    {
        public readonly bool Success;
        public readonly Exception Exception;

        public readonly int ResponseCode;
        public readonly Dictionary<string, string> Headers;
        public readonly DownloadHandler DownloadHandler;


        public LambdaResponse(bool success = false, int responseCode = -1, Exception exception = null, Dictionary<string,string> headers = null, DownloadHandler downloadHandler = null)
        {
            if (exception != null)
                success = false;

            Success = success;
            ResponseCode = responseCode;
            Exception = exception;
            Headers = headers;
            DownloadHandler = downloadHandler;
        }
    }

    public class LambdaException : Exception
    {
        public readonly int ResponseCode;
        public readonly string Description;
        public readonly string FunctionName;
        public readonly Dictionary<string, string> Headers;

        public LambdaException(int responseCode = -1, string description = null, string functionName = null, Dictionary<string, string> headers = null)
        {
            ResponseCode = responseCode;
            Description = description;
            FunctionName = functionName;
            Headers = headers;
        }

        public override string ToString()
        {
            var result = new StringBuilder()
                .AppendLine($"Response Code: { ResponseCode}")
                .AppendLine($"Description: {Description}")
                .AppendLine($"Function Name: {FunctionName}")
                .AppendLine($"Headers: {JsonUtility.ToJson(Headers, true)}");
            return result.ToString();
        }
    }

}