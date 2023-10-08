using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationController : MonoBehaviour
{
    private Animator _animator;
    private SpriteRenderer _spriteRenderer;
    bool _isFacingRight = true; // Karakterin ba�lang��ta sa�a do�ru bak�p bakmad���n� saklar
    void Start()
    {
        _animator = GetComponent<Animator>();
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }
    void Update()
    {

    }
    public void Jump()
    {
        _animator.SetTrigger("Jump");
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

    public void StopAnimations()
    {
        _animator.SetBool("isWalking", false);
        _animator.SetBool("isFalling", false);
    }
    private void FlipCharacter()
    {
        // spriteRenderer.flipX de�erini tersine �evir - Karakterin bakt��� y�n� belirler
        _isFacingRight = !_isFacingRight;
        _spriteRenderer.flipX = !_spriteRenderer.flipX;
    }
}
