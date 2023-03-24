using UnityEngine;

namespace BlockScripts
{
    public class DisappearingGroundController : MonoBehaviour
    {
        private SpriteRenderer _spriteRenderer;
        private BoxCollider2D _boxCollider;
    
        private float _time;
        private const float DisappearingTime = 2;
        private const float ReappearingTime = 5;
        private bool _startedDisappearing;
        private bool _disappeared;

        private void Start()
        {
            _spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
            _boxCollider = GetComponent<BoxCollider2D>();
        }

        private void Update()
        {
            if (_startedDisappearing)
            {
                _time += Time.deltaTime;
            
                UpdateAlphaChannel(Time.deltaTime / DisappearingTime);

                if (_time >= DisappearingTime)
                {
                    _disappeared = true;
                    _startedDisappearing = false;
                    _time = 0;
                    gameObject.SetActive(false);
                    _boxCollider.size = new Vector2(0, 0);
                }
            } 
            else if (_disappeared)
            {
                _time += Time.deltaTime;

                if (_time >= ReappearingTime)
                {
                    _disappeared = false;
                    _time = 0;
                    gameObject.SetActive(true);
                    _boxCollider.size = new Vector2(1, 1);
                    UpdateAlphaChannel(-1);
                }
            }
        }

        private void OnCollisionEnter2D(Collision2D col)
        {
            if (col.gameObject.CompareTag(Constants.PlayerTag))
                _startedDisappearing = true;
        }

        private void UpdateAlphaChannel(float toDeduct)
        {
            Color spriteColor = _spriteRenderer.color;
            spriteColor.a -= toDeduct;
            _spriteRenderer.color = spriteColor;
        }
    }
}
