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
    [Header("Gun Settings")]
    [SerializeField] private Transform _muzzleTransform;
    [SerializeField] private GameObject _bulletPrefab;
    [Space]
    [SerializeField] private GameObject _enemyExplosionPrefab;
    private Vector3 _enemyPosition;
    private float _enemyPositionX;

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
        }
        else
        {
            _isFire = false;            
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
    public void Shoot()
    {
        GameObject bullet = Instantiate(_bulletPrefab, _muzzleTransform.transform.position, Quaternion.identity);
        bullet.transform.position = _muzzleTransform.position;
        bullet.transform.rotation = _muzzleTransform.rotation;
        AudioManager.Instance.PlaySoundFX("Gun1");
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
