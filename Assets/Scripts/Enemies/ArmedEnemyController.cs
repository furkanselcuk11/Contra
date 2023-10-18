using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class ArmedEnemyController : MonoBehaviour
{
    [Header("Move Settings")]
    [SerializeField] private float _jumpSpeed = 250f;
    [Header("Chase Settings")]
    private GameObject _player;
    [SerializeField] private float _chaseDistance = 5f;
    private bool _isFire = false;
    private bool _isCanBeShoot = false;
    [Header("Gun Settings")]
    [SerializeField] private float _rateOfFire;
    private float _fireTimer;
    [SerializeField] private Transform _muzzleTransform;
    [SerializeField] private GameObject _bulletPrefab;
    [Space]
    [SerializeField] private GameObject _enemyExplosionPrefab;
    private Vector3 _enemyPosition;
    private float _enemyPositionX;
    private bool _isDeath = false;

    private Rigidbody2D _rigidbody;
    private Animator _animator;

    void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _player = GameObject.FindGameObjectWithTag("Player");
        _enemyPosition = transform.position;
        _enemyPositionX = _enemyPosition.x;
    }
    void Update()
    {
        AnimChanged();
        CharacterRotation();

        if (DistanceToPlayer() < _chaseDistance)
        {
            // Eðer player ile arasýndaki mesafe _chaseDistance az ise Player'a bak ve Ateþ et
            _isFire = true;
            _isCanBeShoot = true;
        }
        else
        {
            _isFire = false;
            _isCanBeShoot = false;
            _animator.SetBool("Idle", true);
        }
        Fire();
    }
    private void FixedUpdate()
    {

    }
    private void Fire()
    {
        if (_isFire)
        {
            // Silahýn hedefini oyuncunun pozisyonu olarak ayarla
            Vector3 targetDirection = _player.transform.position - transform.position;
            // Silahý hedefe doðru çevir
            float angle = Mathf.Atan2(targetDirection.y, targetDirection.x) * Mathf.Rad2Deg;
            _muzzleTransform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));

            _fireTimer += Time.deltaTime;
            // RateOfFire süresi aralýðýnda ateþ eder
            if (_fireTimer > _rateOfFire)
                Shoot();
        }
    }
    void CharacterRotation()
    {
        float playerPositionX = _player.transform.position.x;
        if (playerPositionX <= _enemyPositionX)
        {
            transform.rotation = Quaternion.Euler(0f, 180f, 0f);
        }
        else
        {
            transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        }
    }
    void AnimChanged()
    {
        if (_isFire)
        {
            float muzzleRotationZ = _muzzleTransform.rotation.eulerAngles.z;
            float rotationY = transform.rotation.eulerAngles.y;

            ResetAnim();
            if (rotationY == 0f)
            {
                // Saða bakýyor
                if (muzzleRotationZ > 180f)
                {
                    muzzleRotationZ -= 360f;
                }
                if (muzzleRotationZ >= -5f && muzzleRotationZ <= 5f)
                {
                    _animator.SetTrigger("RightFire");
                    _muzzleTransform.localPosition = new Vector3(_muzzleTransform.localPosition.x, 0.35f, _muzzleTransform.localPosition.z);
                }
                else if (muzzleRotationZ >= 20f && muzzleRotationZ <= 70f)
                {
                    _animator.SetTrigger("UpFire");
                    _muzzleTransform.localPosition = new Vector3(_muzzleTransform.localPosition.x, 0.65f, _muzzleTransform.localPosition.z);
                }
                else if (muzzleRotationZ >= -70f && muzzleRotationZ <= -20f)
                {
                    _animator.SetTrigger("DownFire");
                    _muzzleTransform.localPosition = new Vector3(_muzzleTransform.localPosition.x, -0.05f, _muzzleTransform.localPosition.z);
                }
            }
            else
            {
                // Sola bakýyor
                if (muzzleRotationZ > 0f)
                {
                    muzzleRotationZ -= 180;
                }
                if (muzzleRotationZ >= -5f && muzzleRotationZ <= 5f)
                {
                    _animator.SetTrigger("RightFire");
                    _muzzleTransform.localPosition = new Vector3(_muzzleTransform.localPosition.x, 0.35f, _muzzleTransform.localPosition.z);
                }
                else if (muzzleRotationZ >= -70f && muzzleRotationZ <= -20f)
                {
                    _animator.SetTrigger("UpFire");
                    _muzzleTransform.localPosition = new Vector3(_muzzleTransform.localPosition.x, 0.65f, _muzzleTransform.localPosition.z);
                }
                else if (muzzleRotationZ >= 20f && muzzleRotationZ <= 70f)
                {
                    _animator.SetTrigger("DownFire");
                    _muzzleTransform.localPosition = new Vector3(_muzzleTransform.localPosition.x, -0.05f, _muzzleTransform.localPosition.z);
                }
            }
        }
    }
    void ResetAnim()
    {
        _animator.ResetTrigger("RightFire");
        _animator.ResetTrigger("UpFire");
        _animator.ResetTrigger("DownFire");
        _animator.SetBool("Idle", false);
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
        if (other.CompareTag("Bullet") && _isCanBeShoot && !_isDeath)
        {
            StartCoroutine(EnemyHit());
        }
    }
    IEnumerator EnemyHit()
    {
        _isDeath = true;
        _rigidbody.AddForce(Vector2.up * _jumpSpeed * Time.fixedDeltaTime, ForceMode2D.Impulse);
        yield return new WaitForSeconds(0.25f);
        GetComponent<Collider2D>().enabled = false;
        AudioManager.Instance.PlaySoundFX("EnemyDie");
        Destroy(gameObject);
        GameObject bulletImpact = Instantiate(_enemyExplosionPrefab, this.gameObject.transform.position, Quaternion.identity);
        Destroy(bulletImpact, 0.5f);
    }
    private float DistanceToPlayer()
    {
        return Vector3.Distance(transform.position, _player.transform.position);
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _chaseDistance);
    }
}
