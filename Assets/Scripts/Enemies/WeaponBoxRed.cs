using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponBoxRed : MonoBehaviour
{
    [Header("Rotation Move")]
    [SerializeField] private float _rotationSpeed = 90f; // Dönme hýzý (derece/saniye)
    [SerializeField] private float _rateOfRotate;
    private float _rotateTimer;
    [Header("Chase Settings")]
    private GameObject _player;
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
    [SerializeField] private BulletObjectPool _bulletObjectPool = null;
    void Start()
    {
        _bulletObjectPool = FindObjectOfType<BulletObjectPool>();
        _health = _maxHealth;
        _player = GameObject.FindGameObjectWithTag("Player");
    }
    void Update()
    {
        if (DistanceToPlayer() < _chaseFireDistance)
        {
            _isFire = true;
            _isCanBeShoot = true;
        }
        else
        {
            _isFire = false;
            _isCanBeShoot = false;
        }
        Rotation();
        Fire();
    }
    void Rotation()
    {
        if (_isFire)
        {
            _rotateTimer += Time.deltaTime;
            // RateOfFire süresi aralýðýnda ateþ eder
            if (_rotateTimer > _rateOfRotate)
                WeaponRotation();
        }
    }
    void WeaponRotation()
    {
        Vector3 weaponPosition = transform.position;
        Vector3 playerPosition = _player.transform.position;

        // Yükseklik farkýný hesaplayýn
        float heightDifference = Mathf.Abs(weaponPosition.y - playerPosition.y);

        // Silahýn Z eksenindeki açýyý hesaplayýn
        Vector3 directionToPlayer = playerPosition - weaponPosition;
        float angle = Vector3.Angle(_weapon.transform.forward, directionToPlayer);

        // Yükseklik farkýna göre açýyý ayarlayýn
        if (heightDifference > 0.25f && heightDifference <= 2f)
        {
            angle = 150f;
        }
        else
        {
            angle = 180f;
        }

        // Silahý döndürün
        Quaternion targetRotation = Quaternion.Euler(0, 0, angle);
        _weapon.transform.rotation = Quaternion.Slerp(_weapon.transform.rotation, targetRotation, _rotationSpeed * Time.deltaTime);
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
        //GameObject bullet = Instantiate(_bulletPrefab, _muzzleTransform.transform.position, Quaternion.identity);
        GameObject bullet = _bulletObjectPool.GetPooledObject(5);
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
            int damage = other.GetComponent<BulletController>().Damage;
            StartCoroutine(WeaponBoxRedHit(damage));
        }
    }
    IEnumerator WeaponBoxRedHit(int damage)
    {
        AudioManager.Instance.PlaySoundFX("EnemyHit");
        _health = _health - damage;
        if (_health <= 0)
        {
            yield return new WaitForSeconds(0.1f);
            WeaponBoxRedDie();
        }
    }
    void WeaponBoxRedDie()
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
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _chaseFireDistance);
    }
}
