using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoginScene : MonoBehaviourPunCallbacks
{
    [SerializeField] UI_Login loginUI;
    // ������ ���� ������ ���
    public override void OnConnectedToMaster()
    {
        // �κ� ������ �ܼ� �̵�
        SceneManager.LoadScene("LobbyScene");
    }

    // ���� ���� ���
    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log($"LoginScene ���� ���� : {cause}");
        loginUI.ShowInfoPanel($"Login Failed...\n( {cause} )", Color.red, false);
        loginUI.HideInfoPanel(true);
    }
}
