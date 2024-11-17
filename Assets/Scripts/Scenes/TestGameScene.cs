using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestGameScene : MonoBehaviourPunCallbacks
{
    // ���� �׽�Ʈ�� ���� �׽�Ʈ ���Ӿ� ����

    // �׽�Ʈ �� name
    const string ROOM_NAME = "TestRoom";

    [SerializeField] Color[] colors;

    void Start()
    {
        PhotonNetwork.NickName = $"Player_{Random.Range(1000, 10000)}";
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        RoomOptions options = new RoomOptions();

        // �ִ� ������ �� ����
        options.MaxPlayers = Define.MAX_PLAYER;
        // ���Ͽ��� ������ �ʵ��� ��
        options.IsVisible = false;

        // ���� �ִٸ� �����ϰ� ���ٸ� ����
        PhotonNetwork.JoinOrCreateRoom(ROOM_NAME, options, TypedLobby.Default);
    }

    // �� ���� ��
    public override void OnJoinedRoom()
    {
        StartCoroutine(WaitRoutine());
    }

    IEnumerator WaitRoutine()
    {
        yield return new WaitForSeconds(1f);
        StartTest();
    }

    void StartTest()
    {
        Debug.Log("<color=yellow>[TestMode]</color> Start Test");

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        SpawnPlayer();
    }

    void SpawnPlayer()
    {
        Vector3 spawnPos = new Vector3(Random.Range(-5, 5), 0, Random.Range(-5, 5));

        Color color = colors[PhotonNetwork.LocalPlayer.GetPlayerNumber()];
        // ��Ʈ��ũ ������Ʈ ���� �� PhotonNetwork�� Instantiate�� ���
        // �� �� Resources ������ �ִ� ������Ʈ�� �̸��� ���� �����ϰ� �ȴ�. 
        // ���� �߰����� �����͸� ���ڷμ� �Ѱ��� �� �����Ƿ� Color ���� �߰��� ����
        GameObject player = PhotonNetwork.Instantiate("Player", spawnPos, Quaternion.identity, data: new object[] {color.r, color.g, color.b, color.a});
    }
}
