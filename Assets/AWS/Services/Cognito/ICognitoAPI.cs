using System;
using AWS.Services.Cognito.Models;

namespace AWS.Services.Cognito
{
    public interface ICognitoAPI
    {
        public event Action<DeepLinkResponse> OnSignIn; 
        public void Setup(CognitoAuthService settings);
        public void SignInFederated();

        public void SignOut(Action onResponse);
    }
}