using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using UnityEngine.Networking;


namespace SquareBeam.AWS.S3
{
    #region Base

    public class S3Request
    {
        public readonly string Bucket;
        public readonly string Key;
        public Action<S3Response> onDone;

        internal string awsAccessKey;
        internal string awsSecretKey;
        internal string region;

        public S3Request(string Bucket, string Key)
        {
            this.Bucket = Bucket;
            this.Key = Key;
        }

        public S3Request SignRequestWithOwnCredentials(string awsAccessKey, string awsSecretKey, string awsRegion)
        {
            this.awsAccessKey = awsAccessKey;
            this.awsSecretKey = awsSecretKey;
            this.region = awsRegion;

            return this;
        }
    }

    public class S3Response
    {
        public readonly bool Success;
        public readonly int ResponseCode;
        public readonly Exception Exception;
        public readonly Dictionary<string, string> Headers;

        public S3Response(bool success = false, int responseCode = 0, Exception exception = null, Dictionary<string, string> headers = null)
        {
            if (exception != null)
                success = false;

            Success = success;
            ResponseCode = responseCode;
            Exception = exception;
            Headers = headers;
        }

        public override string ToString()
        {
            var result = new StringBuilder()
                .AppendLine($"Success: {Success}")
                .AppendLine($"ResponseCode: {ResponseCode}")
                .AppendLine($"Headers: {string.Join("\n", Headers.Select(kv => kv.Key + " : " + kv.Value).ToArray()) }");
            return result.ToString();
        }
    }


    #endregion


    #region HEAD
    public class HeadS3Request : S3Request
    {
        public readonly Dictionary<string, string> Response = new Dictionary<string, string>();
       
        public HeadS3Request(string Bucket, string Key) : base(Bucket, Key)
        {
        }
    }

    public class HeadS3Response : S3Response
    {
        public HeadS3Response(bool success = false, int responseCode = 0, Exception exception = null, Dictionary<string, string> headers = null) : base(success, responseCode, exception, headers)
        {
        }
    }


    #endregion


    #region DOWNLOAD

    public class DownloadS3Request : S3Request
    {
        public Action<OpertationProgress> onProgress;
        public readonly DownloadHandler downloadHandler;

        public DownloadS3Request(string Bucket, string Key, DownloadHandler DownladHandler) : base(Bucket, Key)
        {
            downloadHandler = DownladHandler;
        }
    }

    public class DownloadS3Response : S3Response
    {
        public DownloadS3Response(bool success = false, int responseCode = 0, Exception exception = null, Dictionary<string, string> headers = null) : base(success, responseCode, exception, headers)
        {
        }
    }


    #endregion


    #region UPLOAD

    public class UploadS3Request : S3Request
    {
        public Action<OpertationProgress> onProgress;
        public readonly UploadHandler uploadHandler;

        public UploadS3Request(string Bucket, string Key, UploadHandler UploadHandler) : base(Bucket, Key)
        {
            uploadHandler = UploadHandler;
        }
    }

    public class UploadS3Response : S3Response
    {
        public UploadS3Response(bool success = false, int responseCode = 0, Exception exception = null, Dictionary<string, string> headers = null) : base(success, responseCode, exception, headers)
        {
        }
    }

    #endregion


    #region DELETE

    public class DeleteObjectS3Request : S3Request
    {
        public DeleteObjectS3Request(string Bucket, string Key) : base(Bucket, Key)
        {
        }
    }

    public class DeleteObjectS3Response : S3Response
    {
        public DeleteObjectS3Response(bool success = false, int responseCode = 0, Exception exception = null, Dictionary<string, string> headers = null) : base(success, responseCode, exception, headers)
        {
        }
    }


    #endregion


    #region LIST

    public class ListBucketS3Request : S3Request
    {
        internal readonly List<string> Query;

        public ListBucketS3Request(string Bucket, string Prefix = null, uint MaxKeys = 0, string Delimiter = null, string ContinuationToken = null) : base(Bucket, string.Empty)
        {
            Query = new List<string>() { "list-type=2" };

            if (!string.IsNullOrEmpty(Prefix))
                Query.Add("prefix=" + WebUtility.UrlEncode(Prefix));
            if (MaxKeys > 0)
                Query.Add("max-keys=" + MaxKeys.ToString());
            if (!string.IsNullOrEmpty(Delimiter))
                Query.Add("delimiter=" + Delimiter);
            if (!string.IsNullOrEmpty(ContinuationToken))
                Query.Add("continuation-token=" + ContinuationToken);

            Query.Sort();
        }
    }

    public class ListBucketS3Response : S3Response
    {
        public readonly ListBucketResult Result;
        public ListBucketS3Response(bool success = false, int responseCode = 0, Exception exception = null, Dictionary<string, string> headers = null, DownloadHandler downloadHandler = null) : base(success, responseCode, exception, headers)
        {
            if(downloadHandler != null)
            {
                XmlSerializer serializer = new XmlSerializer(typeof(ListBucketResult));
                using (StringReader reader = new StringReader(downloadHandler.text))
                {
                    Result = (ListBucketResult)(serializer.Deserialize(reader));
                }
            }
        }
    }

    [XmlRoot(ElementName = "ListBucketResult", Namespace = "http://s3.amazonaws.com/doc/2006-03-01/")]
    public class ListBucketResult
    {
        [XmlElement(ElementName = "Name")]
        public string Name { get; set; }
        [XmlElement(ElementName = "Prefix")]
        public string Prefix { get; set; }
        [XmlElement(ElementName = "NextContinuationToken")]
        public string NextContinuationToken { get; set; }
        [XmlElement(ElementName = "KeyCount")]
        public int KeyCount { get; set; }
        [XmlElement(ElementName = "MaxKeys")]
        public int MaxKeys { get; set; }
        [XmlElement(ElementName = "Delimiter")]
        public string Delimiter { get; set; }
        [XmlElement(ElementName = "IsTruncated")]
        public bool IsTruncated { get; set; }
        [XmlElement(ElementName = "Contents")]
        public List<Contents> Contents { get; set; }
    }

    [XmlRoot(ElementName = "Contents")]
    public class Contents
    {
        [XmlElement(ElementName = "Key")]
        public string Key { get; set; }
        [XmlElement(ElementName = "LastModified")]
        public DateTime LastModified { get; set; }
        [XmlElement(ElementName = "ETag")]
        public string ETag { get; set; }
        [XmlElement(ElementName = "Size")]
        public ulong Size { get; set; }
        [XmlElement(ElementName = "StorageClass")]
        public string StorageClass { get; set; }
    }

    #endregion


    #region LIST BUCKETS

    public class ListBucketsS3Request : S3Request
    {
        internal readonly List<string> Query;

        public ListBucketsS3Request() : base(string.Empty, string.Empty)
        {

            Query = new List<string>() { "Action=ListBuckets" };
        }
    }

    public class ListBucketsS3Response : S3Response
    {
        public readonly ListBucketsResult Result;
        public ListBucketsS3Response(bool success = false, int responseCode = 0, Exception exception = null, Dictionary<string, string> headers = null, DownloadHandler downloadHandler = null) : base(success, responseCode, exception, headers)
        {
            if (downloadHandler != null)
            {
                XmlSerializer serializer = new XmlSerializer(typeof(ListBucketsResult));
                using (StringReader reader = new StringReader(downloadHandler.text))
                {
                    Result = (ListBucketsResult)(serializer.Deserialize(reader));
                }
            }
        }
    }

    [XmlRoot(ElementName = "ListAllMyBucketsResult", Namespace = "http://s3.amazonaws.com/doc/2006-03-01/")]
    public class ListBucketsResult
    {
        [XmlArray(ElementName = "Buckets"), XmlArrayItem("Bucket")]
        public List<Bucket> Buckets { get; set; }
    }
    
    public class Bucket
    {
        [XmlElement(ElementName = "Name")]
        public string Name { get; set; }
        [XmlElement(ElementName = "CreationDate")]
        public DateTime CreationDate { get; set; }
    }

    #endregion


    #region CREATE BUCKET

    public class CreateBucketS3Request : S3Request
    {
        public readonly string BucketConfigXML;

        public CreateBucketS3Request(string Bucket, string Region = null) : base(Bucket, string.Empty)
        {
            if(!string.IsNullOrEmpty(Region))
            {
                region = Region;

                XNamespace docNamespace = "http://s3.amazonaws.com/doc/2006-03-01/";
                XDocument doc = new XDocument(
                    new XElement(docNamespace + "CreateBucketConfiguration",
                    new XElement("LocationConstraint", region))
                  );

                BucketConfigXML = doc.ToString();
            }
        }
    }

    public class CreateBucketS3Response : S3Response
    {
        public CreateBucketS3Response(bool success = false, int responseCode = 0, Exception exception = null, Dictionary<string, string> headers = null, DownloadHandler downloadHandler = null) : base(success, responseCode, exception, headers)
        {

        }
    }


    #endregion


    #region DELETE BUCKET

    public class DeleteBucketS3Request : S3Request
    {
        public DeleteBucketS3Request(string Bucket, string Region = null) : base(Bucket, string.Empty)
        {
            if (!string.IsNullOrEmpty(Region))
            {
                region = Region;
            }
        }
    }

    public class DeleteBucketS3Response : S3Response
    {
        public DeleteBucketS3Response(bool success = false, int responseCode = 0, Exception exception = null, Dictionary<string, string> headers = null, DownloadHandler downloadHandler = null) : base(success, responseCode, exception, headers)
        {

        }
    }


    #endregion


    #region EXCEPTION

    public class S3Exception : Exception
    {
        public readonly string Description;
        public readonly string Bucket;
        public readonly string Key;
        public readonly Dictionary<string, string> Headers;

        public S3Exception(string description = null, string bucket = null, string key = null, Dictionary<string, string> headers = null)
        {
            Description = description;
            Bucket = bucket;
            Key = key;
            Headers = headers;
        }

        public override string ToString()
        {
            var result = new StringBuilder()
                .AppendLine($"Description: {Description}")
                .AppendLine($"Bucket: {Bucket}")
                .AppendLine($"Key: {Key}")
                .AppendLine($"Headers: {string.Join("\n", Headers.Select(kv => kv.Key + " : " + kv.Value).ToArray()) }");
            return result.ToString();
        }
    }


    #endregion


    public struct OpertationProgress
    {
        public readonly float Progress;
        public readonly ulong TransferedBytes;

        public OpertationProgress(float progress, ulong transferedBytes)
        {
            Progress = progress;
            TransferedBytes = transferedBytes;
        }
    }
}