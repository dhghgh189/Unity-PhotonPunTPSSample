using Firebase.Auth;
using Firebase.Extensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_QuestionPopup : UIBase
{
    UnityAction _yesCallback;

    private void Start()
    {
        // ó������ ��Ȱ��ȭ�Ǿ������� ui ���ε��� �ȵǹǷ�
        // Ȱ��ȭ �Ȼ��¿��� ���ε� �� start �� ��Ȱ��ȭ �ϵ��� �� 
        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        // Yes ��ư �̺�Ʈ �߰�
        AddUIEvent(Get("btnYes"), Enums.UIEvent.PointerClick, Execute);
    }

    private void OnDisable()
    {
        _yesCallback = null;

        // Yes ��ư �̺�Ʈ ����
        RemoveUIEvent(Get("btnYes"), Enums.UIEvent.PointerClick, Execute);
    }

    public void ShowQuestionPopup(UnityAction yesCallback, in string msg, Color? color = null)
    {
        _yesCallback = yesCallback;

        Get<Text>("QuestionPopupMessageText").text = msg;
        Get<Text>("QuestionPopupMessageText").color = color ?? Color.black;

        gameObject.SetActive(true);
    }

    public void Execute(PointerEventData eventData)
    {
        _yesCallback?.Invoke();
    }
}
