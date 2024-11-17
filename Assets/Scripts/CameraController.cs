using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] Vector3 deltaPos;

    Transform _pivot;


    public void SetPivot(Transform pivot)
    {
        _pivot = pivot;
        transform.rotation = Quaternion.identity;
        transform.SetParent(pivot);

        transform.position = _pivot.position + deltaPos;
    }

    //private void LateUpdate()
    //{
    //    if (_pivot == null)
    //        return;

    //    // pivot�� �������� deltaPos��ŭ �̵��� ��ġ�� �̵�
    //    transform.position = _pivot.position + deltaPos;
    //}
}
