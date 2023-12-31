using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationController : MonoBehaviour
{
    private Animator _animator;
    [SerializeField] private SpriteRenderer _spriteRenderer;
    bool _isFacingRight = true; // Karakterin ba�lang��ta sa�a do�ru bak�p bakmad���n� saklar    

    public bool IsFacingRight { get => _isFacingRight; set => _isFacingRight = value; }

    void Start()
    {
        _animator = GetComponent<Animator>();
    }
    void Update()
    {

    }
    public void Jump()
    {
        _animator.SetTrigger("Jump");
    }
    public void SwimAnimOpen(bool value)
    {
        _animator.SetBool("Swimming", value);
    }
    public void JumpSound()
    {
        AudioManager.Instance.PlaySoundFX("Jump");
    }
    public void Die()
    {
        _animator.SetTrigger("Die");
    }
    public void Move(Vector2 velocity, float horizontalMovement, float verticalMovement)
    {
        // Hareket y�n�ne g�re flipX de�erini ayarla
        if (horizontalMovement > 0 && !_isFacingRight)
        {
            FlipCharacter();
        }
        else if (horizontalMovement < 0 && _isFacingRight)
        {
            FlipCharacter();
        }

        if (Input.GetMouseButton(0))
        {
            // Ate� ediliyor ate� etme animasyonlar� oynas�n
            _animator.SetFloat("HorizontalMovement", horizontalMovement);
            _animator.SetFloat("VerticalMovement", verticalMovement);
            _animator.SetBool("Fire", true);
        }
        else
        {
            // Sadece Hareket animasyonlar� oynas�n
            _animator.SetFloat("HorizontalMovement", horizontalMovement);
            _animator.SetFloat("VerticalMovement", verticalMovement);
            _animator.SetBool("Fire", false);
        }
    }
    public void SwimMove(Vector2 velocity, float horizontalMovement, float verticalMovement)
    {
        // Hareket y�n�ne g�re flipX de�erini ayarla
        if (horizontalMovement > 0 && !_isFacingRight)
        {
            FlipCharacter();
        }
        else if (horizontalMovement < 0 && _isFacingRight)
        {
            FlipCharacter();
        }

        if (Input.GetMouseButton(0))
        {
            // Ate� ediliyor ate� etme animasyonlar� oynas�n
            _animator.SetFloat("HorizontalMovement", horizontalMovement);
            _animator.SetFloat("VerticalMovement", verticalMovement);
            _animator.SetBool("Fire", true);
        }
        else
        {
            // Sadece Hareket animasyonlar� oynas�n
            _animator.SetFloat("HorizontalMovement", horizontalMovement);
            _animator.SetFloat("VerticalMovement", verticalMovement);
            _animator.SetBool("Fire", false);
        }
    }
    public void StopAnimations()
    {
        _animator.SetBool("Fire", false);
        _animator.SetBool("Swimming", false);
    }
    public void ResetAnim()
    {
        _animator.ResetTrigger("Die");
        _animator.SetTrigger("Jump");
    }
    private void FlipCharacter()
    {
        // spriteRenderer.flipX de�erini tersine �evir - Karakterin bakt��� y�n� belirler
        _isFacingRight = !_isFacingRight;
        _spriteRenderer.flipX = !_spriteRenderer.flipX;
        //_spriteRendererTop.flipX = !_spriteRendererTop.flipX;

        //if (!_spriteRendererTop.flipX)
        //{
        //    Vector3 newPosition = _spriteRendererTop.transform.position;
        //    newPosition.x = -0.3f;
        //    _spriteRendererTop.transform.position = newPosition;
        //}
    }
}
