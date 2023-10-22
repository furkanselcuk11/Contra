using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ClimberEnemyController : MonoBehaviour
{
    [SerializeField] private float _speed = 50f;
    [SerializeField] private float _jumpSpeed = 250f;
    [Header("Chase Settings")]
    private GameObject _player;
    [SerializeField] private float _chaseDistance = 5f;
    private bool _isCanBeShoot = false;
    [SerializeField] private Transform _groundCheck;
    [SerializeField] private bool _isGrounded;
    private bool _isJump = true;
    private bool _isDeath = false;

    [SerializeField] private GameObject _enemyExplosionPrefab;
    private Rigidbody2D _rigidbody;
    private Animator _animator;
    void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
        _rigidbody = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
    }
    private void Update()
    {
        if (_player != null)
        {
            if (DistanceToPlayer() < _chaseDistance)
            {
                _isCanBeShoot = true;
            }
            else
            {
                _isCanBeShoot = false;
            }
        }
        if (_isCanBeShoot) Jump();
    }
    private void FixedUpdate()
    {
        if (_isCanBeShoot) Move();
        int layerMask = LayerMask.GetMask("Floor");
        _isGrounded = Physics2D.OverlapPoint(_groundCheck.position, layerMask);
    }
    private void Move()
    {
        if (_isGrounded)
        {
            Vector2 newVelocity = new Vector2(transform.right.x * _speed * Time.fixedDeltaTime, _rigidbody.velocity.y);
            _rigidbody.velocity = newVelocity;
        }
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Temas edilen nesne bir mermi mi kontrol edin
        if (other.CompareTag("Bullet") && _isCanBeShoot && !_isDeath)
        {
            StartCoroutine(EnemyHit());
        }
        if (other.CompareTag("PlayerXAxisLimit") && !_isDeath)
        {
            StartCoroutine(EnemyHit());
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && _isCanBeShoot && !_isDeath)
        {
            PlayerController player = collision.gameObject.GetComponent<PlayerController>();
            if (player != null && player.IsCanBeShoot)
            {
                StartCoroutine(EnemyHit());
                StartCoroutine(player.PlayerHit());
            }
        }
    }
    private void Jump()
    {
        if ((!_isGrounded) && _isJump)
        {
            Vector2 jumpForce = new Vector2(transform.right.x * _jumpSpeed / 100f, _jumpSpeed / 100f); // X ve Y bileþenlerini ayný hýzda ayarlayarak ileri yönde bir zýplama kuvveti oluþturur
            _rigidbody.AddForce(jumpForce, ForceMode2D.Impulse); // Kuvveti uygular

            _isGrounded = false;
            _isJump = false;

            _animator.SetTrigger("Jump");
        }
    }
    public void JumpTrue()
    {
        _isJump = true;
    }
    IEnumerator EnemyHit()
    {
        _isDeath = true;
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
