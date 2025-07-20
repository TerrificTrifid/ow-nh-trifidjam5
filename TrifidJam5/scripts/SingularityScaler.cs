using UnityEngine;

namespace TrifidJam5
{
    public class SingularityScaler : MonoBehaviour
    {
        private Renderer _renderer;
        private float _radius1;
        private float _radius2;
        private float _radius3;
        private float _scale;

        public void Awake()
        {
            _renderer = GetComponent<Renderer>();
            _radius1 = _renderer.material.GetFloat("_Radius");
            _radius2 = _renderer.material.GetFloat("_MaxDistortRadius");
            _radius3 = _renderer.material.GetFloat("_DistortFadeDist");
            _scale = transform.lossyScale.x;
        }

        public void Update()
        {
            _renderer.material.SetFloat("_Radius", _radius1 * transform.lossyScale.x / _scale);
            _renderer.material.SetFloat("_MaxDistortRadius", _radius2 * transform.lossyScale.x / _scale);
            _renderer.material.SetFloat("_DistortFadeDist", _radius3 * transform.lossyScale.x / _scale);
        }
    }
}