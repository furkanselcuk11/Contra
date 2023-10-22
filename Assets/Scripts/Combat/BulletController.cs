using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class BulletController : MonoBehaviour
{
    private Rigidbody2D _rigidbody;
    [SerializeField] int _bulletTypeNumber = 0;
    [SerializeField] private float _speed = 5f;
    [SerializeField] private int _damage = 1;
    [SerializeField] private float _rateOfFire = 0.1f;
    bool _isFacingRight = true;
    [SerializeField] private bool _isBulletDouble = false;
    [SerializeField] private float _rotationSpeed = 100.0f; // Dönüþ hýzý
    private Transform _object1, _object2;

    [SerializeField] private GameObject _bulletImpactPrefab;

    public float RateOfFire { get => _rateOfFire; set => _rateOfFire = value; }
    public bool IsFacingRight { get => _isFacingRight; set => _isFacingRight = value; }
    public int Damage { get => _damage; set => _damage = value; }

    void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _rigidbody.velocity = Vector3.zero;
        if (_isBulletDouble)
        {
            _object1 = transform.Find("SpriteRenderer").GetComponent<Transform>();
            _object2 = transform.Find("SpriteRenderer_1").GetComponent<Transform>();
        }
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
        if (_isBulletDouble)
        {
            RotateObjectAroundPoint(_object1);
            RotateObjectAroundPoint(_object2);
        }
    }
    void RotateObjectAroundPoint(Transform objTransform)
    {
        objTransform.RotateAround(transform.position, Vector3.forward, _rotationSpeed * Time.fixedDeltaTime);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Enemylere çarparsa yok et
        if (collision.gameObject.CompareTag("Enemy"))
        {
            BulletObjectPool.Instance.SetPooledObject(this.gameObject, _bulletTypeNumber);
            Destroy(Instantiate(_bulletImpactPrefab, transform.position, Quaternion.identity), 0.1f);
            ScoreUpdate(100);
        }
        if (collision.gameObject.CompareTag("WeaponBox"))
        {
            BulletObjectPool.Instance.SetPooledObject(this.gameObject, _bulletTypeNumber);
            Destroy(Instantiate(_bulletImpactPrefab, transform.position, Quaternion.identity), 0.1f);
            ScoreUpdate(200);
        }
        if (collision.gameObject.CompareTag("BonusBox"))
        {
            BulletObjectPool.Instance.SetPooledObject(this.gameObject, _bulletTypeNumber);
            Destroy(Instantiate(_bulletImpactPrefab, transform.position, Quaternion.identity), 0.1f);
        }
        if (collision.gameObject.CompareTag("RedDoor"))
        {
            BulletObjectPool.Instance.SetPooledObject(this.gameObject, _bulletTypeNumber);
            Destroy(Instantiate(_bulletImpactPrefab, transform.position, Quaternion.identity), 0.1f);
            ScoreUpdate(500);
        }
    }
    private void ScoreUpdate(int value)
    {
        int score = PlayerPrefs.GetInt("Score");
        score += value;
        PlayerPrefs.SetInt("Score", score);
    }
}
