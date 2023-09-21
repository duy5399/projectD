using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoginUIManager : PlayfabManager
{
    void Awake()
    {
        setNameScreen.gameObject.SetActive(false);
    }

    public void ResetSignInPanel()
    {
        usernameSignIn.text = string.Empty;
        passwordSignIn.text = string.Empty;
        txtAlertSignIn.text = string.Empty;
    }

    public void ResetSignUpPanel()
    {
        usernameSignUp.text = string.Empty;
        passwordSignUp.text = string.Empty;
        confirmPasswordSignUp.text = string.Empty;
        emailSignUp.text= string.Empty;
        txtAlertSignUp.text = string.Empty;
    }

    public void ResetResetPasswordPanel()
    {
        emailResetPW.text = string.Empty;
        txtAlertResetPW.text = string.Empty;
    }

    public void ResetSetNamePanel()
    {
        nameInput.text = string.Empty;
    }

    //tạo tên ngẫu nhiên
    public void GetRandomName()
    {
        string characters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        string randomID = "";
        for (int i = 0; i < 6; i++)
        {
            randomID += characters[UnityEngine.Random.Range(0, characters.Length)];
        }
        nameInput.text = randomID;
    }

    public void BtnLoginClick()
    {
        SignIn();
    }

    public void BtnSignUpClick()
    {
        SignUp();
    }

    public void BtnResetPasswordClick()
    {
        ResetPassword();
    }

    public void BtnSetNameClick()
    {
        SetName();
    }
}
