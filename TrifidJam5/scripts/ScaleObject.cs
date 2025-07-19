using UnityEngine;

namespace TrifidJam5
{
    public class ScaleObject : MonoBehaviour
    {
        public Renderer Singularity;
        private float _singularityRadius1;
        private float _singularityRadius2;
        private float _singularityRadius3;

        public void Awake()
        {
            if (Singularity != null)
            {
                _singularityRadius1 = Singularity.material.GetFloat("_Radius");
                _singularityRadius2 = Singularity.material.GetFloat("_MaxDistortRadius");
                _singularityRadius3 = Singularity.material.GetFloat("_DistortFadeDist");
            }
        }

        public void Update()
        {
            if (Singularity != null)
            {
                Singularity.material.SetFloat("_Radius", _singularityRadius1 * transform.localScale.x);
                Singularity.material.SetFloat("_MaxDistortRadius", _singularityRadius2 * transform.localScale.x);
                Singularity.material.SetFloat("_DistortFadeDist", _singularityRadius3 * transform.localScale.x);
            }
        }
    }
}