using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bridge : MonoBehaviour
{
    [SerializeField] private GameObject _bridgeExplosionPrefab;
    [SerializeField] private float _explosionTime = 1f;
    private bool _isBridgeExploded = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && !_isBridgeExploded)
        {
            _isBridgeExploded = true;
            StartCoroutine(BridgeExplosion());
        }
    }
    IEnumerator BridgeExplosion()
    {
        foreach (Transform item in transform)
        {
            yield return new WaitForSeconds(_explosionTime);
            GameObject explosion = Instantiate(_bridgeExplosionPrefab, item.position, Quaternion.identity);
            AudioManager.Instance.PlaySoundFX("Explosion");
            item.gameObject.SetActive(false);
            Destroy(explosion, 1f);
        }
    }
}
