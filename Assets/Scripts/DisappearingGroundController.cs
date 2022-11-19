using UnityEngine;

public class DisappearingGroundController : MonoBehaviour
{
    private SpriteRenderer _spriteRenderer;
    
    private const string PlayerTag = "Player";

    private float _time;
    private const float DisappearingTime = 2;
    private bool _startedDisappearing;

    private void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (_startedDisappearing)
        {
            _time += Time.deltaTime;
            
            Color spriteColor = _spriteRenderer.color;
            spriteColor.a -= Time.deltaTime / DisappearingTime;
            _spriteRenderer.color = spriteColor;
            
            if (_time >= DisappearingTime) Destroy(gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag(PlayerTag))
        {
            _startedDisappearing = true;
        }
    }
}
