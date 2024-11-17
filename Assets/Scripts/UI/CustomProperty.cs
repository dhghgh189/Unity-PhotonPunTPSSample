using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PhotonHashtable = ExitGames.Client.Photon.Hashtable;

public static class CustomProperty
{
    static PhotonHashtable customProperty = new PhotonHashtable();
    public const string READY = "Ready";

    // ���޹��� �÷��̾��� Ŀ���� ������Ƽ 'Ready' �� ����
    public static void SetReady(this Player player, bool bReday)
    {
        customProperty.Clear();
        // Ready �� ����
        customProperty.Add(READY, bReday);
        // �÷��̾��� Ŀ���� ������Ƽ ������Ʈ
        player.SetCustomProperties(customProperty);
    }
    
    // ���޹��� �÷��̾��� Ŀ���� ������Ƽ 'Ready' �� ��ȯ
    public static bool GetReady(this Player player)
    {
        PhotonHashtable playerProperty = player.CustomProperties;

        // ���� �������� ������ false
        if (playerProperty.ContainsKey(READY) == false)
            return false;

        // �÷��̾��� Ready �� ��ȯ
        return (bool)playerProperty[READY];
    }
}
