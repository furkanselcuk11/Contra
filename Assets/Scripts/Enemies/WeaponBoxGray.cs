using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponBoxGray : MonoBehaviour
{
    [Header("Rotation Move")]
    [SerializeField] private float _rotationSpeed = 90f; // Dönme hýzý (derece/saniye)
    private float _currentRotation = 180f;
    private float _targetRotation = 180f;
    [SerializeField] private float _rateOfRotate;
    private float _rotateTimer;
    [Header("Chase Settings")]
    private GameObject _player;
    [SerializeField] private float _weaponOpenDistance = 6f;
    [SerializeField] private float _chaseFireDistance = 5f;
    private bool _isFire = false;
    private bool _isCanBeShoot = false;
    [Header("Gun Settings")]
    [SerializeField] private float _rateOfFire;
    private float _fireTimer;
    [SerializeField] private GameObject _weapon; // Silahýn Transform bileþeni   
    [SerializeField] private Transform _muzzleTransform; // Silahýn Transform bileþeni
    [SerializeField] private GameObject _bulletPrefab;
    [Space]
    [SerializeField] private GameObject _enemyExplosionPrefab;
    [SerializeField] private int _maxHealth = 3;
    private int _health;
    private Animator _animator;

    
    private bool _isOpen = false;
    void Start()
    {
        _health = _maxHealth;
        _animator = GetComponent<Animator>();
        _player = GameObject.FindGameObjectWithTag("Player");
    }
    void Update()
    {
        if (DistanceToPlayer() < _weaponOpenDistance)
        {
            _animator.SetTrigger("Open");
            _isOpen = true;
            if (DistanceToPlayer() < _chaseFireDistance)
            {
                _isFire = true;
                _animator.ResetTrigger("Open");
                _weapon.SetActive(true);
                _isCanBeShoot = true;
            }
            else
            {
                _isFire = false;
                _isCanBeShoot = false;
            }
        }
        else
        {
            _weapon.SetActive(false);
            _isOpen = false;
            _isFire = false;
        }

        Rotation();
        Fire();
    }
    public void WeaponActive()
    {
        _weapon.SetActive(true);
    }
    void Rotation()
    {
        if (_isOpen)
        {
            _rotateTimer += Time.deltaTime;
            // RateOfFire süresi aralýðýnda ateþ eder
            if (_rotateTimer > _rateOfRotate)
                WeaponRotation();
        }
    }
    void WeaponRotation()
    {
        // Karakterin pozisyonuna göre silahýn rotasyonunu hesaplar
        Vector2 direction = _player.transform.position - _weapon.transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // Hedef rotasyonu 30 ve katlarýna yakýn olan deðere ayarlar
        _targetRotation = Mathf.Round(angle / 30f) * 30f;

        // Saðdan veya soldan daha yakýn olan rotasyonu bul
        if (_targetRotation - _currentRotation > 180f)
        {
            _targetRotation -= 360f;
        }
        else if (_currentRotation - _targetRotation > 180f)
        {
            _currentRotation -= 360f;
        }

        // Hedef rotasyona doðru döner
        _currentRotation = Mathf.MoveTowards(_currentRotation, _targetRotation, _rotationSpeed * Time.deltaTime);

        // Silahýn rotasyonunu günceller
        _weapon.transform.rotation = Quaternion.Euler(0f, 0f, _currentRotation);
    }
    private void Fire()
    {
        if (_isFire)
        {
            _fireTimer += Time.deltaTime;
            // RateOfFire süresi aralýðýnda ateþ eder
            if (_fireTimer > _rateOfFire)
                Shoot();
        }
    }
    private void Shoot()
    {
        GameObject bullet = Instantiate(_bulletPrefab, _muzzleTransform.transform.position, Quaternion.identity);
        bullet.transform.position = _muzzleTransform.position;
        bullet.transform.rotation = _muzzleTransform.rotation;
        AudioManager.Instance.PlaySoundFX("EnemyBullet");
        _fireTimer = 0f;
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Temas edilen nesne bir mermi mi kontrol edin
        if (other.CompareTag("Bullet") && _isCanBeShoot)
        {
            StartCoroutine(WeaponBoxGrayHit());
        }
    }
    IEnumerator WeaponBoxGrayHit()
    {
        AudioManager.Instance.PlaySoundFX("EnemyHit");
        _health = _health - 1;
        if (_health <= 0)
        {         
            yield return new WaitForSeconds(0.1f);
            WeaponBoxGrayDie();
        }
    }
    void WeaponBoxGrayDie()
    {
        AudioManager.Instance.PlaySoundFX("EnemyDie");
        GameObject bulletImpact = Instantiate(_enemyExplosionPrefab, this.gameObject.transform.position, Quaternion.identity);
        Destroy(bulletImpact, 0.5f);
        Destroy(gameObject);
    }
    private float DistanceToPlayer()
    {
        return Vector3.Distance(transform.position, _player.transform.position);
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, _weaponOpenDistance);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _chaseFireDistance);
    }
}
