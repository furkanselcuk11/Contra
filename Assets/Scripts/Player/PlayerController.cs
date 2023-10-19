using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public PlayerData Data { get { return _data; } }
    private Rigidbody2D _rigidBody;
    [SerializeField] private Transform _groundCheck;
    private bool _isGrounded;
    [SerializeField] private int _maxHealth = 3;
    private int _health;
    private bool _isCanBeShoot = true;

    private float _horizontalMovement;
    private float _verticalMovement;

    private CapsuleCollider2D _collider;
    private Vector2 _originalColliderSize;
    private Vector2 _originalColliderOffset;

    private PlayerData _data;
    private AnimationController _animConttoller;
    void Start()
    {
        _health = _maxHealth;
        _data = GetComponent<PlayerData>();
        _animConttoller = GetComponent<AnimationController>();
        _rigidBody = GetComponent<Rigidbody2D>();
        _collider = GetComponent<CapsuleCollider2D>();
        _originalColliderSize = _collider.size;
        _originalColliderOffset = _collider.offset;
    }
    void Update()
    {
        if (!GameManager.Instance.IsDeath && GameManager.Instance.GameStarted) Jump();
    }
    private void FixedUpdate()
    {
        int layerMask = LayerMask.GetMask("Floor");
        _isGrounded = Physics2D.OverlapPoint(_groundCheck.position, layerMask);

        _horizontalMovement = Input.GetAxis("Horizontal");
        _verticalMovement = Input.GetAxis("Vertical");

        Crouch(_verticalMovement);
        if (!GameManager.Instance.IsDeath && GameManager.Instance.GameStarted) Move();
    }
    void Move()
    {
        if (_isGrounded)
        {
            Vector2 newVelocity = new Vector2(_horizontalMovement * _data.MoveSpeed * Time.fixedDeltaTime, _rigidBody.velocity.y);
            _rigidBody.velocity = newVelocity;
        }
        else
        {
            // Zýplarken Yatay hareketi azalt
            float reducedHorizontalMovement = _horizontalMovement * 0.7f; // Örneðin yarýya indirmek için 0.5 kullanabilirsiniz
            Vector2 newVelocity = new Vector2(reducedHorizontalMovement * _data.MoveSpeed * Time.fixedDeltaTime, _rigidBody.velocity.y);
            _rigidBody.velocity = newVelocity;
        }

        _animConttoller.Move(_rigidBody.velocity, _horizontalMovement, _verticalMovement);
    }
    void Jump()
    {
        if ((_isGrounded && (Input.GetKeyDown(KeyCode.Space))) && (!Input.GetKey(KeyCode.S)))
        {
            _rigidBody.AddForce(Vector2.up * _data.JumpSpeed * Time.fixedDeltaTime, ForceMode2D.Impulse);
            _isGrounded = false;

            _animConttoller.Jump();
        }
    }
    void Crouch(float vercital)
    {
        if (vercital < -0.99f)
        {
            // Eðiliyor
            _collider.offset = new Vector2(0f, -0.25f);
            _collider.size = new Vector3(_collider.bounds.size.x, 0.45f);
        }
        else
        {
            // Ayakta
            _collider.offset = _originalColliderOffset;
            _collider.size = _originalColliderSize;
        }
    }
    public IEnumerator PlayerHit()
    {
        _isCanBeShoot = false;
        _rigidBody.AddForce(Vector2.up * _data.JumpSpeed / 1.5f * Time.fixedDeltaTime, ForceMode2D.Impulse);
        yield return new WaitForSeconds(0.25f);
        _animConttoller.Die();
        AudioManager.Instance.PlaySoundFX("PlayerHit");    // Player vurulma çalacak

        GetComponent<Shooter>().ResetWeapon();

        _health = _health - 1;
        GameManager.Instance.IsDeath = true;
        if (_health <= 0)
        {
            Debug.Log("Player is Dead");
            yield return new WaitForSeconds(2f);
            GameManager.Instance.GameOver();
        }
        StartCoroutine(PlayerRestart());
    }
    IEnumerator PlayerRestart()
    {
        yield return new WaitForSeconds(2f);
        transform.position = new Vector3(transform.position.x - 1f, 2f, transform.position.z);
        _animConttoller.ResetAnim();
        GameManager.Instance.IsDeath = false;
        yield return new WaitForSeconds(1f);
        _isCanBeShoot = true;
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Temas edilen nesne bir mermi mi kontrol edin
        if (other.CompareTag("EnemyBullet"))
        {
            if (_isCanBeShoot) StartCoroutine(PlayerHit());
        }
    }
}
