using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BulletEnemyController : MonoBehaviour
{
    private Rigidbody2D _rigidbody;
    [SerializeField] private float _speed = 5f;
    [SerializeField] private GameObject _bulletImpactPrefab;
    void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _rigidbody.velocity = Vector3.zero;
    }
    private void FixedUpdate()
    {
        _rigidbody.velocity = transform.right * _speed * Time.fixedDeltaTime;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Player'a çarparsa yok et
        if (collision.gameObject.CompareTag("Player"))
        {            
            Destroy(gameObject);
            Destroy(Instantiate(_bulletImpactPrefab, transform.position, Quaternion.identity), 0.1f);
        }
    }
}
