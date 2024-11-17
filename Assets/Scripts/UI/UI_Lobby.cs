using Firebase.Auth;
using Firebase.Extensions;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class UI_Lobby : UIBase
{
    private void OnEnable()
    {
        // Create Room ��ư �̺�Ʈ �߰�
        AddUIEvent(Get("btnCreateRoom"), Enums.UIEvent.PointerClick, CreateRoom);
        // Quick Join ��ư �̺�Ʈ �߰�
        AddUIEvent(Get("btnQuickJoin"), Enums.UIEvent.PointerClick, QuickJoin);
        // Leave Lobby ��ư �̺�Ʈ �߰�
        AddUIEvent(Get("btnLeaveLobby"), Enums.UIEvent.PointerClick, LeaveLobby);
        // Change Profile ��ư �̺�Ʈ �߰�
        AddUIEvent(Get("btnChangeProfile"), Enums.UIEvent.PointerClick, ShowAuthPanelForChangeProfile);
        // Delete Account ��ư �̺�Ʈ �߰�
        AddUIEvent(Get("btnDeleteAccount"), Enums.UIEvent.PointerClick, ShowAuthPanelForDeleteAccount);
        // Confirm Change Profile ��ư �̺�Ʈ �߰�
        AddUIEvent(Get("btnConfirmChangeProfile"), Enums.UIEvent.PointerClick, ConfirmChangeProfile);

        // Create Room Panel ��Ȱ��ȭ
        Get("CreateRoomPanel").gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        // Create Room ��ư �̺�Ʈ ����
        RemoveUIEvent(Get("btnCreateRoom"), Enums.UIEvent.PointerClick, CreateRoom);
        // Quick Join ��ư �̺�Ʈ ����
        RemoveUIEvent(Get("btnQuickJoin"), Enums.UIEvent.PointerClick, QuickJoin);
        // Leave Lobby ��ư �̺�Ʈ ����
        RemoveUIEvent(Get("btnLeaveLobby"), Enums.UIEvent.PointerClick, LeaveLobby);
        // Change Profile ��ư �̺�Ʈ ����
        RemoveUIEvent(Get("btnChangeProfile"), Enums.UIEvent.PointerClick, ShowAuthPanelForChangeProfile);
        // Delete Account ��ư �̺�Ʈ ����
        RemoveUIEvent(Get("btnDeleteAccount"), Enums.UIEvent.PointerClick, ShowAuthPanelForDeleteAccount);
        // Confirm Change Profile ��ư �̺�Ʈ ����
        RemoveUIEvent(Get("btnConfirmChangeProfile"), Enums.UIEvent.PointerClick, ConfirmChangeProfile);
    }

    public void CreateRoom(PointerEventData eventData)
    {
        // Create Room Panel Ȱ��ȭ
        Get("CreateRoomPanel").gameObject.SetActive(true);
    }

    public void QuickJoin(PointerEventData eventData)
    {
        // ���� ���� (���� ������ ���� ���� ��� ���� ����)
        RoomOptions option = new RoomOptions();
        option.MaxPlayers = Define.MAX_PLAYER;
        PhotonNetwork.JoinRandomOrCreateRoom(roomName: $"{PhotonNetwork.LocalPlayer.NickName}'s Room", roomOptions: option);
    }

    #region Change Profile
    public void ShowAuthPanelForChangeProfile(PointerEventData eventData)
    {
        Get<UI_CheckAuthPanel>("CheckAuthPanel").EnableAuthPanel(
        // success callback
        () =>
        {
            Get("CheckAuthPanel").SetActive(false);
            Get("ChangeProfilePanel").SetActive(true);
        },
        // failed callback
        () =>
        {
            Debug.LogError("ReauthenticateAsync encountered an error");
            // �Ϸ���� ���� ���� ���
            ShowInfoPopup("Check Account!", Color.red);
        });
    }

    public void ConfirmChangeProfile(PointerEventData eventData)
    {
        // text field�� ������ �����´�.
        string nickName = Get<InputField>("ChangeNickNameInputField").text;
        string pass = Get<InputField>("ChangePasswordInputField").text;
        string confirmPass = Get<InputField>("ChangePasswordConfirmInputField").text;

        // �г����� �Է����� ���� ���
        if (string.IsNullOrEmpty(nickName))
        {
            ShowInfoPopup("Please Enter NickName!");
            return;
        }

        // �н����� �Է°����� ��ġ���� �ʴ� ���
        if (pass != confirmPass)
        {
            ShowInfoPopup("Please Check Confirm Password!");
            return;
        }

        FirebaseUser user = BackendManager.Auth.CurrentUser;

        ChangeProfile(user, nickName, pass);
    }

    public void ChangeProfile(FirebaseUser user, string nickName, string password)
    {
        // �г��� ����
        if (user.DisplayName != nickName)
        {
            UserProfile profile = new UserProfile();
            profile.DisplayName = nickName;

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
                    Debug.LogError("UpdateUserProfileAsync encountered an error");
                    string msg = task.Exception.Message;

                    // �Ϸ���� ���� ���� ���
                    ShowInfoPopup($"{msg.Substring(msg.IndexOf('(') + 1).Replace(')', ' ')}");
                    return;
                }

                // ���� ���� nickname ���� �ǽð����� ����
                PhotonNetwork.LocalPlayer.NickName = nickName;
                ShowInfoPopup("Update NickName Successfully!", Color.green);
                ChangePassword(user, password);
            });
        }
        else
        {
            // �г��� �������� �ʴ� ��� �ٷ� �н����� ���� �õ�
            ChangePassword(user, password);
        }
    }

    public void ChangePassword(FirebaseUser user, string password)
    {
        // �н����� ����
        Get<UI_CheckAuthPanel>("CheckAuthPanel").CheckInvalid(user, user.Email, password, null,
        // �Ѱܹ��� password�� ���� �������� �����ϴ� ��� ��й�ȣ�� ������й�ȣ�� �ƴ϶�� ���� Ȯ�ε�
        // �Ѱܹ��� password�� ���� password�� �ٸ� ��� ������ �õ�
        () =>
        {
            if (!string.IsNullOrEmpty(password))
            {
                user.UpdatePasswordAsync(password).ContinueWithOnMainThread(task =>
                {
                    // �۾��� ��ҵ� ���
                    if (task.IsCanceled)
                    {
                        ShowInfoPopup("UpdatePasswordAsync was canceled.");
                        return;
                    }
                    // �۾��� �Ϸ���� ���� ���
                    if (task.IsFaulted)
                    {
                        Debug.LogError("UpdatePasswordAsync encountered an error");
                        string msg = task.Exception.Message;

                        // �Ϸ���� ���� ���� ���
                        ShowInfoPopup($"{msg.Substring(msg.IndexOf('(') + 1).Replace(')', ' ')}");
                        return;
                    }

                    Debug.Log("Update Password Successfully!");
                    ShowInfoPopup("Update Password Successfully!", Color.green);
                });
            }
        });
    }
    #endregion

    #region Delete Account
    public void ShowAuthPanelForDeleteAccount(PointerEventData eventData)
    {
        Get<UI_CheckAuthPanel>("CheckAuthPanel").EnableAuthPanel(
        // success callback
        () =>
        {
            Get("CheckAuthPanel").SetActive(false);
            ShowQuestionPopup(DeleteAccount, "Are you sure you want to delete account?", Color.red);
        },
        // failed callback
        () =>
        {
            Debug.LogError("ReauthenticateAsync encountered an error");
            // �Ϸ���� ���� ���� ���
            ShowInfoPopup("Check Account!", Color.red);
        });
    }

    public void DeleteAccount()
    {
        FirebaseUser user = BackendManager.Auth.CurrentUser;

        user.DeleteAsync().ContinueWithOnMainThread(task =>
        {
            // �۾��� ��ҵ� ���
            if (task.IsCanceled)
            {
                ShowInfoPopup("DeleteAsync was canceled.");
                return;
            }
            // �۾��� �Ϸ���� ���� ���
            if (task.IsFaulted)
            {
                Debug.LogError($"DeleteAsync encountered an error");
                string msg = task.Exception.Message;

                // �Ϸ���� ���� ���� ���
                ShowInfoPopup($"{msg.Substring(msg.IndexOf('(') + 1).Replace(')', ' ')}");
                return;
            }

            // ���� ���� �Ϸ�
            Debug.Log("Account Delete Successfully!");

            // �κ񿡼� ����
            PhotonNetwork.LeaveLobby();
            Get("QuestionPopup").SetActive(false);
        });
    }
    #endregion

    public void LeaveLobby(PointerEventData eventData)
    {
        // �κ� ������ ��û
        PhotonNetwork.LeaveLobby();
    }

    public void ShowInfoPopup(in string msg, Color? color = null)
    {
        Get<Text>("InfoPopupMessageText").text = msg;
        Get<Text>("InfoPopupMessageText").color = color ?? Color.black;
        Get("InfoPopup").SetActive(true);
    }

    public void ShowQuestionPopup(UnityAction yesCallback, in string msg, Color? color = null)
    {
        Get<UI_QuestionPopup>("QuestionPopup").ShowQuestionPopup(yesCallback, msg, color);
    }
}
