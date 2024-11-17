using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_RoomListPanel : UIBase
{
    [SerializeField] UI_RoomEntry roomEntryPrefab;

    Dictionary<string, UI_RoomEntry> roomDict = new Dictionary<string, UI_RoomEntry>();

    public void UpdateRoomList(List<RoomInfo> roomList)
    {
        foreach (RoomInfo room in roomList)
        {
            // ����Ʈ���� �������� �ʾƾ� �� Room�� ���
            if (room.RemovedFromList || room.IsVisible == false || room.IsOpen == false)
            {
                // ���� �������� ���� ��Ȳ�̿��ٸ� �����ص� ������
                if (roomDict.ContainsKey(room.Name) == false)
                    continue;

                // Entry ����
                Destroy(roomDict[room.Name].gameObject);
                roomDict.Remove(room.Name);
            }
            // ���� ���ŵǴ� ��� (�̹� dictionary�� �����Ѵٸ� ���ŵǴ� ��Ȳ)
            else if (roomDict.ContainsKey(room.Name))
            {
                // ���� ����
                roomDict[room.Name].SetInfo(room);
            }
            // ���� �����Ǿ� �ϴ� ���� ��� (dictionary�� �������� ����)
            else if (roomDict.ContainsKey(room.Name) == false)
            {
                // �� Entry ����
                UI_RoomEntry entry = Instantiate(roomEntryPrefab, Get("ContentArea").transform);
                // ���� ����
                entry.SetInfo(room);
                // Entry �߰�
                roomDict.Add(room.Name, entry);
            }
        }
    }

    // ��� Entry ����
    public void ClearRoomEntries()
    {
        foreach (UI_RoomEntry entry in roomDict.Values)
        {
            Destroy(entry.gameObject);
        }
        roomDict.Clear();
    }
}
