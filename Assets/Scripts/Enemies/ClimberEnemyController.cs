using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ClimberEnemyController : MonoBehaviour
{
    [SerializeField] private float _speed = 50f;
    [SerializeField] private float _jumpSpeed = 250f;

    [SerializeField] private Transform _groundCheck;
    [SerializeField] private bool _isGrounded;
    private bool _isJump = true;

    [SerializeField] private GameObject _enemyExplosionPrefab;
    private Rigidbody2D _rigidbody;
    private Animator _animator;
    void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
    }
    private void Update()
    {
        Jump();
    }
    private void FixedUpdate()
    {
        Move();
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
        if (other.CompareTag("Bullet"))
        {
            StartCoroutine(EnemyHit());
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
        _rigidbody.AddForce(Vector2.up * _jumpSpeed * Time.fixedDeltaTime, ForceMode2D.Impulse);
        yield return new WaitForSeconds(0.25f);
        GetComponent<Collider2D>().enabled = false;
        AudioManager.Instance.PlaySoundFX("EnemyDie");
        Destroy(gameObject);
        GameObject bulletImpact = Instantiate(_enemyExplosionPrefab, this.gameObject.transform.position, Quaternion.identity);
        Destroy(bulletImpact, 0.5f);
    }
}
