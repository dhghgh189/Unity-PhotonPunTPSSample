using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] float destroyTime;
    [SerializeField] Renderer _renderer;
    public float moveSpeed;

    Rigidbody _rb;

    public void SetColor(Color color)
    {
        // Color 동기화
        _renderer.material.color = color;
    }

    public void RequestDestroy(float lag)
    {
        // 파괴 예정 시간에 지연시간을 보정하여 동기화
        Destroy(gameObject, destroyTime - lag);
    }

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    void Start()
    {
        _rb.velocity = transform.forward * moveSpeed;
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
            Destroy(gameObject);
    }
}
