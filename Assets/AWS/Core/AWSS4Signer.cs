using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace SquareBeam.AWS
{
    public static class AWSS4Signer
    {

        #region Signing

        public static string SignRequest(string awsAccessKey, string awsSecretKey, string service, string region, string method, string endpoint, Dictionary<string, string> headers, string canonicalQueryString, string payloadHash, string dateStamp, string amzdate)
        {
            //Add trailing \n is required
            var canonicalHeaders = string.Join("\n", headers.Select(x => x.Key + ":" + x.Value)) + "\n";
            var signedHeaders = string.Join(";", headers.Keys);

            var canonicalUri = "/" + endpoint;
            var canonicalRequest = string.Join("\n", new string[] { method, canonicalUri, canonicalQueryString, canonicalHeaders, signedHeaders, payloadHash });

            var credentialScope = string.Join("/", new string[] { dateStamp, region, service, "aws4_request" });

            var algorithm = "AWS4-HMAC-SHA256";

            var stringToSign = string.Join("\n", new string[] { algorithm, amzdate, credentialScope, SHA256Hash(canonicalRequest) });
            var signingKey = GetSignatureKey(awsSecretKey, dateStamp, region, service);

            var signature = BytesToHexString(new HMACSHA256(signingKey).ComputeHash(new UTF8Encoding().GetBytes(stringToSign)));

            return algorithm + " " + $"Credential={awsAccessKey}" + "/" + credentialScope + ", " + "SignedHeaders=" + signedHeaders + ", " + "Signature=" + signature;
        }


        private static byte[] Sign(byte[] key, string msg)
        {
            HMACSHA256 hash = new HMACSHA256(key);

            byte[] messageBytes = new UTF8Encoding().GetBytes(msg);
            byte[] hashedBytes = hash.ComputeHash(messageBytes);

            return hashedBytes;
        }

        private static byte[] GetSignatureKey(string secretKey, string dateStamp, string regionName, string serviceName)
        {
            var kDate = Sign(new UTF8Encoding().GetBytes("AWS4" + secretKey), dateStamp);
            var kRegion = Sign(kDate, regionName);
            var kService = Sign(kRegion, serviceName);
            var kSigning = Sign(kService, "aws4_request");
            return kSigning;
        }


        #endregion

        #region Hashing

        public static string SHA256Hash(string value)
        {
            return SHA256Hash(new UTF8Encoding().GetBytes(value));
        }

        public static string SHA256Hash(byte[] value)
        {
            var hash = SHA256.Create().ComputeHash(value);

            // Convert byte array to a string   
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                builder.Append(hash[i].ToString("x2"));
            }
            return builder.ToString();
        }

        private static string BytesToHexString(byte[] array)
        {
            StringBuilder hex = new StringBuilder(array.Length * 2);
            foreach (byte b in array)
            {
                hex.AppendFormat("{0:x2}", b);
            }
            return hex.ToString();
        }


        #endregion
    }
}