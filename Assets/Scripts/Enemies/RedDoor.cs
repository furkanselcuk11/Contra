using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class RedDoor : MonoBehaviour
{
    [Header("Chase Settings")]
    private GameObject _player;
    [SerializeField] private float _chaseFireDistance = 5f;
    private bool _isCanBeShoot = false;
    [Space]
    [SerializeField] private GameObject _enemyExplosionPrefab;
    [SerializeField] private GameObject _explodedDoor;
    [SerializeField] private int _maxHealth = 3;
    private int _health;
    void Start()
    {
        _health = _maxHealth;
        _player = GameObject.FindGameObjectWithTag("Player");
    }
    void Update()
    {
        if (DistanceToPlayer() < _chaseFireDistance)
        {
            _isCanBeShoot = true;
        }
        else
        {
            _isCanBeShoot = false;
        }
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Temas edilen nesne bir mermi mi kontrol edin
        if (other.CompareTag("Bullet") && _isCanBeShoot)
        {
            int damage = other.GetComponent<BulletController>().Damage;
            StartCoroutine(RedDoorHit(damage));
        }
    }
    IEnumerator RedDoorHit(int damage)
    {
        AudioManager.Instance.PlaySoundFX("EnemyHit");
        _health = _health - damage;
        if (_health <= 0)
        {
            Instantiate(_explodedDoor, this.gameObject.transform.position, Quaternion.identity);
            yield return new WaitForSeconds(0.1f);
            RedDoorOpen();
        }
    }
    void RedDoorOpen()
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
}
