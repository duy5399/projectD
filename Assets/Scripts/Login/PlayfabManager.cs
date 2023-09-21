using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PlayFab;
using PlayFab.ClientModels;
using System;
using TMPro;
using System.Net.Mail;
using TMPro.Examples;
using System.Threading.Tasks;
using UnityEngine.EventSystems;

public class PlayfabManager : MonoBehaviour
{
    [Header("Sign In")]
    [SerializeField] protected TMP_InputField usernameSignIn, passwordSignIn;
    [SerializeField] protected TextMeshProUGUI txtAlertSignIn;

    [Header("Sign Up")]
    [SerializeField] protected TMP_InputField usernameSignUp, passwordSignUp, confirmPasswordSignUp, emailSignUp;
    [SerializeField] protected TextMeshProUGUI txtAlertSignUp;

    [Header("Reset Password")]
    [SerializeField] protected TMP_InputField emailResetPW;
    [SerializeField] protected TextMeshProUGUI txtAlertResetPW;

    [Header("Loading Screen")]
    [SerializeField] protected Transform loadingScreen;

    [Header("Set Name Screen")]
    [SerializeField] protected Transform setNameScreen;
    [SerializeField] protected TMP_InputField nameInput;
    [SerializeField] protected TextMeshProUGUI txtAlertSetName;

    string Encrypt(string password)
    {
        if (string.IsNullOrEmpty(password))
        {
            return null;
        }
        System.Security.Cryptography.MD5CryptoServiceProvider x = new System.Security.Cryptography.MD5CryptoServiceProvider();
        byte[] bs = System.Text.Encoding.UTF8.GetBytes(password);
        bs = x.ComputeHash(bs);
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        foreach (byte b in bs)
        {
            sb.Append(b.ToString("x2").ToLower());
        }
        return sb.ToString();
    }

    #region Đăng nhập
    //Đăng nhập------------------------------------------------------------------------------------------------------------------------------------------------------------------
    protected void SignIn()
    {
        if(!IsValidSignIn())
        {
            return;
        }
        loadingScreen.gameObject.SetActive(true);
        var loginRequest = new LoginWithPlayFabRequest { Username = usernameSignIn.text, Password = passwordSignIn.text };
        PlayFabClientAPI.LoginWithPlayFab(loginRequest, LoginSuccess, LoginError);
    }

    private async void LoginSuccess(LoginResult result)
    {
        ExecuteCloudScriptRequest request = new ExecuteCloudScriptRequest
        {
            FunctionName = "UpdateLastLogin"
        };
        PlayFabClientAPI.ExecuteCloudScript(request,
            result =>
            {
                Debug.Log("UpdateLastLogin thành công: " + result.FunctionResult);
            },
            error =>
            {
                Debug.Log("UpdateLastLogin thất bại" + error.Error);
            });
        //PlayfabDataManager.GetUserData("username");
        PlayfabDataManager.instance.GetPlayFabID(result.PlayFabId);
        PlayfabDataManager.instance.GetCatalogItems("MainCatalog");
        PlayfabDataManager.instance.GetStoreItems("MainCatalog", "store_01");
        PlayfabDataManager.instance.GetDiscountStoreData("MainCatalog");
        await Task.Delay(1000);
        //PlayfabDataManager.instance.GetDiscontStoreItems("MainCatalog", "EquipmentDiscountStore-Uncommon");
        //PlayfabDataManager.instance.GetDiscontStoreItems("MainCatalog", "EquipmentDiscountStore-Rare");
        txtAlertSignIn.text = "Đăng nhập thành công!";
        txtAlertSignIn.color = new Color32(0, 255, 0, 255);
        CheckHasUsername();

        //AsyncLoadingScene.instance.LoadNewScene(1);
    }

    private void LoginError(PlayFabError error)
    {
        loadingScreen.gameObject.SetActive(false);
        txtAlertSignIn.text = "Tài khoản hoặc mật khẩu không đúng! Hãy kiểm tra lại";
        txtAlertSignIn.color = new Color32(255, 255, 0, 255);
    }

    private void CheckHasUsername()
    {
        //var request = new GetUserDataRequest { Keys = new List<string> { "username"} };
        //PlayFabClientAPI.GetUserData(request,
        //    CheckHasUsernameSuccess =>
        //    {
        //        if (CheckHasUsernameSuccess.Data.ContainsKey("username"))
        //        {
        //            loadingScreen.gameObject.SetActive(false);
        //            Debug.Log("Đã có tên nhân vật");
        //            AsyncLoadingScene.instance.LoadNewScene(1);
        //        }
        //        else
        //        {
        //            loadingScreen.gameObject.SetActive(false);
        //            Debug.Log("Chưa có tên nhân vật");
        //            setNameScreen.gameObject.SetActive(true);
        //        }
        //    },
        //    CheckHasUsernameError => 
        //    {
        //        Debug.Log(CheckHasUsernameError.Error);
        //    });

        PlayFabClientAPI.GetAccountInfo(new GetAccountInfoRequest
        {           
        }, result =>
        {
            if (result.AccountInfo.TitleInfo.DisplayName != null)
            {
                loadingScreen.gameObject.SetActive(false);
                Debug.Log("Đã có tên nhân vật" + result.AccountInfo.TitleInfo.DisplayName);
                PlayfabDataManager.instance.GetDisplayname(result.AccountInfo.TitleInfo.DisplayName);
                AsyncLoadingScene.instance.LoadNewScene(1);
            }
            else
            {
                loadingScreen.gameObject.SetActive(false);
                Debug.Log("Chưa có tên nhân vật");
                setNameScreen.gameObject.SetActive(true);
            };
        }, error =>
        {
            Debug.Log("Đặt tên nhân vật thất bại");
        });
    }
    #endregion

    #region Tạo nhân vật
    //tạo tên nhân vật------------------------------------------------------------------------------------------------------------------------------------------------------------------
    protected void SetName()
    {
        if (!IsValidNameInput())
        {
            Debug.Log("IsValidNameInput");
            return;
        }
        //var request = new UpdateUserDataRequest {
        //    Data = new Dictionary<string, string>
        //    {
        //        {"username", nameInput.text}
        //    }
        //};
        //PlayFabClientAPI.UpdateUserData(request, 
        //    SetNameSuccess =>
        //    {
        //        Debug.Log("Đặt tên nhân vật thành công");
        //        AsyncLoadingScene.instance.LoadNewScene(1);
        //    }, SetNameError =>
        //    {
        //        Debug.Log("Đặt tên nhân vật thất bại");
        //    });


        PlayFabClientAPI.UpdateUserTitleDisplayName(new UpdateUserTitleDisplayNameRequest
        {
            DisplayName = nameInput.text
        }, result =>
        {
            Debug.Log("Đặt tên nhân vật thành công");
            ExecuteCloudScriptRequest request = new ExecuteCloudScriptRequest
            {
                FunctionName = "InitDataForNewPlayer"
            };
            PlayFabClientAPI.ExecuteCloudScript(request,
                result =>
                {
                    Debug.Log("Khởi chỉ dữ liệu mặc định thành công");
                },
                error =>
                {
                    Debug.Log("Khởi dữ liệu mặc định thất bại" + error.Error);
                });
            PlayfabDataManager.instance.GetDisplayname(nameInput.text);
            AsyncLoadingScene.instance.LoadNewScene(1);
        }, error =>
        {
            Debug.Log("Đặt tên nhân vật thất bại");
        });
    }
    #endregion

    #region Đăng ký
    //đăng ký------------------------------------------------------------------------------------------------------------------------------------------------------------------
    protected void SignUp()
    {
        if (!IsValidSignUp())
        {
            return;
        }
        loadingScreen.gameObject.SetActive(true);
        var registerRequest = new RegisterPlayFabUserRequest { Username = usernameSignUp.text, Password = passwordSignUp.text, Email = emailSignUp.text };
        PlayFabClientAPI.RegisterPlayFabUser(registerRequest, RegisterSuccess, RegisterError);
    }
    private void RegisterSuccess(RegisterPlayFabUserResult result)
    {
        loadingScreen.gameObject.SetActive(false);
        txtAlertSignUp.text = "Đăng ký thành công!";
        txtAlertSignUp.color = new Color32(0, 255, 0, 255);
    }

    private void RegisterError(PlayFabError error)
    {
        loadingScreen.gameObject.SetActive(false);
        if (error.Error == PlayFabErrorCode.UsernameNotAvailable)
        {
            txtAlertSignUp.text = "Đăng ký thất bại! Tên tài khoản đã được sử dụng!";
        }
        else if (error.Error == PlayFabErrorCode.EmailAddressNotAvailable)
        {
            txtAlertSignUp.text = "Đăng ký thất bại! Email đã được sử dụng cho một tài khoản khác!";
        }
        else
        {
            txtAlertSignUp.text = "Đăng ký thất bại! Hãy thử lại!";
        }
        txtAlertSignUp.color = new Color32(255, 0, 0, 255);
    }
    #endregion

    #region Quên mật khẩu
    //khôi phục mật khẩu------------------------------------------------------------------------------------------------------------------------------------------------------------------
    protected void ResetPassword()
    {
        if (!IsValidResetPassword())
        {
            return;
        }
        loadingScreen.gameObject.SetActive(true);
        var requestResetPassword = new SendAccountRecoveryEmailRequest { Email = emailResetPW.text, TitleId = PlayFabSettings.TitleId };
        PlayFabClientAPI.SendAccountRecoveryEmail(requestResetPassword, ResetPasswordSuccess, ResetPasswordError);
    }

    private void ResetPasswordError(PlayFabError obj)
    {
        loadingScreen.gameObject.SetActive(false);
        txtAlertResetPW.text = "Có lỗi khi gửi thư khôi phục mật khẩu, hãy liên hệ bộ phận CSKH!";
        txtAlertResetPW.color = new Color32(0, 127, 255, 255);
    }

    private void ResetPasswordSuccess(SendAccountRecoveryEmailResult obj)
    {
        loadingScreen.gameObject.SetActive(false);
        txtAlertResetPW.text = "Thư khôi phục mật khẩu đã được gửi, hãy kiểm tra email của bạn!";
        txtAlertResetPW.color = new Color32(0, 127, 255, 255);
    }
    #endregion

    #region Validate
    //validate------------------------------------------------------------------------------------------------------------------------------------------------------------------

    private bool IsValidSignIn()
    {
        txtAlertSignIn.color = new Color32(255, 0, 0, 255);
        if (usernameSignIn.text.Length <= 0)
        {
            txtAlertSignIn.text = "Vui lòng nhập tài khoản!";
            return false;
        }
        else if (passwordSignIn.text.Length <= 0)
        {
            txtAlertSignIn.text = "Vui lòng nhập mật khẩu!";
            return false;
        }
        return true;
    }

    private bool IsValidSignUp()
    {
        txtAlertSignUp.color = new Color32(255, 0, 0, 255);
        if (usernameSignUp.text.Length <= 0)
        {
            txtAlertSignUp.text = "Vui lòng nhập tài khoản!";
            return false;
        }
        else if (passwordSignUp.text.Length <= 0)
        {
            txtAlertSignUp.text = "Vui lòng nhập mật khẩu!";
            return false;
        }
        else if (confirmPasswordSignUp.text.Length <= 0)
        {
            txtAlertSignUp.text = "Vui lòng nhập xác nhận mật khẩu!";
            return false;
        }
        else if (emailSignUp.text.Length <= 0)
        {
            txtAlertSignUp.text = "Vui lòng nhập email!";
            return false;
        }
        else if (!IsValidUsername(usernameSignUp.text))
        {
            txtAlertSignUp.text = "Tài khoản phải có độ dài từ 3 - 20 ký tự";
            return false;
        }
        else if (!IsValidPassword(passwordSignUp.text))
        {
            txtAlertSignUp.text = "Mật khẩu phải có độ dài từ 6 - 100 ký tự";
            return false;
        }
        else if (!IsValidPassword2(passwordSignUp.text, confirmPasswordSignUp.text))
        {
            txtAlertSignUp.text = "Nhập lại Mật khẩu không trùng khớp";
            return false;
        }
        else if (!IsValidEmail(emailSignUp.text))
        {
            txtAlertSignUp.text = "Email không đúng định dạng";
            return false;
        }
        else
        {
            return true;
        }
    }

    private bool IsValidResetPassword()
    {
        txtAlertResetPW.color = new Color32(255, 0, 0, 255);
        if (emailResetPW.text.Length <= 0)
        {
            txtAlertResetPW.text = "Vui lòng nhập email!";
            return false;
        }
        else if (!IsValidEmail(emailResetPW.text))
        {
            txtAlertResetPW.text = "Email không đúng định dạng";
            return false;
        }
        return true;
    }


    private bool IsValidUsername(string username)
    {
        bool isValid = false;
        if (username.Length >= 3 && username.Length <= 20)
        {
            isValid = true;
        }
        return isValid;
    }

    private bool IsValidPassword(string password)
    {
        bool isValid = false;
        if (password.Length >= 6)
        {
            isValid = true;
        }
        return isValid;
    }

    private bool IsValidPassword2(string password1, string password2)
    {
        bool isValid = false;
        if (password1 == password2)
        {
            isValid = true;
        }
        return isValid;
    }

    private bool IsValidEmail(string email)
    {
        bool isValid = false;
        try
        {
            MailAddress m = new MailAddress(email);
            isValid = true;
        }
        catch
        {
            isValid = false;
        }
        return isValid;
    }

    private bool IsValidNameInput()
    {
        txtAlertSetName.color = new Color32(255, 0, 0, 255);
        if (nameInput.text.Length < 5 || nameInput.text.Length > 10)
        {
            txtAlertSetName.text = "Tên nhân vật phải có độ dài từ 5-10 ký tự!";
            return false;
        }
        return true;
    }
    #endregion
}
