using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class BulletEnemyController : MonoBehaviour
{
    private Rigidbody2D _rigidbody;
    [SerializeField] private float _speed = 5f;
    [SerializeField] int _bulletTypeNumber = 0;

    [SerializeField] private float _baslangicHizi = 12f;
    [SerializeField] private float _ivme = 2f;
    [SerializeField] private float _sinValue = 2f;
    private float _time = 0f;

    [SerializeField] private GameObject _bulletImpactPrefab;
    void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _rigidbody.velocity = Vector3.zero;
    }
    private void Update()
    {

    }
    private void FixedUpdate()
    {
        if (_bulletTypeNumber != 6)
        {
            _rigidbody.velocity = transform.right * _speed * Time.fixedDeltaTime;
        }
        else
        {
            ParabolicShot();
        }
    }
    private void ParabolicShot()
    {
        _time += Time.fixedDeltaTime;

        // Parabolik atýþ hesaplamasý
        float xHizi = _baslangicHizi;
        float yHizi = _baslangicHizi - _ivme * _time;

        // Mermiyi ileri doðru hareket ettirin
        Vector3 hareket = new Vector3(xHizi, yHizi, 0) * Time.fixedDeltaTime;
        transform.Translate(hareket);

        // Mermiyi oval þekilde hareket ettirin (elips yolu)
        float x = Mathf.Sin(Time.time) * _sinValue;
        float y = Mathf.Cos(Time.time) * _sinValue;
        transform.position += new Vector3(x, y, 0) * Time.fixedDeltaTime;

    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Player'a çarparsa yok et
        if (collision.gameObject.CompareTag("Player"))
        {
            BulletObjectPool.Instance.SetPooledObject(this.gameObject, _bulletTypeNumber);
            Destroy(Instantiate(_bulletImpactPrefab, transform.position, Quaternion.identity), 0.1f);
        }
        if (collision.gameObject.CompareTag("PlayerXAxisLimit"))
        {
            BulletObjectPool.Instance.SetPooledObject(this.gameObject, _bulletTypeNumber);
        }
    }
}
