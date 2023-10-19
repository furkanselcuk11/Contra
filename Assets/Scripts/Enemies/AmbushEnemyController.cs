using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmbushEnemyController : MonoBehaviour
{
    [Header("Move Settings")]
    [SerializeField] private float _jumpSpeed = 250f;
    [Header("Chase Settings")]
    private GameObject _player;
    [SerializeField] private float _chaseDistance = 5f;
    private bool _isFire = false;
    private bool _isCanBeShoot = false;
    [Header("Gun Settings")]
    [SerializeField] private Transform _muzzleTransform;
    [SerializeField] private GameObject _bulletPrefab;
    [Space]
    [SerializeField] private GameObject _enemyExplosionPrefab;
    private Vector3 _enemyPosition;
    private float _enemyPositionX;
    private bool _isDeath = false;

    private CapsuleCollider2D _collider;
    private Vector2 _originalColliderSize;
    private Vector2 _originalColliderOffset;

    [SerializeField] private BulletObjectPool _bulletObjectPool = null;
    private Rigidbody2D _rigidbody;
    private Animator _animator;
    void Start()
    {
        _bulletObjectPool = FindObjectOfType<BulletObjectPool>();
        _rigidbody = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _player = GameObject.FindGameObjectWithTag("Player");
        _enemyPosition = transform.position;
        _enemyPositionX = _enemyPosition.x;
        _collider = GetComponent<CapsuleCollider2D>();
        _originalColliderSize = _collider.size;
        _originalColliderOffset = _collider.offset;
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
            _animator.SetTrigger("Fire");
            _animator.SetBool("Idle", false);
        }
        else
        {
            _animator.ResetTrigger("Fire");
            _animator.SetBool("Idle", true);
        }
    }
    public void Crouch()
    {
        // Eðiliyor
        _collider.offset = new Vector2(0f, -0.25f);
        _collider.size = new Vector3(_collider.bounds.size.x, 0.45f);
    }
    public void NotCrouch()
    {
        // Ayakta
        _collider.offset = _originalColliderOffset;
        _collider.size = _originalColliderSize;
    }
    public void Shoot()
    {
        //GameObject bullet = Instantiate(_bulletPrefab, _muzzleTransform.transform.position, Quaternion.identity);
        GameObject bullet = _bulletObjectPool.GetPooledObject(5);
        bullet.transform.position = _muzzleTransform.position;
        bullet.transform.rotation = _muzzleTransform.rotation;
        AudioManager.Instance.PlaySoundFX("EnemyBullet");
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
        _animator.ResetTrigger("Fire");
        _animator.SetBool("Idle", false);
        _animator.SetTrigger("Die");
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
