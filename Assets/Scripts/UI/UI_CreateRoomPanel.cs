using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_CreateRoomPanel : UIBase
{
    private void OnEnable()
    {
        // create ��ư �̺�Ʈ �߰�
        AddUIEvent(Get("btnCreate"), Enums.UIEvent.PointerClick, CreateRoom);
        // cancel ��ư �̺�Ʈ �߰�
        AddUIEvent(Get("btnCancel"), Enums.UIEvent.PointerClick, Cancel);

        // �ʱ�ȭ
        Get<InputField>("RoomNameInputField").text = $"{PhotonNetwork.LocalPlayer.NickName}'s Room";
        Get<InputField>("MaxPlayerInputField").text = $"{Define.MAX_PLAYER}";
    }

    private void OnDisable()
    {
        // create ��ư �̺�Ʈ ����
        RemoveUIEvent(Get("btnCreate"), Enums.UIEvent.PointerClick, CreateRoom);
        // cancel ��ư �̺�Ʈ ����
        RemoveUIEvent(Get("btnCancel"), Enums.UIEvent.PointerClick, Cancel);
    }

    public void CreateRoom(PointerEventData eventData)
    {
        // RoomName Ȯ��
        if (string.IsNullOrEmpty(Get<InputField>("RoomNameInputField").text))
        {
            Debug.LogWarning("Please Input RoomName");
            return;
        }

        // Max Player Ȯ��
        if (string.IsNullOrEmpty(Get<InputField>("MaxPlayerInputField").text))
        {
            Debug.LogWarning("Please Input Max Player");
            return;
        }

        // �� �о����
        string roomName = Get<InputField>("RoomNameInputField").text;
        int maxPlayerCount = int.Parse(Get<InputField>("MaxPlayerInputField").text);
        maxPlayerCount = Mathf.Clamp(maxPlayerCount, 1, Define.MAX_PLAYER);

        RoomOptions option = new RoomOptions();
        // �ִ� �÷��̾� �� ����
        option.MaxPlayers = maxPlayerCount;

        // �� ���� ��û
        PhotonNetwork.CreateRoom(roomName, option);
    }

    public void Cancel(PointerEventData eventData)
    {
        gameObject.SetActive(false);
    }
}
