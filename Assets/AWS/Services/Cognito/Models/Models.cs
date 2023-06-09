using System;
using Newtonsoft.Json;

namespace AWS.Services.Cognito.Models
{
    [Serializable]
    public class CognitoAuthService
    {
        public string region = "eu-central-1";
        public string clientId = "ENTER YOUR CLIENT ID";
        public string responseType = "token";
        public string scope = "email openid phone";
        public string loginRedirectURL = "unitydl://mylink.com/";
        public string logoutRedirectURL = "https://localhost:3000/";
    }
    
    public class DeepLinkResponse
    {
        public string AccessToken;
        public string IdToken;
        public string TokenType;
        public int ExpiresIn;
    }

    public class UserProfile
    {
        [JsonProperty("sub")]public string UserId;
        [JsonProperty("email_verified")]public string EmailVerified;
        [JsonProperty("email")]public string Email;
        [JsonProperty("username")]public string UserName;
        [JsonProperty("auth_time")]public double AuthTime;
        [JsonProperty("exp")]public double Expiration;
    }
    
    public class AccessToken
    {
        [JsonProperty("sub")]public string UserId;
        [JsonProperty("username")]public string UserName;
        [JsonProperty("auth_time")]public double AuthTime;
        [JsonProperty("exp")]public double Expiration;
    }
    
    public class IdToken
    {
        [JsonProperty("sub")]public string UserId;
        [JsonProperty("email_verified")]public string EmailVerified;
        [JsonProperty("email")]public string Email;
        [JsonProperty("auth_time")]public double AuthTime;
        [JsonProperty("exp")]public double Expiration;
    }
}