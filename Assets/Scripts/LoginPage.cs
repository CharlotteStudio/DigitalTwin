using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoginPage : MonoBehaviour
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
            _dialog.SetActiveDialog("");
            return;
        }
        
        if (pw.Equals(""))
        {
            _dialog.SetActiveDialog("");
            return;
        }

        TryLoginAWS();
    }

    private void TryLoginAWS()
    {
        if (!name.Equals(MyConstant.UserName) || !name.Equals(MyConstant.Password))
        {
            _dialog.SetActiveDialog("");
            return;
        }
        
        SceneManager.LoadScene(MyConstant.MainScene);
    }
}
