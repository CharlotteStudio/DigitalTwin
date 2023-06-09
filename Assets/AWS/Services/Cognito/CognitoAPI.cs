using System;
using System.Collections;
using System.IO;
using System.Web;
using AWS.Services.Cognito.Models;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

namespace AWS.Services.Cognito
{
    public class CognitoAPI : MonoBehaviour, ICognitoAPI
    {
        private static readonly string TOKEN_STORAGE_FILE = "token.json";
        private CognitoAuthService settings;
        private string ServerURL;

        public event Action<DeepLinkResponse> OnSignIn;

        void ICognitoAPI.Setup(CognitoAuthService settings)
        {
            if (this.settings != null)
            {
                Debug.LogError($"{nameof(CognitoAPI)}::Already configured. Dispose first");
                return;
            }
            
            this.settings = settings;
            ServerURL = $"https://w.auth.{settings.region}.amazoncognito.com";

            Initialize();
        }
        
        void ICognitoAPI.SignInFederated() {
            if(!CheckSetup())
                return;
            var url = ServerURL + $"/oauth2/authorize?client_id={settings.clientId}&response_type={settings.responseType}&scope={HttpUtility.UrlEncode(settings.scope)}&redirect_uri={HttpUtility.UrlEncode(settings.loginRedirectURL)}";
            Application.OpenURL(url);
        }
    
        void ICognitoAPI.SignOut(Action onResponse)
        {
            if(!CheckSetup())
                return;
            StartCoroutine(SignOutRequest(onResponse));
        }
        
        
        #region Implementation

        private void OnEnable() => Application.deepLinkActivated += PerformDeepLinkAuthorization;
        
        private void OnDisable() => Application.deepLinkActivated -= PerformDeepLinkAuthorization;

        private void Initialize()
        {
            if (!string.IsNullOrEmpty(Application.absoluteURL))
            {
                PerformDeepLinkAuthorization();
                return;
            }
            
            var data = GetCachedDeepLinkData();
        
            if (data != null)
            {
                Debug.Log($"{nameof(CognitoExampleUsage)}::Cached Token is Valid");
                PerformLogin(data);
            }
        }

        private void PerformDeepLinkAuthorization(string url = null)
        {
            Debug.Log($"{nameof(CognitoExampleUsage)}::Perform DeepLink SignIn");
            var deeplinkData = ParseURL(Application.absoluteURL);
            CacheDeeplink(deeplinkData);

            PerformLogin(deeplinkData);
        }
        
        private void PerformLogin(DeepLinkResponse data)
        {
            OnSignIn?.Invoke(data);
        }
        
        private IEnumerator SignOutRequest(Action onResponse)
        {
            DisposeCachedDeeplink();
            
            var url = ServerURL + $"/logout?client_id={settings.clientId}&response_type={settings.responseType}&logout_uri={HttpUtility.UrlEncode(settings.logoutRedirectURL)}";
            var downloadHandler = new DownloadHandlerBuffer();
            var request = new UnityWebRequest(url, "GET");
            request.downloadHandler = downloadHandler;

            yield return request.SendWebRequest();

            Debug.Log($"{nameof(CognitoAPI)}::OnSignedOut {downloadHandler.text}");
            onResponse.Invoke();
        }
        
        
        #endregion

        #region Utils
        
        private static DeepLinkResponse ParseURL(string url)
        {
            var data = new DeepLinkResponse();
            var query = HttpUtility.ParseQueryString(url.Split('#')[1]);
            data.AccessToken = query["access_token"];
            data.IdToken = query["id_token"];
            data.TokenType = query["token_type"];
            data.ExpiresIn = int.Parse(query["expires_in"]);
            return data;
        }

        public static T DecodeJWT<T>(string data)
        {
            var parts = data.Split('.');
            if (parts.Length <= 2)
            {
                throw new Exception($"{nameof(CognitoAPI)}::Invalid JWT");
            }
            
            var decode = parts[1];
            var padLength = 4 - decode.Length % 4;
            if (padLength < 4)
            {
                decode += new string('=', padLength);
            }
            
            var bytes = Convert.FromBase64String(decode);
            var userInfo = System.Text.Encoding.ASCII.GetString(bytes);

            return JsonConvert.DeserializeObject<T>(userInfo);
            
        }
        
        public static DateTime UnixTimeStampToDateTime(double unixTimeStamp)
        {
            DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dateTime = dateTime.AddSeconds( unixTimeStamp ).ToLocalTime();
            return dateTime;
        }
        
        
        #endregion

        #region Auth Token Caching

        private static DeepLinkResponse GetCachedDeepLinkData()
        {
            if (!File.Exists(CachedTokenPath))
                return null;
            
            var data = File.ReadAllText(CachedTokenPath);
            
            if (string.IsNullOrEmpty(data))
            {
                return null;
            }
            
            var parsedData = JsonConvert.DeserializeObject<DeepLinkResponse>(data);

            if (parsedData == null)
            {
                Debug.LogError($"{nameof(CognitoAPI)}::Cached token exists, but unable to parse it");
                return null;
            }
            
            var token = DecodeJWT<AccessToken>(parsedData.AccessToken);
            
            var expirationDate = UnixTimeStampToDateTime(token.Expiration);
            if (expirationDate < DateTime.Now)
            {
                Debug.LogError($"{nameof(CognitoAPI)}::Cached access token is expired");
                return null;
            }
            
            return parsedData;
        }

        private static void CacheDeeplink(DeepLinkResponse token)
        {
            File.WriteAllText(CachedTokenPath, JsonConvert.SerializeObject(token));
        }

        private static void DisposeCachedDeeplink()
        {
            if(File.Exists(CachedTokenPath))
                File.Delete(CachedTokenPath);
        }

        private static string CachedTokenPath => Path.Combine(Application.persistentDataPath, TOKEN_STORAGE_FILE);
        
        
        #endregion
        
        
        private bool CheckSetup()
        {
            if (settings == null)
            {
                Debug.LogError($"{nameof(CognitoAPI)}::Unable to perform operation. Use 'ICognitoAPI.Setup' first");
                return false;
            }

            return true;
        }
    }
}