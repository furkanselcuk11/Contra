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
    void Start()
    {
        _bulletType = BulletType.bullet1;
    }
    void Update()
    {
        switch (_bulletType)
        {
            case BulletType.bullet1:
                _bulletTypeNumber = 0;
                _rateOfFire = _bulletObjectPool.Pools[0].objectPrefab.GetComponent<BulletController>().RateOfFire;
                break;
            case BulletType.bullet2:
                _bulletTypeNumber = 1;
                _rateOfFire = _bulletObjectPool.Pools[1].objectPrefab.GetComponent<BulletController>().RateOfFire;
                break;
            case BulletType.laser:
                _bulletTypeNumber = 2;
                _rateOfFire = _bulletObjectPool.Pools[2].objectPrefab.GetComponent<BulletController>().RateOfFire;
                break;
        }
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

        _fireTimer += Time.deltaTime;
        if (Input.GetMouseButton(0) && !GameManager.Instance.IsDeath)
        {
            // RateOfFire süresi aralýðýnda ateþ eder
            if (_fireTimer > _rateOfFire)
                Shoot();
        }
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
    enum BulletType
    {
        bullet1,
        bullet2,
        laser
    }
}
