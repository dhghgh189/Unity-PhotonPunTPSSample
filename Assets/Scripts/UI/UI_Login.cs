using Firebase.Auth;
using Firebase.Extensions;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class UI_Login : UIBase
{
    [SerializeField] float hideDelay;
    WaitForSeconds _delay;

    // email check ����
    bool bCheckEmail = false;

    private void Start()
    {
        // InfoPanel�� �ʱ⿡ �����.
        HideInfoPanel();

        // Login ��ư�� �̺�Ʈ �߰�
        AddUIEvent(Get("btnLogin"), Enums.UIEvent.PointerClick, Login);
        // SignUp ��ư�� �̺�Ʈ �߰�
        AddUIEvent(Get("btnSignUp"), Enums.UIEvent.PointerClick, ShowSignUpPanel);
        // Check Email ��ư�� �̺�Ʈ �߰�
        AddUIEvent(Get("btnCheckEmail"), Enums.UIEvent.PointerClick, CheckEmail);
        // Sing up Confirm ��ư�� �̺�Ʈ �߰�
        AddUIEvent(Get("btnConfirm"), Enums.UIEvent.PointerClick, ConfirmSignUp);
        // NickName Confirm ��ư�� �̺�Ʈ �߰�
        AddUIEvent(Get("btnConfirmNickName"), Enums.UIEvent.PointerClick, ConfirmNickName);

        // signUpIdInputField �̺�Ʈ �߰�
        Get<InputField>("signUpIdInputField").onValueChanged.AddListener(str => 
        {
            // �����ߴ� �̸����� �����ϸ� �ٽ� �����ʿ�
            if (bCheckEmail != false)
            {
                Debug.Log("Need to Email Check Again");
                SetEmailCheck(false);
            }
        });

        _delay = new WaitForSeconds(hideDelay);
    }

    public void Login(PointerEventData eventData)
    {
        // �Է°��� �޾ƿ´�.
        string email = Get<InputField>("idInputField").text;
        string pass = Get<InputField>("passInputField").text;

        // ��ȿ���� �˻�
        if (string.IsNullOrEmpty(email))
        {
            Debug.LogWarning("Please Enter E-mail!");
            ShowInfoPopup("Please Enter E-mail!");
            return;
        }

        if (string.IsNullOrEmpty(pass))
        {
            Debug.LogWarning("Please Enter Password!");
            ShowInfoPopup("Please Enter Password!");
            return;
        }

        ShowInfoPanel("Please Wait...", Color.black);

        // �α��� ��û
        BackendManager.Auth.SignInWithEmailAndPasswordAsync(email, pass)
            .ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled)
            {
                HideInfoPanel();
                ShowInfoPopup("SignInWithEmailAndPasswordAsync was canceled.");
                return;
            }
            // �α��� ���� ��
            if (task.IsFaulted)
            {
                HideInfoPanel();
                Debug.Log("SignInWithEmailAndPasswordAsync encountered an error.");

                // Exception�� FirebaseException���� ��ȯ�Ͽ� ErrorCode�� ������
                // AuthError ���������� ��ȯ�Ͽ� �α��� ���� ������ Ȯ��
                AuthError eError = (AuthError)(task.Exception.GetBaseException() as Firebase.FirebaseException).ErrorCode;

                // ������ ���� �˾� ó��
                CheckAuthError(eError);
                return;
            }

            // �α��� ���� ��
            Debug.Log("Login Successfully!");

            // �α����� ������ �����´�.
            FirebaseUser user = task.Result.User;

            // ���� �̸��� ������ ���� ���� ���
            if (!user.IsEmailVerified)
            {
                HideInfoPanel();
                // �̸��� ������ ���� �г� ǥ��
                Get("VerifyEmailPanel").SetActive(true);
            }
            // ���� �г����� �������� ���� ���
            else if (string.IsNullOrEmpty(user.DisplayName))
            {
                HideInfoPanel();
                // �г��� ������ ���� �г� ǥ��
                Get("NickNamePanel").SetActive(true);
            }
            // �̸��� ���� �� �г��� ������ �Ϸ�� ������ ���
            else
            {
                // ���� ���� �õ�
                ConnectToMasterServer(user);
            }
        });
    }

    public void ConfirmNickName(PointerEventData eventData)
    {
        string nickName = Get<InputField>("NickNameInputField").text;
        if (string.IsNullOrEmpty(nickName))
        {
            ShowInfoPopup("Please Enter NickName!", Color.red);
            return;
        }

        FirebaseUser user = BackendManager.Auth.CurrentUser;

        // ���� ������ ������ ���� UserProfile ��ü
        UserProfile profile = new UserProfile();
        // nickName�ʵ� ����
        profile.DisplayName = nickName;
        // ������ ���� ��û
        user.UpdateUserProfileAsync(profile).ContinueWithOnMainThread(task =>
        {
            // �۾��� ��ҵ� ���
            if (task.IsCanceled)
            {
                ShowInfoPopup("UpdateUserProfileAsync was canceled.");
                return;
            }
            // �۾��� �Ϸ���� ���� ���
            if (task.IsFaulted)
            {
                Debug.LogError($"UpdateUserProfileAsync encountered an error");
                string msg = task.Exception.Message;

                // �Ϸ���� ���� ���� ���
                ShowInfoPopup($"{msg.Substring(msg.IndexOf('(') + 1).Replace(')', ' ')}");
                return;
            }

            Debug.Log($"Set NickName Successfully! : {nickName}");

            // ������ ���� ���� ��û
            ConnectToMasterServer(user);
        });
    }

    public void ConnectToMasterServer(FirebaseUser user)
    {
        // �г��� ���� �� ����
        PhotonNetwork.NickName = user.DisplayName;
        PhotonNetwork.ConnectUsingSettings();
    }

    public void CheckAuthError(AuthError eError)
    {
        switch (eError)
        {
            // �������� �ʴ� �̸����� ��� ���� �޽��� ���
            // Wrong Password�� ���� ���� ����� �˸��� ��� ��ŷ�� ������ �����Ƿ�
            // Wrong Password case�� �����Ͽ���
            case AuthError.UserNotFound:
                ShowInfoPopup("User not exist!", Color.red);
                break;
            case AuthError.Failure:
            case AuthError.ApiNotAvailable:
                ShowInfoPopup("System Error!", Color.red);
                break;
            default:
                ShowInfoPopup("Check Your Account!", Color.red);
                break;
        }
    }

    public void ShowInfoPanel(string msg, Color color, bool bShow = true)
    {
        Get<Text>("txtMessage").text = msg;
        Get<Text>("txtMessage").color = color;

        if (bShow)
            Get("InfoPanel").SetActive(true);
    }

    public void HideInfoPanel(bool bDelay = false)
    {
        if (!bDelay)
            Get("InfoPanel").SetActive(false);
        else
            StartCoroutine(HideRoutine());
    }

    IEnumerator HideRoutine()
    {
        yield return _delay;
        HideInfoPanel();
    }

    #region Sign Up
    public void ShowSignUpPanel(PointerEventData eventData)
    {
        // ȸ������ �г� Ȱ��ȭ
        Get("SignUpPanel").SetActive(true);

        // �ʱ�ȭ
        Get<InputField>("signUpIdInputField").text = string.Empty;
        Get<InputField>("signUpPassInputField").text = string.Empty;
        Get<InputField>("signUpConfirmPassInputField").text = string.Empty;
    }

    public void CheckEmail(PointerEventData eventData)
    {
        // �̸��� �ʵ尪�� �����´�
        string email = Get<InputField>("signUpIdInputField").text;
        if (string.IsNullOrEmpty(email))
        {
            ShowInfoPopup("Please Enter E-mail!");
            return;
        }

        // email�� ���� ���� ������ü ����� �����´�.
        // �ش� email�� ���� ������ ��Ͽ� ���� �ϳ��� ������ ������ �����ϴ� email�̸�
        // ��Ͽ� ���� �ϳ��� ���ٸ� ���� �������� �ʴ� email���� �Ǵ��Ѵ�.
        BackendManager.Auth.FetchProvidersForEmailAsync(email)
            .ContinueWithOnMainThread(task =>
        {
            // �۾��� ��ҵ� ���
            if (task.IsCanceled)
            {
                ShowInfoPopup("FetchProvidersForEmailAsync was canceled.");
                return;
            }
            // �۾��� �Ϸ���� ���� ���
            if (task.IsFaulted)
            {
                Debug.LogError($"FetchProvidersForEmailAsync encountered an error");
                string msg = task.Exception.Message;

                // �Ϸ���� ���� ���� ���
                ShowInfoPopup($"{msg.Substring(msg.IndexOf('(') + 1).Replace(')', ' ')}");
                return;
            }

            // ���� ������ü ����� �����ϸ� email �ߺ����� �Ǵ�
            if (task.Result.Count() > 0)
            {
                SetEmailCheck(false);
                ShowInfoPopup("This e-mail already exist!\nUse another e-mail", Color.red);
            }
            else
            {
                // �̸��� �ߺ�üũ ���
                SetEmailCheck(true);
                ShowInfoPopup("E-mail Check OK!", Color.green);
            }
        });
            
    }

    // email check ���� ����
    public void SetEmailCheck(bool bCheck)
    {
        bCheckEmail = bCheck;
        Get("imgCheckEmail").SetActive(bCheckEmail);
    }

    public void ConfirmSignUp(PointerEventData eventData)
    {
        // text field�� ������ �����´�.
        string email = Get<InputField>("signUpIdInputField").text;
        string pass = Get<InputField>("signUpPassInputField").text;
        string confirmPass = Get<InputField>("signUpConfirmPassInputField").text;

        // �̸��� �ߺ� Ȯ���� ������� ���� ���
        if (!bCheckEmail)
        {
            ShowInfoPopup("Please Check Email!");
            return;
        }

        // �н����� �Է��� �ȵ� ���
        if (string.IsNullOrEmpty(pass))
        {
            ShowInfoPopup("Please Enter Password!");
            return;
        }

        // �н����� �Է°����� ��ġ���� �ʴ� ���
        if (pass != confirmPass)
        {
            ShowInfoPopup("Please Check Confirm Password!");
            return;
        }

        // Firebase Auth�� ���� ���� ���� ��û (eamil, password�� ���� ���� ����)
        BackendManager.Auth.CreateUserWithEmailAndPasswordAsync(email, pass)
            .ContinueWithOnMainThread(task =>
        {
            // �۾��� ��ҵ� ���
            if (task.IsCanceled)
            {
                ShowInfoPopup("CreateUserWithEmailAndPasswordAsync was canceled.");
                return;
            }
            // �۾��� �Ϸ���� ���� ���
            if (task.IsFaulted)
            {
                Debug.LogError("CreateUserWithEmailAndPasswordAsync encountered an error");
                string msg = task.Exception.Message;

                // �Ϸ���� ���� ���� ���
                ShowInfoPopup($"{msg.Substring(msg.IndexOf('(') + 1).Replace(')', ' ')}");
                return;
            }

            // ���� ���� ���� ��
            Debug.Log("Create User Successfully!");
            Get("SignUpPanel").SetActive(false);
        });
    }

    public void ShowInfoPopup(string msg, Color? color = null)
    {
        Get<Text>("InfoPopupMessageText").text = msg;
        Get<Text>("InfoPopupMessageText").color = color ?? Color.black;
        Get("InfoPopup").SetActive(true);
    }
    #endregion
}
