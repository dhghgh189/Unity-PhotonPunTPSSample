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

    //    // pivot을 기준으로 deltaPos만큼 이동한 위치로 이동
    //    transform.position = _pivot.position + deltaPos;
    //}
}
