using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

using PhotonHashtable = ExitGames.Client.Photon.Hashtable;

public class LobbyScene : MonoBehaviourPunCallbacks
{
    public enum Panel { LobbyPanel, RoomPanel }

    [SerializeField] GameObject menuPanel;
    [SerializeField] UI_RoomListPanel roomListPanel;
    [SerializeField] UI_RoomPanel roomPanel;
    [SerializeField] UI_CreateRoomPanel createRoomPanel;

    void Start()
    {
        // ȣ��Ʈ�� �� ��Ȳ�� ����ȭ�ϵ��� ����
        PhotonNetwork.AutomaticallySyncScene = true;

        // �κ� ���� �õ�
        // ���� ������ ���±��� ����ߴٰ� ����
        StartCoroutine(LobbyConnectRoutine());
    }

    // �κ� ���� ��
    public override void OnJoinedLobby()
    {
        Debug.Log("�κ� ���� ����");

        // ������ �ο��� �濡�� ���ͼ� �ٽ� �κ� �������� ��
        // ���� �������� ���� ���� ���̴� ���� ���ڿ�������Ƿ�
        // �κ� �������ڸ��� �ѹ� entry�� clear ���ִ� �۾� ����
        // ���� OnRoomListUpdate�� ȣ��ǹǷ� �ٽ� �ֽ�ȭ ��
        roomListPanel.ClearRoomEntries();

        SetActivePanel(Panel.LobbyPanel);
    }

    // �κ� ���� ��
    public override void OnLeftLobby()
    {
        Debug.Log("�κ񿡼� ����");
        PhotonNetwork.Disconnect();
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log($"LobbyScene ���� ���� : {cause}");
        SceneManager.LoadScene("LoginScene");
    }

    // �κ� �ִ� ���� ��鿡 ���� ������ ����
    // ���� 1�� ��ü �濡 ���� ������ �޾ƿ���
    // ���Ŀ��� ��������� �ִ� ���� �������� �޾ƿ´�.
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        roomListPanel.UpdateRoomList(roomList);
    }

    // �� ���� ��
    public override void OnCreatedRoom()
    {
        Debug.Log($"�� ���� ���� : {PhotonNetwork.CurrentRoom.Name}");
        createRoomPanel.gameObject.SetActive(false);
    }

    // �濡 �� ���
    public override void OnJoinedRoom()
    {
        Debug.Log($"�� ���� ���� : {PhotonNetwork.CurrentRoom.Name}");
        SetActivePanel(Panel.RoomPanel);
    }

    // �� ���� ����
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log($"�� ���� ���� : {message}");
    }

    // �� ���� ����
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.Log($"�� ���� ���� : {message}");

        // �� ���� ���� �� Lobby���� ������ ���°� �ǹǷ� �ٽ� Lobby�� ���� �õ�
        StartCoroutine(LobbyConnectRoutine());
    }

    // ���� ���� ����
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log($"���� ���� ���� {message}");
    }

    // �濡�� ���� ���
    public override void OnLeftRoom()
    {
        Debug.Log("�濡�� ����");

        // �濡�� ���� �� �ٷ� �κ� ����õ��� ���� �� ��Ʈ��ũ�� �غ���� �ʾ�
        // �ڷ�ƾ���� ���ᰡ���� ���°� �� �� ���� ����ϵ��� ó��
        StartCoroutine(LobbyConnectRoutine());
    }

    // �� �÷��̾ �濡 ���� ��
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        roomPanel.EnterPlayer(newPlayer);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        roomPanel.ExitPlayer(otherPlayer);
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, PhotonHashtable changedProps)
    {
        roomPanel.UpdateRoomInfo();
    }

    public void SetActivePanel(Panel panel)
    {
        menuPanel.gameObject.SetActive(panel == Panel.LobbyPanel);
        roomListPanel.gameObject.SetActive(panel == Panel.LobbyPanel);
        roomPanel.gameObject.SetActive(panel == Panel.RoomPanel);
    }

    IEnumerator LobbyConnectRoutine()
    {
        // ���� ��⸦ ���� ���� Counter
        int iCount = 500;

        while (true)
        {
            // ���� �κ� ������ ���� �� ���°� �ƴ϶�� (��Ʈ��ũ�� ���� �غ���� ����)
            if (PhotonNetwork.NetworkClientState != ClientState.ConnectedToMasterServer)
            {
                // ī��Ʈ ����
                iCount--;
                // ī��Ʈ�� ��� �����Ǹ� Timeout ó��
                if (iCount <= 0)
                {
                    Debug.LogError("Timeout! : LobbyConnectRoutine");
                    yield break;
                }

                // �� ������ ����� ó������ �ݺ�
                yield return null;
            }
            // �κ� ������ ������ ���°� �� ���
            else
            {
                // �κ� ���� �õ�
                PhotonNetwork.JoinLobby();
                break;
            }
        }
    }
}
