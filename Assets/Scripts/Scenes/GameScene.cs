using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameScene : MonoBehaviourPunCallbacks
{
    public override void OnLeftRoom()
    {
        Debug.Log("�濡�� �����Ͽ����ϴ�.");
        
        // �κ� ������ �̵�
        PhotonNetwork.LoadLevel("LobbyScene");
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        // �κ� ������ �̵�
        PhotonNetwork.LoadLevel("LobbyScene");
    }
}
