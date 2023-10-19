using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class Shooter : MonoBehaviour
{
    [SerializeField] private BulletObjectPool _bulletObjectPool = null;
    [SerializeField] private Transform _muzzleRight;
    [SerializeField] private Transform _muzzleLeft;
    [SerializeField] private float _rateOfFire;
    private float _fireTimer;

    [SerializeField] private BulletType _bulletType;
    int _bulletTypeNumber = 0;
    bool _isMultiShoot = false;
    //private bool _isInterlacedShoot = false; // Taramalý atýþ durumu
    //private bool _isFire = false;
    void Start()
    {
        _bulletType = BulletType.bulletNormal;
    }
    void Update()
    {
        switch (_bulletType)
        {
            case BulletType.bulletNormal:
                _bulletTypeNumber = 0;
                _rateOfFire = _bulletObjectPool.Pools[0].objectPrefab.GetComponent<BulletController>().RateOfFire;
                _isMultiShoot = false;
                break;
            case BulletType.bulletRed:
                _bulletTypeNumber = 1;
                _rateOfFire = _bulletObjectPool.Pools[1].objectPrefab.GetComponent<BulletController>().RateOfFire;
                _isMultiShoot = false;
                break;
            case BulletType.bulletLaser:
                _bulletTypeNumber = 2;
                _rateOfFire = _bulletObjectPool.Pools[2].objectPrefab.GetComponent<BulletController>().RateOfFire;
                _isMultiShoot = false;
                break;
            case BulletType.bulletMultiple:
                _bulletTypeNumber = 3;
                _rateOfFire = _bulletObjectPool.Pools[3].objectPrefab.GetComponent<BulletController>().RateOfFire;
                _isMultiShoot = true;
                break;
            case BulletType.bulletDouble:
                _bulletTypeNumber = 4;
                _rateOfFire = _bulletObjectPool.Pools[4].objectPrefab.GetComponent<BulletController>().RateOfFire;
                _isMultiShoot = false;
                break;
        }
        MuzzlePosition();
        Fire();
    }

    private void MuzzlePosition()
    {
        if (GetComponent<AnimationController>().IsFacingRight)
        {
            _muzzleLeft.localPosition = new Vector3(-_muzzleRight.localPosition.x, _muzzleRight.localPosition.y, _muzzleRight.localPosition.z);
            _muzzleLeft.localRotation = Quaternion.Euler(_muzzleRight.localRotation.eulerAngles.x,
                _muzzleRight.localRotation.eulerAngles.y,
                -_muzzleRight.localRotation.eulerAngles.z);
        }
        else
        {
            _muzzleLeft.localPosition = new Vector3(-_muzzleRight.localPosition.x, _muzzleRight.localPosition.y, _muzzleRight.localPosition.z);
            _muzzleLeft.localRotation = Quaternion.Euler(_muzzleRight.localRotation.eulerAngles.x,
                _muzzleRight.localRotation.eulerAngles.y,
                -_muzzleRight.localRotation.eulerAngles.z);
        }
    }

    void Fire()
    {
        _fireTimer += Time.deltaTime;
        if (Input.GetMouseButton(0) && !GameManager.Instance.IsDeath)
        {
            // RateOfFire süresi aralýðýnda ateþ eder
            if (_isMultiShoot)
            {
                if (_fireTimer > _rateOfFire) MultipleShoot();
            }
            else
            {
                if (_fireTimer > _rateOfFire) Shoot();
            }
        }
        // Tekli atýl veya çoklu atýþ için
        //_fireTimer += Time.deltaTime;
        //if (!GameManager.Instance.IsDeath)
        //{
        //    // Fareye basýlý tutma olayýný kontrol et
        //    if (Input.GetMouseButtonDown(0)) // Sol fare düðmesine basýldýðýnda
        //    {
        //        _isInterlacedShoot = true;
        //        _isFire = true;
        //    }

        //    if (Input.GetMouseButtonUp(0)) // Sol fare düðmesine basýlý tutmayý býraktýðýnda
        //    {
        //        _isInterlacedShoot = false;
        //        _isFire = false;
        //        MultipleShoot();
        //    }

        //    // Fareye basýlý tutuluyorsa Shoot fonksiyonunu çalýþtýr
        //    if (_isInterlacedShoot && _isFire)
        //    {
        //        if (_fireTimer > _rateOfFire) Shoot();
        //    }
        //}
        //else
        //{
        //    _isFire = false;
        //}
    }
    private void Shoot()
    {
        // Mermi atýþ sistemi
        GameObject bullet = _bulletObjectPool.GetPooledObject(_bulletTypeNumber);
        AudioManager.Instance.PlaySoundFX("Gun1");
        if (GetComponent<AnimationController>().IsFacingRight)
        {
            bullet.transform.position = _muzzleRight.position;
            bullet.transform.rotation = _muzzleRight.rotation;
            bullet.GetComponent<BulletController>().IsFacingRight = true;
        }
        else
        {
            bullet.transform.position = _muzzleLeft.position;
            bullet.transform.rotation = _muzzleLeft.rotation;
            bullet.GetComponent<BulletController>().IsFacingRight = false;
        }
        _fireTimer = 0f;
    }
    private void MultipleShoot()
    {
        for (int i = -2; i < 3; i++)
        {
            // Mermi objesini oluþtur
            GameObject bullet = _bulletObjectPool.GetPooledObject(_bulletTypeNumber);
            AudioManager.Instance.PlaySoundFX("Gun1");

            // Rastgele bir açý seç
            float angle = i * 15f;

            if (GetComponent<AnimationController>().IsFacingRight)
            {
                bullet.transform.position = _muzzleRight.position;
                bullet.transform.rotation = Quaternion.Euler(0, 0, angle + _muzzleRight.eulerAngles.z);
                bullet.GetComponent<BulletController>().IsFacingRight = true;
            }
            else
            {
                bullet.transform.position = _muzzleLeft.position;
                bullet.transform.rotation = Quaternion.Euler(0, 0, angle + _muzzleLeft.eulerAngles.z);
                bullet.GetComponent<BulletController>().IsFacingRight = false;
            }
            _fireTimer = 0f;
        }
    }
    public void ResetWeapon()
    {
        _bulletType = BulletType.bulletNormal;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Bonus"))
        {
            string bonusName = collision.gameObject.GetComponent<BonusMove>().Name;
            Debug.Log("BONUS");
            switch (bonusName)
            {
                case "M":
                    _bulletType = BulletType.bulletRed;
                    Debug.Log("BulletType.bulletRed");
                    break;
                case "L":
                    _bulletType = BulletType.bulletLaser;
                    Debug.Log("BulletType.bulletLaser");
                    break;
                case "S":
                    _bulletType = BulletType.bulletMultiple;
                    Debug.Log("BulletType.bulletMultiple");
                    break;
                case "F":
                    _bulletType = BulletType.bulletDouble;
                    Debug.Log("BulletType.bulletDouble");
                    break;
                case "R":
                    // Health arttýr
                    break;
            }
            Destroy(collision.gameObject);
        }
    }
    enum BulletType
    {
        bulletNormal,
        bulletRed,  // M
        bulletLaser,// L
        bulletMultiple, // S
        bulletDouble    // F
    }
}
