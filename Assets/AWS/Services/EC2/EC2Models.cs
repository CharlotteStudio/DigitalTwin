using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.Networking;
using SquareBeam.AWS.EC2.Models.ParsedResponses;

namespace SquareBeam.AWS.EC2.Models
{

    #region Requests

    public class EC2Request
    {
        public readonly string FunctionName;
        public readonly string Payload;
        public string ActionName;
        public readonly string Version = "2016-11-15";
        public Action<EC2Response> onDone;

        internal string awsAccessKey;
        internal string awsSecretKey;
        internal string region;

        public EC2Request()
        {
        }

        public EC2Request(Action<EC2Response> onDone)
        {
            this.onDone = onDone;
        }

        public EC2Request SignRequestWithOwnCredentials(string awsAccessKey, string awsSecretKey, string awsRegion)
        {
            this.awsAccessKey = awsAccessKey;
            this.awsSecretKey = awsSecretKey;
            this.region = awsRegion;

            return this;
        }
    }

    public class RunEC2Instance : EC2Request
    {
        public new string ActionName = "RunInstances";
        public string ImageId;
        internal readonly List<string> Query;
        public RunEC2Instance(string ImageId, uint MinCount, uint MaxCount, string InstanceType, string KeyName = null, string SecurityGroup = null, string SubnetId = null)
        {
            this.ImageId = ImageId;

            Query = new List<string>() { $"Action={ActionName}" };

            Query.Add("ImageId=" + ImageId);
            Query.Add("InstanceType=" + InstanceType);

            Query.Add("MinCount=" + MinCount.ToString());
            Query.Add("MaxCount=" + MaxCount.ToString());


            //Query.Add("KeyName=" + KeyName);
            //Query.Add("SecurityGroup=" + SecurityGroup);
            //Query.Add("SubnetId=" + SubnetId);


            Query.Add("Version=" + Version);

            Query = Query.OrderBy(header => header).ToList();
           
        }

    }

    public class TerminateEC2Instance : EC2Request
    {
        public new string ActionName = "TerminateInstances";
        public string[] InstanceIds;
        internal readonly List<string> Query;
        public TerminateEC2Instance(string[] InstanceIds)
        {
            this.InstanceIds = InstanceIds;

            Query = new List<string>() { $"Action={ActionName}" };
            Query.Add("InstanceId=" + InstanceIds[0]);
            Query.Add("Version=" + Version);

            Query = Query.OrderBy(header => header).ToList();
        }

    }

    public class StartEC2Instance : EC2Request
    {
        public new string ActionName = "StartInstances";
        public string[] InstanceIds;
        internal readonly List<string> Query;
        public StartEC2Instance(string[] InstanceIds)
        {
            this.InstanceIds = InstanceIds;

            Query = new List<string>() { $"Action={ActionName}" };
            Query.Add("InstanceId=" + InstanceIds[0]);
            Query.Add("Version=" + Version);
        }

    }

    public class StopEC2Instance : EC2Request
    {
        public new string ActionName = "StopInstances";
        public string[] InstanceIds;
        internal readonly List<string> Query;
        public StopEC2Instance(string[] InstanceIds)
        {
            this.InstanceIds = InstanceIds;

            Query = new List<string>() { $"Action={ActionName}" };
            Query.Add("InstanceId=" + InstanceIds[0]);
            Query.Add("Version=" + Version);
        }

    }

    public class DescribeEC2Instance : EC2Request
    {
        public new string ActionName = "DescribeInstances";
        public string[] InstanceIds;
        internal readonly List<string> Query;
        public DescribeEC2Instance(string[] InstanceIds)
        {
            this.InstanceIds = InstanceIds;

            Query = new List<string>() { $"Action={ActionName}" };
            Query.Add("InstanceId=" + InstanceIds[0]);
            Query.Add("Version=" + Version);
        }

    }


    #endregion


    #region Responses

    public class EC2Response
    {
        public readonly bool Success;
        public readonly Exception Exception;

        public readonly int ResponseCode;
        public readonly Dictionary<string, string> Headers;
        public readonly DownloadHandler DownloadHandler;


        public EC2Response(bool success = false, int responseCode = -1, Exception exception = null, Dictionary<string,string> headers = null, DownloadHandler downloadHandler = null)
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


    public class RunEC2Response : EC2Response
    {
        public readonly RunEC2InstanceResponse Result;

        public RunEC2Response(bool success = false, int responseCode = -1, Exception exception = null, Dictionary<string, string> headers = null, DownloadHandler downloadHandler = null) : base(success, responseCode, exception, headers)
        {
            if (downloadHandler != null)
            {
                XmlSerializer serializer = new XmlSerializer(typeof(RunEC2InstanceResponse));
                using (StringReader reader = new StringReader(downloadHandler.text))
                {
                    Result = (RunEC2InstanceResponse)(serializer.Deserialize(reader));
                }
            }
        }
    }

    public class TerminateEC2Response : EC2Response
    {
        public readonly TerminateEC2InstanceResponse Result;

        public TerminateEC2Response(bool success = false, int responseCode = -1, Exception exception = null, Dictionary<string, string> headers = null, DownloadHandler downloadHandler = null) : base(success, responseCode, exception, headers)
        {
            if (downloadHandler != null)
            {
                XmlSerializer serializer = new XmlSerializer(typeof(TerminateEC2InstanceResponse));
                using (StringReader reader = new StringReader(downloadHandler.text))
                {
                    Result = (TerminateEC2InstanceResponse)(serializer.Deserialize(reader));
                }
            }
        }
    }

    public class StartEC2Response : EC2Response
    {
        public readonly StartEC2InstanceResponse Result;

        public StartEC2Response(bool success = false, int responseCode = -1, Exception exception = null, Dictionary<string, string> headers = null, DownloadHandler downloadHandler = null) : base(success, responseCode, exception, headers)
        {
            if (downloadHandler != null)
            {
                XmlSerializer serializer = new XmlSerializer(typeof(StartEC2InstanceResponse));
                using (StringReader reader = new StringReader(downloadHandler.text))
                {
                    Result = (StartEC2InstanceResponse)(serializer.Deserialize(reader));
                }
            }
        }
    }

    public class StopEC2Response : EC2Response
    {
        public readonly StopEC2InstanceResponse Result;

        public StopEC2Response(bool success = false, int responseCode = -1, Exception exception = null, Dictionary<string, string> headers = null, DownloadHandler downloadHandler = null) : base(success, responseCode, exception, headers)
        {
            if (downloadHandler != null)
            {
                XmlSerializer serializer = new XmlSerializer(typeof(StopEC2InstanceResponse));
                using (StringReader reader = new StringReader(downloadHandler.text))
                {
                    Result = (StopEC2InstanceResponse)(serializer.Deserialize(reader));
                }
            }
        }
    }



    public class DescribeEC2Response : EC2Response
    {
        public readonly DescribeEC2InstanceResponse Result;

        public DescribeEC2Response(bool success = false, int responseCode = -1, Exception exception = null, Dictionary<string, string> headers = null, DownloadHandler downloadHandler = null) : base(success, responseCode, exception, headers)
        {
            if (downloadHandler != null)
            {
                XmlSerializer serializer = new XmlSerializer(typeof(DescribeEC2InstanceResponse));
                using (StringReader reader = new StringReader(downloadHandler.text))
                {
                    Result = (DescribeEC2InstanceResponse)(serializer.Deserialize(reader));
                }
            }
        }
    }



    public class EC2Exception : Exception
    {
        public readonly int ResponseCode;
        public readonly string Description;
        public readonly string FunctionName;
        public readonly Dictionary<string, string> Headers;

        public EC2Exception(int responseCode = -1, string description = null, string functionName = null, Dictionary<string, string> headers = null)
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


#endregion