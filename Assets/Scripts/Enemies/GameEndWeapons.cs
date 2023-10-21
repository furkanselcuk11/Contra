using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class GameEndWeapons : MonoBehaviour
{
    [Header("Chase Settings")]
    private GameObject _player;
    [SerializeField] private float _chaseFireDistance = 5f;
    private bool _isCanBeShoot = false;
    [Header("Gun Settings")]
    [SerializeField] private Transform _muzzleTransform;
    [Space]
    [SerializeField] private GameObject _enemyExplosionPrefab;
    [SerializeField] private GameObject _explodedWeapon;
    [SerializeField] private int _maxHealth = 3;
    private int _health;
    private Animator _animator;
    [SerializeField] private BulletObjectPool _bulletObjectPool = null;

    void Start()
    {
        _bulletObjectPool = FindObjectOfType<BulletObjectPool>();
        _health = _maxHealth;
        _animator = GetComponent<Animator>();
        _player = GameObject.FindGameObjectWithTag("Player");
    }
    void Update()
    {
        if (DistanceToPlayer() < _chaseFireDistance)
        {
            _animator.SetTrigger("Open");
            _isCanBeShoot = true;
        }
        else
        {
            _animator.ResetTrigger("Open");
            _isCanBeShoot = false;
        }
    }
    public void Shoot()
    {
        GameObject bullet = _bulletObjectPool.GetPooledObject(6);
        bullet.transform.position = _muzzleTransform.position;
        bullet.transform.rotation = _muzzleTransform.rotation;
        AudioManager.Instance.PlaySoundFX("EnemyBullet");
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Temas edilen nesne bir mermi mi kontrol edin
        if (other.CompareTag("Bullet") && _isCanBeShoot)
        {
            int damage = other.GetComponent<BulletController>().Damage;
            StartCoroutine(GameEndWeaponHit(damage));
        }
    }
    IEnumerator GameEndWeaponHit(int damage)
    {
        AudioManager.Instance.PlaySoundFX("EnemyHit");
        _health = _health - damage;
        if (_health <= 0)
        {
            Vector3 ExplodedWeaponPos = new Vector3(this.gameObject.transform.position.x - 0.1f, this.gameObject.transform.position.y + 0.15f, this.gameObject.transform.position.z);
            Instantiate(_explodedWeapon, ExplodedWeaponPos, Quaternion.identity);
            yield return new WaitForSeconds(0.1f);
            GameEndWeaponDie();
        }
    }
    void GameEndWeaponDie()
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
