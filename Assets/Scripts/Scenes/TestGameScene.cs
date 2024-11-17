using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestGameScene : MonoBehaviourPunCallbacks
{
    // 빠른 테스트를 위한 테스트 게임씬 구성

    // 테스트 룸 name
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

        // 최대 참가자 수 설정
        options.MaxPlayers = Define.MAX_PLAYER;
        // 방목록에서 보이지 않도록 함
        options.IsVisible = false;

        // 방이 있다면 입장하고 없다면 생성
        PhotonNetwork.JoinOrCreateRoom(ROOM_NAME, options, TypedLobby.Default);
    }

    // 방 입장 시
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
        // 네트워크 오브젝트 생성 시 PhotonNetwork의 Instantiate를 사용
        // 이 때 Resources 폴더에 있는 오브젝트를 이름을 통해 생성하게 된다. 
        // 또한 추가적인 데이터를 인자로서 넘겨줄 수 있으므로 Color 값을 추가로 전달
        GameObject player = PhotonNetwork.Instantiate("Player", spawnPos, Quaternion.identity, data: new object[] {color.r, color.g, color.b, color.a});
    }
}
