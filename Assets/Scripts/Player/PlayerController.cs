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

    }
    private void FixedUpdate()
    {
        int layerMask = LayerMask.GetMask("Floor");
        _isGrounded = Physics2D.OverlapPoint(_groundCheck.position, layerMask);

        _horizontalMovement = Input.GetAxis("Horizontal");
        _verticalMovement = Input.GetAxis("Vertical");
        
        Jump();
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
            // Zýplarken Yatay hareketi azalt
            float reducedHorizontalMovement = _horizontalMovement * 0.7f; // Örneðin yarýya indirmek için 0.5 kullanabilirsiniz
            Vector2 newVelocity = new Vector2(reducedHorizontalMovement * _data.MoveSpeed * Time.fixedDeltaTime, _rigidBody.velocity.y);
            _rigidBody.velocity = newVelocity;
        }

        _animConttoller.Move(_rigidBody.velocity, _horizontalMovement, _verticalMovement);
    }
    void Jump()
    {
        if (_isGrounded && (Input.GetKey(KeyCode.Space)))
        {
            _rigidBody.AddForce(Vector2.up * _data.JumpSpeed * Time.fixedDeltaTime, ForceMode2D.Impulse);
            _isGrounded = false;

            _animConttoller.Jump();
        }
    }
}
