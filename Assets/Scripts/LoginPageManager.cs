using System;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoginPageManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TMP_InputField _nameInput;
    [SerializeField] private TMP_InputField _passwordInput;
    [SerializeField] private Button _loginButton;

    [Header("Component")]
    [SerializeField] private Dialog _dialog;
    
    private void Start()
    {
        _loginButton.onClick.AddListener(CheckLogin);
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
        if (!userName.Equals(MyConstant.UserName, StringComparison.OrdinalIgnoreCase) || !pw.Equals(MyConstant.Password))
        {
            _dialog.gameObject.SetActive(true);
            _dialog.SetActiveDialog("Username or Password are incurrect !");
            Debug.Log($"username : [{userName}], pw : [{pw}]");
            return;
        }
        
        SceneManager.LoadScene(MyConstant.MainScene);
    }
}
