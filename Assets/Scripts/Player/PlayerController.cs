using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public PlayerData Data { get { return _data; } }

    private Rigidbody2D _rigidBody;
    [SerializeField] private Transform _groundCheck;
    [SerializeField] private Transform _camera;
    [SerializeField] private Transform _playerXAxisLimitTransform;
    [SerializeField] private float _playerXAxisLimit = 5f;
    private bool _isGrounded;
    private bool _isWater;
    [SerializeField] private Transform _healthSpritesContent;
    [SerializeField] private int _maxHealth = 3;
    private int _health;
    private bool _isCanBeShoot = true;
    bool _isFire = false;

    private float _horizontalMovement;
    private float _verticalMovement;

    private CapsuleCollider2D _collider;
    private Vector2 _originalColliderSize;
    private Vector2 _originalColliderOffset;

    private PlayerData _data;
    private AnimationController _animConttoller;

    public bool IsFire { get => _isFire; set => _isFire = value; }
    public bool IsCanBeShoot { get => _isCanBeShoot; set => _isCanBeShoot = value; }

    void Start()
    {
        _health = _maxHealth;
        _data = GetComponent<PlayerData>();
        _animConttoller = GetComponent<AnimationController>();
        _rigidBody = GetComponent<Rigidbody2D>();
        _collider = GetComponent<CapsuleCollider2D>();
        _originalColliderSize = _collider.size;
        _originalColliderOffset = _collider.offset;
        HealthSpritesUpdate();
    }
    void Update()
    {
        if (!GameManager.Instance.IsDeath && GameManager.Instance.GameStarted) Jump();

    }
    private void FixedUpdate()
    {
        int layerMask = LayerMask.GetMask("Floor");
        _isGrounded = Physics2D.OverlapPoint(_groundCheck.position, layerMask);

        int layerMaskWater = LayerMask.GetMask("Water");
        _isWater = Physics2D.OverlapPoint(_groundCheck.position, layerMaskWater);

        _horizontalMovement = Input.GetAxis("Horizontal");
        _verticalMovement = Input.GetAxis("Vertical");

        Crouch(_verticalMovement);
        if (!GameManager.Instance.IsDeath && GameManager.Instance.GameStarted) Move();
        if (!GameManager.Instance.IsDeath && GameManager.Instance.GameStarted) SwimMove();
    }
    void Move()
    {
        if (_isGrounded && !_isWater)
        {
            _isFire = true;
            Vector2 newVelocity = new Vector2(_horizontalMovement * _data.MoveSpeed * Time.fixedDeltaTime, _rigidBody.velocity.y);
            _rigidBody.velocity = newVelocity;

            if (_horizontalMovement > 0)
            {
                // Karakterin sol sýnýra geçmesini engelle
                Vector3 objePozisyon = _playerXAxisLimitTransform.position;
                // Sadece X pozisyonunu deðiþtir
                objePozisyon.x = _camera.position.x - _playerXAxisLimit;
                // Objeyi güncelle
                _playerXAxisLimitTransform.position = objePozisyon;
            }
        }
        else if (!_isGrounded && !_isWater)
        {
            // Zýplarken Yatay hareketi azalt
            float reducedHorizontalMovement = _horizontalMovement * 0.7f; // Örneðin yarýya indirmek için 0.5 kullanabilirsiniz
            Vector2 newVelocity = new Vector2(reducedHorizontalMovement * _data.MoveSpeed * Time.fixedDeltaTime, _rigidBody.velocity.y);
            _rigidBody.velocity = newVelocity;
        }

        _animConttoller.Move(_rigidBody.velocity, _horizontalMovement, _verticalMovement);
    }
    void SwimMove()
    {
        if (_isWater && !_isGrounded)
        {
            _animConttoller.SwimAnimOpen(true);
            if (_verticalMovement >= 0)
            {
                Vector2 newVelocity = new Vector2(_horizontalMovement * _data.MoveSpeed * Time.fixedDeltaTime, _rigidBody.velocity.y);
                _rigidBody.velocity = newVelocity;
                _isFire = true;
            }
            else
            {
                _rigidBody.velocity = Vector3.zero;
                _isFire = false;
            }

            if (_horizontalMovement > 0)
            {
                // Karakterin sol sýnýra geçmesini engelle
                Vector3 objePozisyon = _playerXAxisLimitTransform.position;
                // Sadece X pozisyonunu deðiþtir
                objePozisyon.x = _camera.position.x - _playerXAxisLimit;
                // Objeyi güncelle
                _playerXAxisLimitTransform.position = objePozisyon;
            }
            _animConttoller.SwimMove(_rigidBody.velocity, _horizontalMovement, _verticalMovement);
        }
        else if (!_isWater)
        {
            _animConttoller.SwimAnimOpen(false);
        }
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
        IsCanBeShoot = false;
        _rigidBody.AddForce(Vector2.up * _data.JumpSpeed / 1.5f * Time.fixedDeltaTime, ForceMode2D.Impulse);
        yield return new WaitForSeconds(0.25f);
        _animConttoller.Die();
        _rigidBody.velocity = Vector3.zero;
        GetComponent<CapsuleCollider2D>().isTrigger = true;
        _rigidBody.gravityScale = 0f;
        AudioManager.Instance.PlaySoundFX("PlayerHit");    // Player vurulma çalacak

        GetComponent<Shooter>().ResetWeapon();

        _health = _health - 1;
        GameManager.Instance.IsDeath = true;
        HealthSpritesUpdate();
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
        yield return new WaitForSeconds(0.2f);
        GetComponent<CapsuleCollider2D>().isTrigger = false;
        _rigidBody.gravityScale = 1f;
        yield return new WaitForSeconds(1.8f);
        transform.position = new Vector3(transform.position.x - 1f, 2f, transform.position.z);
        _animConttoller.ResetAnim();
        yield return new WaitForSeconds(1f);
        GameManager.Instance.IsDeath = false;
        IsCanBeShoot = true;
    }
    void HealthSpritesUpdate()
    {
        for (int i = 0; i < _maxHealth; i++)
        {
            _healthSpritesContent.transform.GetChild(i).GetComponent<Image>().enabled = false;
        }
        for (int i = 0; i < _health; i++)
        {
            _healthSpritesContent.transform.GetChild(i).GetComponent<Image>().enabled = true;
        }
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Temas edilen nesne bir mermi mi kontrol edin
        if (other.CompareTag("EnemyBullet"))
        {
            if (IsCanBeShoot) StartCoroutine(PlayerHit());
        }
        if (other.CompareTag("BorderDie"))
        {
            if (IsCanBeShoot) StartCoroutine(PlayerHit());
        }
        if (other.CompareTag("TriggersFly"))
        {
            int flybonusNumber = int.Parse(other.gameObject.name);
            StageController.Instance.FlyBonusOpen(flybonusNumber);
            other.gameObject.tag = "Untagged";
        }
        if (other.CompareTag("TriggersClimbers"))
        {
            int ClimberNumber = int.Parse(other.gameObject.name);
            StageController.Instance.IsSpawning = true;
            StageController.Instance.Timer = 0f;
            StageController.Instance.SpawnEnemies(ClimberNumber);
            other.gameObject.tag = "Untagged";
        }
        if (other.CompareTag("Finish"))
        {
            GameManager.Instance.FirstScene();
        }
    }
}
