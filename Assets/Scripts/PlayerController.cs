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
    [SerializeField] float angleYMin; // y�� - ���� ����
    [SerializeField] float angleYMax; // y�� + ���� ����
    [SerializeField] float sensitivity;

    // �÷��̾��� �������� ȸ������ ī�޶� ȸ���� �ϱ� ���� pivot
    [SerializeField] Transform rotatePivot;

    float angleX;
    float angleY;

    private void Start()
    {
        // ��Ʈ��ũ ������Ʈ ������ �Ѱ���� ���ڰ��� �޾ƿ´�.
        object[] data = photonView.InstantiationData; 
        // �޾ƿ� data�� ���� color ���� �� ����ȭ
        _renderer.material.color = new Color
            ((float)data[0], (float)data[1], (float)data[2], (float)data[3]);

        // ������ Ȯ��
        if (!photonView.IsMine)
            return;

        // ī�޶� ��Ʈ�ѷ��� ���� ����
        FindAnyObjectByType<CameraController>().SetPivot(rotatePivot);
    }

    private void Update()
    {
        // ������ Ȯ��
        if (!photonView.IsMine)
            return;

        Move();
        Rotation();
    }

    private void Move()
    {
        // ���� �Է� Ȯ��
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");

        Vector3 direction = new Vector3(x, 0, z).normalized;

        // ���� 0�� ��� return
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

        // �¿� ȸ�� (�÷��̾� ��ü�� ȸ��)
        transform.rotation = Quaternion.Euler(transform.eulerAngles.x, angleX, transform.eulerAngles.z);
        // ���� ȸ�� (pivot�� ȸ��)
        rotatePivot.rotation = Quaternion.Euler(angleY, transform.eulerAngles.y, transform.eulerAngles.z);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        
    }
}
