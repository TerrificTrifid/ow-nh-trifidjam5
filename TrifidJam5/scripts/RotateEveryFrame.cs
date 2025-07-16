using UnityEngine;

namespace TrifidJam5
{
    public class RotateEveryFrame : MonoBehaviour
    {
        public void Update()
        {
            transform.localRotation = Random.rotation;
        }
    }
}