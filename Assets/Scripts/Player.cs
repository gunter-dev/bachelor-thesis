using System;
using UnityEngine;

public class Player : MonoBehaviour
{
    private Rigidbody2D _playerBody;
    private Animator _animator;
    private SpriteRenderer _spriteRenderer;

    private float _xMovement;

    public int keysNeeded;

    private bool _movingRight = true;
    private bool _grounded = true;
    private bool _jumped;
    private bool _reversedGravity;
    private bool _gravityLeft;
    private bool _sideGravity;
    private bool _sliding;
    private bool _slowedDown;
    private bool _onAccelerator;

    private void Awake()
    {
        _playerBody = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        PlayerMovement();
        CalculateFlip();
        Animate();
    }

    private void PlayerMovement()
    {
        _xMovement = MovementSpeed();

        transform.position += Time.deltaTime * Constants.MoveForce * new Vector3(_xMovement, 0f, 0f);

        if (_sideGravity)
        {
            var yMovement = Input.GetAxisRaw(Constants.Vertical);
            transform.position += Time.deltaTime * Constants.MoveForce * new Vector3(0f, yMovement, 0f);
        }

        if (Input.GetButtonDown(Constants.Jump) && _grounded) PlayerJump();
        if (Input.GetKeyDown(KeyCode.O))
        {
            _sideGravity = true;
            _gravityLeft = !_gravityLeft;
            Physics2D.gravity = new Vector2(9.8f * (_gravityLeft ? 1 : -1), 0);
        }
    }

    private float MovementSpeed()
    {
        if (_sideGravity) return 0;
        if (_sliding)
            return Math.Abs(Input.GetAxis(Constants.Horizontal)) < Constants.SlidingSpeed
                ? Constants.SlidingSpeed * (_movingRight ? 1 : -1)
                : Input.GetAxis(Constants.Horizontal);

        if (_onAccelerator) return Input.GetAxisRaw(Constants.Horizontal) + Constants.AcceleratorPlusSpeed;

        return Input.GetAxisRaw(Constants.Horizontal) * (_slowedDown ? Constants.SlowedDownSpeed : 1);
    }

    private void PlayerJump()
    {
        _grounded = false;
        _playerBody.AddForce(
            _sideGravity
                ? new Vector2(Constants.JumpForce * (_gravityLeft ? -1 : 1), 0f)
                : new Vector2(0f, Constants.JumpForce * (_reversedGravity ? -1 : 1) * (_slowedDown ? 0.5f : 1)), 
            ForceMode2D.Impulse);
    }
    
    private void CalculateFlip()
    {
        // When _xMovement is smaller than zero, the player is running to the left
        // which means we have to flip the player model (set the flipX to true)
        
        // This if condition makes sure the player model doesn't turn around when 
        // it stops after running to the left
        if (_xMovement == 0) return;
        
        _movingRight = _xMovement > 0;
        _spriteRenderer.flipX = !_movingRight;
    }

    private void Animate()
    {
        if (_grounded) AnimateGrounded();
        else if (!_jumped) AnimateJump();
    }

    private void AnimateGrounded()
    {
        _animator.Play(Input.GetAxisRaw(Constants.Horizontal) == 0 ? Constants.Idle : Constants.Run);
    }

    private void AnimateJump()
    {
        _jumped = true;
        _animator.Play(Constants.Jump);
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        Vector3 contactPoint = col.contacts[0].point;
        Vector3 center = col.collider.bounds.center;

        if (contactPoint.y > center.y && !_reversedGravity || contactPoint.y < center.y && _reversedGravity)
        {
            _grounded = col.gameObject.CompareTag(Constants.GroundTag) || col.gameObject.CompareTag(Constants.IceTag)
                                                             || col.gameObject.CompareTag(Constants.SlimeTag) ||
                                                             col.gameObject.CompareTag(Constants.AcceleratorTag)
                                                             || col.gameObject.CompareTag(Constants.SideTag) ||
                                                             col.gameObject.CompareTag(Constants.BoxTag)
                                                             || col.gameObject.CompareTag(Constants.ButtonTag);
            _jumped = !_grounded;

            if (col.gameObject.CompareTag(Constants.GravityBlockTag)) HandleGravityChange();

            _sliding = col.gameObject.CompareTag(Constants.IceTag);
            _slowedDown = col.gameObject.CompareTag(Constants.SlimeTag);
            _onAccelerator = col.gameObject.CompareTag(Constants.AcceleratorTag);
        }
        
        if (col.gameObject.CompareTag(Constants.SpikeTag) || col.gameObject.CompareTag(Constants.ExitTag)) KillPlayer();
    }

    private void HandleGravityChange()
    {
        _reversedGravity = !_reversedGravity;
        Physics2D.gravity = new Vector2(0, 9.8f * (_reversedGravity ? 1 : -1));
        _spriteRenderer.flipY = !_spriteRenderer.flipY;
    }

    private void KillPlayer()
    {
        Destroy(gameObject);
    }
}
