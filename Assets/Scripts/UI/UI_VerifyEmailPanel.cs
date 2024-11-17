using Firebase.Auth;
using Firebase.Extensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_VerifyEmailPanel : UIBase
{
    Coroutine _verifyRoutine;
    WaitForSeconds _waitTime = new WaitForSeconds(1f);

    // �г� Ȱ��ȭ �� 
    void OnEnable()
    {
        FirebaseUser user = BackendManager.Auth.CurrentUser;

        Get<Text>("labelVerifyEmail").text = "An email has been sent for authentication.\r\nPlease Check Your E-mail !";

        // ������ ���� �̸��� ���� ��û
        user.SendEmailVerificationAsync().ContinueWithOnMainThread(task =>
        {
            // �۾��� ��ҵ� ���
            if (task.IsCanceled)
            {
                Get<Text>("labelVerifyEmail").text = "SendEmailVerificationAsync was canceled.";
                return;
            }
            // �۾��� �Ϸ���� ���� ���
            if (task.IsFaulted)
            {
                string msg = task.Exception.Message;
                Get<Text>("labelVerifyEmail").text = $"SendEmailVerificationAsync encountered an error\n({msg.Substring(msg.IndexOf('(') + 1).Replace(')', ' ')})";
                return;
            }

            // �̸��� �۽� �Ϸ� �� ���� ��� �ڷ�ƾ ����
            _verifyRoutine = StartCoroutine(VerifyRoutine());
        });
    }

    private void OnDisable()
    {
        // �г� ��Ȱ��ȭ �� �ڷ�ƾ �Ҵ�����
        if (_verifyRoutine != null)
        {
            StopCoroutine(_verifyRoutine);
            _verifyRoutine = null;
        }
    }

    IEnumerator VerifyRoutine()
    {
        FirebaseUser user = BackendManager.Auth.CurrentUser;

        while (true)
        {
            // �̸��� ���� ���� Ȯ�� �� �÷��̾� ���� Reload ��û
            user.ReloadAsync().ContinueWithOnMainThread(task =>
            {
                // �۾��� ��ҵ� ���
                if (task.IsCanceled)
                {
                    Get<Text>("labelVerifyEmail").text = "ReloadAsync was canceled.";
                    return;
                }
                // �۾��� �Ϸ���� ���� ���
                if (task.IsFaulted)
                {
                    string msg = task.Exception.Message;
                    Get<Text>("labelVerifyEmail").text = $"ReloadAsync encountered an error\n({msg.Substring(msg.IndexOf('(') + 1).Replace(')', ' ')})";
                    return;
                }

                // ���� ���� Ȯ��
                if (user.IsEmailVerified)
                {
                    Get<Text>("labelVerifyEmail").text = "Email Verified Successfully!\nPlease Login Again!";
                    return;
                }
            });

            // ��� ���
            yield return _waitTime;
        }
    }
}
