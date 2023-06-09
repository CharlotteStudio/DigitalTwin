using AWS.Services.Cognito;
using AWS.Services.Cognito.Models;
using UnityEngine;

public class CognitoExampleUsage : MonoBehaviour
{
    [SerializeField] private CognitoAuthService settings;
    [SerializeField] private CognitoExampleUI uiManager;
    
    private ICognitoAPI cognitoAPI;
    
    private void Awake()
    {
        cognitoAPI = new GameObject("Cognito Client").AddComponent<CognitoAPI>();
        cognitoAPI.OnSignIn += OnSignedIn;
        cognitoAPI.Setup(settings);
        
        uiManager.Setup(PerformAuthorization, PerformLogout);
    }

    #region UI

    private void PerformAuthorization()
    {
        cognitoAPI.SignInFederated();
    }

    private void PerformLogout()
    {
        cognitoAPI.SignOut(OnSignedOut);
    }
    

    #endregion
    
    private void OnSignedIn(DeepLinkResponse data)
    {
        var accessToken = CognitoAPI.DecodeJWT<AccessToken>(data.AccessToken);
        var idToken = CognitoAPI.DecodeJWT<IdToken>(data.IdToken);

        var profile = new UserProfile();
        profile.UserId = accessToken.UserId;
        profile.UserName = accessToken.UserName;
        profile.Email = idToken.Email;
        profile.EmailVerified = idToken.EmailVerified;
        profile.AuthTime = accessToken.AuthTime;
        profile.Expiration = accessToken.Expiration;
        
        uiManager.OnUserProfileReceived(profile);
        
    }

    private void OnSignedOut()
    {
        uiManager.OnUserSignedOut();
    }
}
