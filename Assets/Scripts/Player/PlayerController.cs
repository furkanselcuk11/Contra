using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private PlayerData _data;
    public PlayerData Data { get { return _data; } }
    private Rigidbody2D _rigidBody;
    [SerializeField] private Transform _groundCheck;
    [SerializeField] private bool _isGrounded;
    void Start()
    {
        _data = GetComponent<PlayerData>();
        _rigidBody = GetComponent<Rigidbody2D>();
    }
    void Update()
    {

    }
    private void FixedUpdate()
    {
        int layerMask = LayerMask.GetMask("Floor");
        _isGrounded = Physics2D.OverlapPoint(_groundCheck.position, layerMask);

        float moveX = Input.GetAxis("Horizontal");

        if (_isGrounded && (Input.GetKey(KeyCode.Space)))
        {
            Debug.Log("JUMP");
            _rigidBody.AddForce(Vector2.up * _data.JumpSpeed * Time.fixedDeltaTime, ForceMode2D.Impulse);
            _isGrounded = false;
        }

        Vector2 newVelocity = new Vector2(moveX * _data.MoveSpeed * Time.fixedDeltaTime, _rigidBody.velocity.y);
        _rigidBody.velocity = newVelocity;
    }
}
