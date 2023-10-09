using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    private Rigidbody2D _rigidbody;
    [SerializeField] int _bulletTypeNumber = 0;
    [SerializeField] private float _speed = 5f;
    [SerializeField] private float _rateOfFire = 0.1f;
    bool _isFacingRight = true;

    [SerializeField] private GameObject _bulletImpactPrefab;
    public float RateOfFire { get => _rateOfFire; set => _rateOfFire = value; }
    public bool IsFacingRight { get => _isFacingRight; set => _isFacingRight = value; }

    void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _rigidbody.velocity = Vector3.zero;
    }
    void Update()
    {

    }
    private void FixedUpdate()
    {
        if (IsFacingRight)
        {
            _rigidbody.velocity = transform.right * _speed * Time.fixedDeltaTime;
        }
        else
        {
            _rigidbody.velocity = -transform.right * _speed * Time.fixedDeltaTime;
        }
        
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Enemylere çarparsa yok et
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Debug.Log("Enemy is Hit");
            EnemyHit(collision.gameObject);
        }
    }
    void EnemyHit(GameObject enemy)
    {
        //Enemy içindeki EnemyHit fonk. çalýþacak
        GameObject bulletImpact = Instantiate(_bulletImpactPrefab, this.gameObject.transform.position, Quaternion.identity);
        Destroy(enemy, 0.5f);
        Destroy(bulletImpact, 0.5f);
        BulletObjectPool.Instance.SetPooledObject(this.gameObject, _bulletTypeNumber);
        enemy.tag = "Untagged";
    }
}
