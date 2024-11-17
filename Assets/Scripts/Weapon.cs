using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviourPun
{
    [SerializeField] Transform muzzleTransform;
    [SerializeField] Bullet bulletPrefab;

    public void Fire(Color color)
    {
        // rpc ȣ���� ���� �Լ� ����ȭ
        photonView.RPC("FireRPC", RpcTarget.AllViaServer, color.r, color.g, color.b, color.a);
    }

    // RPC : ���� �Լ� ȣ��
    // RpcTarget�� ������ Target ���� ������� RPC ȣ���� ����
    // PhotonMessageInfo Ÿ���� �Ű������� �����ϴ� ��� �߰������� ���޵ȴ�.
    [PunRPC]
    public void FireRPC(float r, float g, float b, float a, PhotonMessageInfo info)
    {
        // ����ð� - rpc ȣ�� �ð��� ���� �����ð��� ���
        float lag = (float)(PhotonNetwork.Time - info.SentServerTime);

        Debug.Log($"Lag : {lag}");

        // �����ð����� �̵����� ������ġ��ŭ �������� �����Ѵ�.
        // �ٶ󺸴� ���� (muzzleTransform.rotation * Vector3.forward) ���� �����ð� ��ŭ �̵� (moveSpeed * lag)
        Vector3 interpolatePosition = muzzleTransform.position + (muzzleTransform.rotation * Vector3.forward) * bulletPrefab.moveSpeed * lag;

        // �����ð��� ������ ��ġ�� �������� bullet ����
        Bullet bullet = Instantiate(bulletPrefab, interpolatePosition, muzzleTransform.rotation);
        
        // Color ����ȭ
        Color color = new Color(r, g, b, a);
        bullet.SetColor(color);

        // �����ð� ������ ���� �ı� �ð� ����ȭ
        bullet.RequestDestroy(lag);
    }
}
