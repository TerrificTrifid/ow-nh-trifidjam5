using UnityEngine;

namespace TrifidJam5
{
    public class ScaleObject : MonoBehaviour
    {
        public static float FadeSpeed = 5f;

        private Vector2 _direction;
        private float _radius;
        private bool _fade;
        private float _fadeT;
        private float _localRadius;
        private float _localScale;

        public void Awake()
        {
            Vector2 position = new Vector2(transform.localPosition.x, transform.localPosition.z);
            _direction = position.normalized;
            _radius = position.magnitude;
        }

        public void Start()
        {
            _fade = true;
            _fadeT = 0;
            gameObject.SetActive(false);
        }

        public void Update()
        {
            if (!_fade && _fadeT < 1)
            {
                _fadeT += Time.deltaTime * FadeSpeed;
                if (_fadeT >= 1)
                {
                    _fadeT = 1;
                }
            }
            else if (_fade && _fadeT > 0)
            {
                _fadeT -= Time.deltaTime * FadeSpeed;
                if (_fadeT <= 0)
                {
                    _fadeT = 0;
                    gameObject.SetActive(false);
                }
            }

            transform.localPosition = new Vector3(_direction.x * _localRadius, 0, _direction.y * _localRadius);
            transform.localScale = Vector3.one * _localScale * _fadeT;
        }

        public void ApplyRelativeExponent(float exp)
        {
            float s = Mathf.Pow(10, -exp);
            float r = _radius * s;
            if (r < ScaleExplorer.InnerScaleThreshold)
            {
                _localRadius = ScaleExplorer.InnerScaleThreshold;
                _localScale = ScaleExplorer.InnerScaleThreshold / _radius;
                FadeOut();
            }
            else if (r > ScaleExplorer.OuterScaleThreshold)
            {
                _localRadius = ScaleExplorer.OuterScaleThreshold;
                _localScale = ScaleExplorer.OuterScaleThreshold / _radius;
                FadeOut();
            }
            else
            {
                _localRadius = r;
                _localScale = s;
                FadeIn();
            }
        }

        public void FadeIn()
        {
            gameObject.SetActive(true);
            _fade = false;
            transform.localPosition = new Vector3(_direction.x * _localRadius, 0, _direction.y * _localRadius);
            transform.localScale = Vector3.one * _localScale * _fadeT;
        }

        public void FadeOut()
        {
            _fade = true;
        }
    }
}