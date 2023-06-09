using System;
using AWS.Services.Cognito.Models;
using UnityEngine;
using UnityEngine.UI;

namespace AWS.Services.Cognito
{
    public class CognitoExampleUI : MonoBehaviour
    {
        public Button LoginButton;
        public Button LogoutButton;

        public Text UserProfileInfo;
        public Text Logs;
        
        private Action onLoginButton;
        private Action onLogoutButton;

        public void Setup(Action onLoginButton, Action onLogoutButton)
        {
            this.onLoginButton = onLoginButton;
            this.onLogoutButton = onLogoutButton;

            LoginButton.onClick.AddListener(OnLoginButton);
            LogoutButton.onClick.AddListener(OnLogoutButton);
        }

        private void OnLoginButton()
        {
            onLoginButton.Invoke();

            SetButtonsInteractable(false);
        }

        private void OnLogoutButton()
        {
            onLogoutButton.Invoke();

            SetButtonsInteractable(false);
        }

        public void OnUserProfileReceived(UserProfile profile)
        {
            UserProfileInfo.text = "UserProfileData:\n" +
                                   $"UserId: {profile.UserId}\n" +
                                   $"UserName: {profile.UserName}\n" +
                                   $"Email: {profile.Email}\n" +
                                   $"EmailVerified: {profile.EmailVerified}\n" +
                                   $"AuthTime: {CognitoAPI.UnixTimeStampToDateTime(profile.AuthTime)}\n" +
                                   $"ExpirationTime: {CognitoAPI.UnixTimeStampToDateTime(profile.Expiration)}\n";

            SetButtonsInteractable(true);
        }

        public void OnUserSignedOut()
        {
            UserProfileInfo.text = "Signed Out";
            
            SetButtonsInteractable(true);
        }

        private void SetButtonsInteractable(bool value)
        {
            LoginButton.interactable = value;
            LogoutButton.interactable = value;
        }

        private void OnEnable()
        {
            Application.logMessageReceived += OnLogReceived;
        }

        private void OnDisable()
        {
            Application.logMessageReceived -= OnLogReceived;
        }

        private void OnLogReceived(string condition, string trace, LogType type)
        {
            Logs.text = condition;
        }
    }
}