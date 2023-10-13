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

    private float _horizontalMovement;
    private float _verticalMovement;

    private PlayerData _data;
    private AnimationController _animConttoller;
    void Start()
    {
        _data = GetComponent<PlayerData>();
        _animConttoller = GetComponent<AnimationController>();
        _rigidBody = GetComponent<Rigidbody2D>();
    }
    void Update()
    {
        Jump();
    }
    private void FixedUpdate()
    {
        int layerMask = LayerMask.GetMask("Floor");
        _isGrounded = Physics2D.OverlapPoint(_groundCheck.position, layerMask);

        _horizontalMovement = Input.GetAxis("Horizontal");
        _verticalMovement = Input.GetAxis("Vertical");


        Move();
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
            // Z�plarken Yatay hareketi azalt
            float reducedHorizontalMovement = _horizontalMovement * 0.7f; // �rne�in yar�ya indirmek i�in 0.5 kullanabilirsiniz
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
    IEnumerator PlayerHit()
    {
        Debug.Log("Player Hit");
        _rigidBody.AddForce(Vector2.up * _data.JumpSpeed / 1.5f * Time.fixedDeltaTime, ForceMode2D.Impulse);
        yield return new WaitForSeconds(0.25f);
        _animConttoller.Die();
        AudioManager.Instance.PlaySoundFX("PlayerHit");    // Player vurulma �alacak
        // Player can� 1 azalacak
        yield return new WaitForSeconds(2f);
        PlayerRestart();
    }
    void PlayerRestart()
    {
        transform.position = new Vector3(transform.position.x - 1f, 2f, transform.position.z);
        _animConttoller.ResetAnim();
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Temas edilen nesne bir mermi mi kontrol edin
        if (other.CompareTag("EnemyBullet"))
        {
            //StartCoroutine(PlayerHit());
        }
    }
}
