using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerOneWayPlatform : MonoBehaviour
{
    [SerializeField] private GameObject _currentOneWayPlatform;
    private CapsuleCollider2D _playerCollider;
    [SerializeField] private float _disableCollisionTime = 0.5f;

    private void Start()
    {
        _playerCollider = GetComponent<CapsuleCollider2D>();
    }

    void Update()
    {
        if ((Input.GetKey(KeyCode.S)) && Input.GetKeyDown(KeyCode.Space))
        {
            if (_currentOneWayPlatform != null)
            {
                StartCoroutine(DisableCollision());
            }
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("OneWayPlatform"))
        {
            _currentOneWayPlatform = collision.gameObject;
        }
        //_currentOneWayPlatform.GetComponent<PlatformEffector2D>().rotationalOffset = 0f;
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("OneWayPlatform"))
        {
            //_currentOneWayPlatform.GetComponent<PlatformEffector2D>().rotationalOffset = 180f;
            _currentOneWayPlatform = null;
        }
    }
    IEnumerator DisableCollision()
    {
        BoxCollider2D platformCollider = _currentOneWayPlatform.GetComponent<BoxCollider2D>();

        Physics2D.IgnoreCollision(_playerCollider, platformCollider);
        yield return new WaitForSeconds(_disableCollisionTime);
        Physics2D.IgnoreCollision(_playerCollider, platformCollider, false);
    }
}
