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
        // rpc 호출을 통한 함수 동기화
        photonView.RPC("FireRPC", RpcTarget.AllViaServer, color.r, color.g, color.b, color.a);
    }

    // RPC : 원격 함수 호출
    // RpcTarget로 지정한 Target 들을 대상으로 RPC 호출을 진행
    // PhotonMessageInfo 타입의 매개변수를 정의하는 경우 추가정보가 전달된다.
    [PunRPC]
    public void FireRPC(float r, float g, float b, float a, PhotonMessageInfo info)
    {
        // 현재시간 - rpc 호출 시간을 통해 지연시간을 계산
        float lag = (float)(PhotonNetwork.Time - info.SentServerTime);

        Debug.Log($"Lag : {lag}");

        // 지연시간동안 이동했을 예상위치만큼 증가시켜 보정한다.
        // 바라보는 방향 (muzzleTransform.rotation * Vector3.forward) 으로 지연시간 만큼 이동 (moveSpeed * lag)
        Vector3 interpolatePosition = muzzleTransform.position + (muzzleTransform.rotation * Vector3.forward) * bulletPrefab.moveSpeed * lag;

        // 지연시간을 보정한 위치를 기준으로 bullet 생성
        Bullet bullet = Instantiate(bulletPrefab, interpolatePosition, muzzleTransform.rotation);
        
        // Color 동기화
        Color color = new Color(r, g, b, a);
        bullet.SetColor(color);

        // 지연시간 보정을 통한 파괴 시간 동기화
        bullet.RequestDestroy(lag);
    }
}
