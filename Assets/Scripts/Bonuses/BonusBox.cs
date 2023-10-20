using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BonusBox : MonoBehaviour
{
    [Header("Chase Settings")]
    private GameObject _player;
    [SerializeField] private float _chaseDistance = 5f;
    [Space]
    [SerializeField] private GameObject _bonusBoxExplosionPrefab;
    private Animator _animator;
    [SerializeField] private int _maxHealth = 3;
    private int _health;
    [SerializeField] private BonusType _bonusType;
    [SerializeField] private GameObject[] _bonuses;
    private bool _bonusOpened = false;
    private bool _isCanBeShoot = true;
    void Start()
    {
        _health = _maxHealth;
        _animator = GetComponent<Animator>();
        _player = GameObject.FindGameObjectWithTag("Player");

        string enumName = this._bonusType.ToString(); // Enum deðerinin adý
        _bonusType = (BonusType)Enum.Parse(typeof(BonusType), enumName);
    }
    void Update()
    {
        if (DistanceToPlayer() < _chaseDistance)
        {
            ResetAnim();
            _animator.SetTrigger("Open");
            _isCanBeShoot = true;
        }
        else
        {
            ResetAnim();
            _animator.SetTrigger("Close");
            _isCanBeShoot = false;
        }
    }
    private void ResetAnim()
    {
        _animator.ResetTrigger("Open");
        _animator.ResetTrigger("Close");
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Temas edilen nesne bir mermi mi kontrol edin
        if (other.CompareTag("Bullet") && !_bonusOpened && _isCanBeShoot)
        {
            int damage = other.GetComponent<BulletController>().Damage;
            StartCoroutine(BonusBoxHit(damage));
        }
    }
    IEnumerator BonusBoxHit(int damage)
    {
        AudioManager.Instance.PlaySoundFX("EnemyHit");
        _health = _health - damage;
        if (_health <= 0)
        {
            _bonusOpened = true;
            BonusOpen();
            yield return new WaitForSeconds(0.1f);
            BonusBoxDie();
        }
    }
    void BonusBoxDie()
    {
        AudioManager.Instance.PlaySoundFX("EnemyDie");
        GameObject bulletImpact = Instantiate(_bonusBoxExplosionPrefab, this.gameObject.transform.position, Quaternion.identity);
        Destroy(bulletImpact, 0.5f);
        Destroy(gameObject);
    }
    void BonusOpen()
    {
        GameObject bonusObj = Instantiate(_bonuses[(int)_bonusType], transform.position, Quaternion.identity);
        bonusObj.GetComponent<BonusMove>().Move();
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
    enum BonusType
    {
        F,
        L,
        M,
        R,
        S
    }
}
