using UnityEngine;

namespace BlockScripts
{
    public class DisappearingGroundController : MonoBehaviour
    {
        private SpriteRenderer _spriteRenderer;
        private BoxCollider2D _boxCollider;
    
        private float _time;
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
            
                UpdateAlphaChannel(Time.deltaTime / Constants.DisappearingTime);

                if (_time >= Constants.DisappearingTime)
                {
                    _disappeared = true;
                    _startedDisappearing = false;
                    _time = 0;
                    _spriteRenderer.enabled = false;
                    _boxCollider.size = new Vector2(0, 0);
                }
            } 
            else if (_disappeared)
            {
                _time += Time.deltaTime;

                if (_time >= Constants.ReappearingTime)
                {
                    _disappeared = false;
                    _time = 0;
                    _spriteRenderer.enabled = true;
                    _boxCollider.size = new Vector2(1, 0.3f);
                    UpdateAlphaChannel(-1);
                }
            }
        }

        private void OnCollisionEnter2D(Collision2D col)
        {
            if (col.gameObject.CompareTag(Constants.PlayerTag) || col.gameObject.CompareTag(Constants.BoxTag))
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
