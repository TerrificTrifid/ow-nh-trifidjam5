using UnityEngine;

namespace TrifidJam5
{
    public class ScaleLevel : MonoBehaviour
    {
        public int Exponent = 0;

        private ScaleObject[] _objects;
        private float _relativeExponent;
        private bool _fadedOut;

        public void Awake()
        {
            
        }

        public void Start()
        {
            _objects = gameObject.GetComponentsInChildren<ScaleObject>(true);
            foreach (ScaleObject obj in _objects)
            {
                obj.FadeOut();
            }
            _fadedOut = true;
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
        }

        public void ApplyExponent(float exp)
        {
            _relativeExponent = exp - Exponent;
            if ( -_relativeExponent > ScaleExplorer.InnerExponentThreshold && -_relativeExponent < ScaleExplorer.OuterExponentThreshold)
            {
                _fadedOut = false;
                foreach (ScaleObject obj in _objects)
                {
                    obj.ApplyRelativeExponent(_relativeExponent);
                }
            }
            else if (!_fadedOut)
            {
                foreach (ScaleObject obj in _objects)
                {
                    obj.FadeOut();
                }
                _fadedOut = true;
            }
        }

        public void FadeOut()
        {
            if (!_fadedOut)
            {
                foreach (ScaleObject obj in _objects)
                {
                    obj.FadeOut();
                }
                _fadedOut = true;
            }
        }
    }
}