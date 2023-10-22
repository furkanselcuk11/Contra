using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingBonus : MonoBehaviour
{
    [Header("Chase Settings")]
    private GameObject _player;
    [SerializeField] private float flyingTime = 5f;
    [SerializeField] private float _speed = 5f;
    private float _flyingTimer;
    [Space]
    [SerializeField] private GameObject _bonusBoxExplosionPrefab;
    private Rigidbody2D _rigidbody;
    [SerializeField] private BonusType _bonusType;
    [SerializeField] private GameObject[] _bonuses;
    [SerializeField] private Vector3 _bonusEndPos;
    private bool _bonusOpened = false;
    [SerializeField] private bool _isCanBeShoot = false;  // Vurulabilir
    void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _player = GameObject.FindGameObjectWithTag("Player");
        _isCanBeShoot = true;
        string enumName = this._bonusType.ToString(); // Enum deðerinin adý
        _bonusType = (BonusType)Enum.Parse(typeof(BonusType), enumName);
    }
    void Update()
    {

    }
    private void FixedUpdate()
    {
        if (_isCanBeShoot)
        {
            _flyingTimer += Time.fixedDeltaTime;
            if (flyingTime > _flyingTimer)
            {
                _rigidbody.velocity = transform.right * _speed * Time.fixedDeltaTime;
            }
            else
            {
                flyingTime = 0f;
                _isCanBeShoot = false;
                Destroy(gameObject);
            }
        }
        else
        {
            _rigidbody.velocity = Vector2.zero;
        }
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Temas edilen nesne bir mermi mi kontrol edin
        if (other.CompareTag("Bullet") && !_bonusOpened && _isCanBeShoot)
        {
            StartCoroutine(FlyingBonusHit());
        }
    }
    IEnumerator FlyingBonusHit()
    {
        AudioManager.Instance.PlaySoundFX("EnemyHit");
        _bonusOpened = true;
        BonusOpen();
        yield return new WaitForSeconds(0.1f);
        FlyingBonusDie();
    }
    void FlyingBonusDie()
    {
        AudioManager.Instance.PlaySoundFX("EnemyDie");
        GameObject bulletImpact = Instantiate(_bonusBoxExplosionPrefab, this.gameObject.transform.position, Quaternion.identity);
        Destroy(bulletImpact, 0.5f);
        Destroy(gameObject);
    }
    void BonusOpen()
    {
        GameObject bonusObj = Instantiate(_bonuses[(int)_bonusType], transform.position, Quaternion.identity);
        bonusObj.GetComponent<BonusMove>().EndPos = _bonusEndPos;
        bonusObj.GetComponent<BonusMove>().Move();
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
