using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ClimberEnemyController : MonoBehaviour
{
    [SerializeField] private float _speed = 50f;
    [SerializeField] private float _jumpSpeed = 250f;

    [SerializeField] private GameObject _bulletImpactPrefab;
    private Rigidbody2D _rigidbody;
    void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
    }
    private void FixedUpdate()
    {
        Vector2 newVelocity = new Vector2(transform.right.x * _speed * Time.fixedDeltaTime, _rigidbody.velocity.y);
        _rigidbody.velocity = newVelocity;
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Temas edilen nesne bir mermi mi kontrol edin
        if (other.CompareTag("Bullet"))
        {
            StartCoroutine(EnemyHit());
        }
    }
    IEnumerator EnemyHit()
    {
        _rigidbody.AddForce(Vector2.up * _jumpSpeed * Time.fixedDeltaTime, ForceMode2D.Impulse);
        yield return new WaitForSeconds(0.25f);
        GetComponent<Collider2D>().enabled = false; 
        Destroy(gameObject);
        GameObject bulletImpact = Instantiate(_bulletImpactPrefab, this.gameObject.transform.position, Quaternion.identity);
        Destroy(bulletImpact, 0.5f);
    }
}
