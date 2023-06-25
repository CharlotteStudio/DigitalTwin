using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using SquareBeam.AWS.Lambda.Models;

public class LoginPageManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TMP_InputField _nameInput;
    [SerializeField] private TMP_InputField _passwordInput;
    [SerializeField] private Button _loginButton;
    [SerializeField] private Toggle _toggle;
    [SerializeField] private Button _exitButton;

    [Header("Component")]
    [SerializeField] private Dialog _dialog;
    
    private void Start()
    {
        _loginButton.gameObject.SetActive(true);
        _loginButton.onClick.AddListener(CheckLogin);
        _exitButton.onClick.AddListener(()=> Application.Quit());
        ReadUser();
    }

    private void CheckLogin()
    {
        var name = _nameInput.text;
        var pw = _passwordInput.text;

        if (name.Equals(""))
        {
            _dialog.gameObject.SetActive(true);
            _dialog.SetActiveDialog("Username is empty !");
            return;
        }
        
        if (pw.Equals(""))
        {
            _dialog.gameObject.SetActive(true);
            _dialog.SetActiveDialog("Password is empty !");
            return;
        }

        TryLoginAWS(name, pw);
    }

    private void TryLoginAWS(string userName, string pw)
    {
        $"UserName : [{userName}], Password : [{pw}]".DebugLog();
        
        string payload = "{\"UserName\":\"";
        payload += userName;
        payload += "\",\"Password\":\"";
        payload += pw.ToMD5();
        payload += "\"}";

        _loginButton.gameObject.SetActive(false);
        AWSManager.Instance.InvokeLambdaFunction(MyConstant.AWSService.LambdaFunction.CheckUser, payload, OnReceivedEvent);
        
        void OnReceivedEvent(LambdaResponse response)
        {
            if (response.Success)
            {
                $"Success: Response is :\n{response.DownloadHandler.text.ToPrettyPrintJsonString()}".DebugLog();
                if (response.DownloadHandler.text.Equals("") || response.DownloadHandler.text.Equals("null"))
                {
                    "Can not get anything".DebugLogWarning();
                    return;
                }
                response.DownloadHandler.text.DebugLog();
                if (response.DownloadHandler.text.Equals("\"pass\""))
                {
                    if (_toggle.isOn)
                        SaveUser(userName, pw);
                    else
                        DeleteUser();
                    
                    UserProfile.Instance.SetUserName(userName);
                    SceneManager.LoadScene(MyConstant.MainScene);
                }
                else
                {
                    _dialog.gameObject.SetActive(true);
                    _dialog.SetActiveDialog("Username or Password are incurrect !");
                    _loginButton.gameObject.SetActive(true);
                }
            }
            else
                AWSManager.Instance.ResponseFail(response);
        }
    }

    private void SaveUser(string userName, string pw)
    {
        SaveManager.Instance.SaveStringData(MyConstant.SaveKey.UserName, userName);
        SaveManager.Instance.SaveStringData(MyConstant.SaveKey.Password, pw);
    }

    private void ReadUser()
    {
        string userName = SaveManager.Instance.ReadStringData(MyConstant.SaveKey.UserName);
        string pw = SaveManager.Instance.ReadStringData(MyConstant.SaveKey.Password);
        
        if (!userName.Equals(""))
            _nameInput.text = userName;
        
        if (!pw.Equals(""))
            _passwordInput.text = pw;
    }

    private void DeleteUser()
    {
        SaveManager.Instance.DeleteSave(MyConstant.SaveKey.UserName);
        SaveManager.Instance.DeleteSave(MyConstant.SaveKey.Password);
    }
}
