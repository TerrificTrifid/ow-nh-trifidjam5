using UnityEngine;

namespace TrifidJam5
{
    public class RotateToPlayer : MonoBehaviour
    {
        private Transform _player;

        public void Start()
        {
            _player = Locator.GetPlayerTransform();
        }

        public void Update()
        {
            transform.LookAt(_player, transform.TransformDirection(Vector3.up));
            transform.localEulerAngles = new Vector3(0, transform.localEulerAngles.y, 0);
        }
    }
}