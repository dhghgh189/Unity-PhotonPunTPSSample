using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerController : MonoBehaviourPun, IPunObservable
{
    [SerializeField] Renderer _renderer;
    [SerializeField] float moveSpeed;
    [SerializeField] float angleYMin; // y축 - 각도 제한
    [SerializeField] float angleYMax; // y축 + 각도 제한
    [SerializeField] float sensitivity;

    // 플레이어의 직접적인 회전없이 카메라 회전을 하기 위한 pivot
    [SerializeField] Transform rotatePivot;

    float angleX;
    float angleY;

    private void Start()
    {
        // 네트워크 오브젝트 생성시 넘겨줬던 인자값을 받아온다.
        object[] data = photonView.InstantiationData; 
        // 받아온 data를 통해 color 설정 및 동기화
        _renderer.material.color = new Color
            ((float)data[0], (float)data[1], (float)data[2], (float)data[3]);

        // 소유권 확인
        if (!photonView.IsMine)
            return;

        // 카메라 컨트롤러를 위한 설정
        FindAnyObjectByType<CameraController>().SetPivot(rotatePivot);
    }

    private void Update()
    {
        // 소유권 확인
        if (!photonView.IsMine)
            return;

        Move();
        Rotation();
    }

    private void Move()
    {
        // 유저 입력 확인
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");

        Vector3 direction = new Vector3(x, 0, z).normalized;

        // 값이 0인 경우 return
        if (direction == Vector3.zero)
            return;

        Vector3 worldDirection = transform.TransformDirection(direction);
        transform.position += worldDirection * moveSpeed * Time.deltaTime;
    }

    private void Rotation()
    {
        angleX += Input.GetAxisRaw("Mouse X") * sensitivity;
        angleY -= Input.GetAxis("Mouse Y") * sensitivity;

        angleY = Mathf.Clamp(angleY, angleYMin, angleYMax);

        // 좌우 회전 (플레이어 본체를 회전)
        transform.rotation = Quaternion.Euler(transform.eulerAngles.x, angleX, transform.eulerAngles.z);
        // 상하 회전 (pivot만 회전)
        rotatePivot.rotation = Quaternion.Euler(angleY, transform.eulerAngles.y, transform.eulerAngles.z);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        
    }
}
