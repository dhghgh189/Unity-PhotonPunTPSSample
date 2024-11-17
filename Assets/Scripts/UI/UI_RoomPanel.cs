using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_RoomPanel : UIBase
{
    [SerializeField] UI_PlayerEntry[] playerEntries;
    private void OnEnable()
    {
        // Ready Or Start ��ư �̺�Ʈ �߰�
        AddUIEvent(Get("btnReadyOrStart"), Enums.UIEvent.PointerClick, StartOrReady);
        // Leave Room ��ư �̺�Ʈ �߰�
        AddUIEvent(Get("btnLeaveRoom"), Enums.UIEvent.PointerClick, LeaveRoom);

        // ���� �泻 player numbering�� ������ �ִ� ��� ȣ��Ǵ� �̺�Ʈ
        PlayerNumbering.OnPlayerNumberingChanged += UpdateRoomInfo;

        // ���� ����� ready ���� ����
        PhotonNetwork.LocalPlayer.SetReady(false);
    }

    private void OnDisable()
    {
        // Ready Or Start ��ư �̺�Ʈ ����
        RemoveUIEvent(Get("btnReadyOrStart"), Enums.UIEvent.PointerClick, StartOrReady);
        // Leave Room ��ư �̺�Ʈ ����
        RemoveUIEvent(Get("btnLeaveRoom"), Enums.UIEvent.PointerClick, LeaveRoom);

        PlayerNumbering.OnPlayerNumberingChanged -= UpdateRoomInfo;
    }

    public void LeaveRoom(PointerEventData eventData)
    {
        // �� ������ ��û
        PhotonNetwork.LeaveRoom();
    }

    public void UpdateRoomInfo()
    {
        // Room UI �ʱ�ȭ
        Get<Text>("txtRoomName").text = PhotonNetwork.CurrentRoom.Name;
        Get<Text>("btnReadyOrStartText").text = "READY";

        // Entry �ʱ�ȭ
        foreach (UI_PlayerEntry entry in playerEntries)
        {
            entry.Clear();
        }

        // Entry ����
        foreach(Player player in PhotonNetwork.PlayerList)
        {
            // �÷��̾ ������ ��ȣ�� index�� ���
            int number = player.GetPlayerNumber();

            // ���� number ������ �ȵ� player�� ��� ����ó��
            if (number < 0)
                continue;

            // entry ����
            playerEntries[number].SetEntry(player);
        }

        // ������ ��� �߰��� ó���� ����
        if (PhotonNetwork.IsMasterClient)
        {
            // ��ư �ؽ�Ʈ ����
            Get<Text>("btnReadyOrStartText").text = "START";
            Get<Button>("btnReadyOrStart").interactable = CheckAllReady();
        }
    }

    // �� �÷��̾� ���� ��
    public void EnterPlayer(Player player)
    {
        Debug.Log($"{player.NickName} ����");
        UpdateRoomInfo();
    }

    // �ٸ� �÷��̾� ���� ��
    public void ExitPlayer(Player player)
    {
        Debug.Log($"{player.NickName} ����");
        UpdateRoomInfo();
    }

    public bool CheckAllReady()
    {
        foreach(Player player in PhotonNetwork.PlayerList)
        {
            // ������ ��� üũ�� �������� �ʴ´�.
            if (player.IsMasterClient)
                continue;

            // �Ѹ��̶� �غ����� �ʾҴٸ� false
            if (!player.GetReady())
                return false;
        }

        // ��� �غ��� ����
        return true;
    }

    public void StartOrReady(PointerEventData eventData)
    {
        // ������ ��� START ���� ó��
        if (PhotonNetwork.IsMasterClient)
        {
            if (!CheckAllReady())
                return;

            // �� �̵�
            PhotonNetwork.LoadLevel("GameScene");
        }
        // ������ �ƴ� ��� READY ���� ó��
        else
        {
            Player player = PhotonNetwork.LocalPlayer;

            // ������� ����
            player.SetReady(!player.GetReady());
        }
    }
}
