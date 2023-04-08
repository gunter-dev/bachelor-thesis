using MenuScripts;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Player : MonoBehaviour
{
    private Rigidbody2D _playerBody;
    private Animator _animator;
    private SpriteRenderer _spriteRenderer;
    private BoxCollider2D _collider;
    private Light2D _light;
    private AudioSource _audioSource;

    private float _xMovement;
    private float _slowedDownSpeedMultiplier;
    private float _highSpeedMultiplier = Constants.InitialMultiplier;
    private float _highJumpMultiplier = Constants.InitialMultiplier;
    private float _nightVisionSpeedMultiplier = Constants.InitialMultiplier;

    private bool _grounded;
    private bool _jumpAnimated;
    private bool _reversedGravity;
    private bool _sliding;
    private bool _isUnbreakable;

    private AudioClip _stepSound;
    private AudioClip _jumpSound;
    private AudioClip _landSound;

    private void Awake()
    {
        _playerBody = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _collider = transform.GetComponent<BoxCollider2D>();
        _light = GetComponent<Light2D>();
        _audioSource = GetComponent<AudioSource>();
        
        InitializeSounds();
    }

    private void InitializeSounds()
    {
        _stepSound = Resources.Load<AudioClip>("Sounds/Step");
        _jumpSound = Resources.Load<AudioClip>("Sounds/Jump");
        _landSound = Resources.Load<AudioClip>("Sounds/Land");
    }

    // Update is called once per frame
    void Update()
    {
        if (LobbyMenus.isPaused) return;
        SwitchAbility();
        PlayerMovement();
        CalculateFlip();
        Animate();
    }

    private void SwitchAbility()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SwitchToDefaultAbilities();
            Debug.Log("Default");
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            // switch to high speed
            SwitchToDefaultAbilities();
            _highSpeedMultiplier = Constants.HighSpeedMultiplier;
            Debug.Log("High speed");
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            // switch to high jump
            SwitchToDefaultAbilities();
            _highJumpMultiplier = Constants.HighJumpMultiplier;
            Debug.Log("High jump");
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            // switch to unbreakable
            SwitchToDefaultAbilities();
            _isUnbreakable = true;
            _nightVisionSpeedMultiplier = Constants.NightVisionSpeedMultiplier;
            Debug.Log("Unbreakable");
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            // switch to night vision
            SwitchToDefaultAbilities();
            _light.pointLightInnerRadius = Constants.NightVisionLightRadius;
            _light.pointLightOuterRadius = Constants.NightVisionLightRadius;
            _light.color = Color.yellow;
            
            _nightVisionSpeedMultiplier = Constants.NightVisionSpeedMultiplier;
            Debug.Log("Night vision");
        }
    }

    private void SwitchToDefaultAbilities()
    {
        _highSpeedMultiplier = Constants.InitialMultiplier;
        _highJumpMultiplier = Constants.InitialMultiplier;
        
        _light.pointLightInnerRadius = Constants.DefaultInnerLightRadius;
        _light.pointLightOuterRadius = Constants.DefaultOuterLightRadius;
        _light.color = Color.white;
        
        _nightVisionSpeedMultiplier = Constants.InitialMultiplier;

        _isUnbreakable = false;
    }

    private void PlayerMovement()
    {
        _grounded = IsGrounded();
        if (_grounded && _jumpAnimated)
        {
            _jumpAnimated = false;
            PlaySound(_landSound);
        }
        if (!_grounded) CheckPlayerIsOnMap();

        _xMovement = MovementSpeed();
        
        if (_sliding && _playerBody.velocity.x < Constants.MovementSpeed)
            _playerBody.AddForce(new Vector2(_xMovement * 2f, 0f));
        else
            transform.position += Time.deltaTime * Constants.MovementSpeed * new Vector3(_xMovement, 0f, 0f);

        if (Input.GetButtonDown(Constants.Jump) && _grounded) PlayerJump();
    }

    private void CheckPlayerIsOnMap()
    {
        if (transform.position.y < Constants.MapStartingCoordinate || transform.position.y > GlobalVariables.mapHeight) 
            KillPlayer();
    }

    private float MovementSpeed()
    {
        float axis = _grounded ? Input.GetAxisRaw(Constants.Horizontal) : Input.GetAxis(Constants.Horizontal);
        return axis * _slowedDownSpeedMultiplier * _highSpeedMultiplier * _nightVisionSpeedMultiplier;
    }

    private void PlayerJump()
    {
        float y = Constants.JumpForce * (_reversedGravity ? -1 : 1) * _slowedDownSpeedMultiplier * _highJumpMultiplier;
        _playerBody.AddForce(new Vector2(0f, y), ForceMode2D.Impulse);
        PlaySound(_jumpSound);
    }
    
    private void CalculateFlip()
    {
        // When _xMovement is smaller than zero, the player is running to the left
        // which means we have to flip the player model (set the flipX to true)
        
        // This if condition makes sure the player model doesn't turn around when 
        // it stops after running to the left
        if (_xMovement == 0) return;
        
        bool movingLeft = _xMovement < 0;
        _spriteRenderer.flipX = movingLeft;
    }

    private void Animate()
    {
        if (_grounded) AnimateGrounded();
        else if (!_jumpAnimated) AnimateJump();
    }

    private void AnimateGrounded()
    {
        _animator.Play(Input.GetAxisRaw(Constants.Horizontal) == 0 ? Constants.Idle : Constants.Run);
    }

    private void AnimateJump()
    {
        _jumpAnimated = true;
        _animator.Play(Constants.Jump);
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        Vector3 contactPoint = col.contacts[0].point;
        Vector3 center = col.collider.bounds.center;

        if (contactPoint.y > center.y && !_reversedGravity || contactPoint.y < center.y && _reversedGravity)
        {
            if (col.gameObject.CompareTag(Constants.GravityBlockTag)) HandleGravityChange();

            _sliding = col.gameObject.CompareTag(Constants.IceTag);
            _slowedDownSpeedMultiplier = col.gameObject.CompareTag(Constants.SlimeTag) ? Constants.SlowedDownSpeed : Constants.InitialMultiplier;
        }

        if (col.gameObject.CompareTag(Constants.SpikeTag))
        {
            if (_isUnbreakable) Destroy(col.gameObject);
            else KillPlayer();
        }
        else if (col.gameObject.CompareTag(Constants.ExitTag))
        {
            LobbyMenus.playerWon = true;
            Destroy(gameObject);
        }
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
        LobbyMenus.isPlayerDead = true;
    }

    private bool IsGrounded()
    {
        LayerMask layer = LayerMask.GetMask(Constants.GroundTag);
        Bounds bounds = _collider.bounds;
        Vector2 direction = _reversedGravity ? Vector2.up : Vector2.down;
        return Physics2D.BoxCast(bounds.center, bounds.size, 0f, direction, 0.1f, layer);
    }

    // ReSharper disable once UnusedMember.Local
    private void Step()
    {
        // This function is called from animation - Animation Event
        _audioSource.clip = _stepSound;
        _audioSource.Play();
    }

    private void PlaySound(AudioClip sound)
    {
        _audioSource.clip = sound;
        _audioSource.Play();
    }
}
