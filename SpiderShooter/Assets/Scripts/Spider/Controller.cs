using Assets.Scripts.Spider;
using UnityEngine;

namespace Spider
{
    [AddComponentMenu("Spider.Controller")]
    public class Controller : MonoBehaviour
    {
        [SerializeField]
        private Movement movement;

        [SerializeField]
        private Shooting shooting;

        [SerializeField]
        private SpiderImpl spider;

        private void Update()
        {
            if (Input.GetKey(KeyCode.D))
            {
                movement.RotateRight();
            }

            if (Input.GetKey(KeyCode.A))
            {
                movement.RotateLeft();
            }

            if (Input.GetKey(KeyCode.W))
            {
                movement.MoveForward();
            }

            if (Input.GetKey(KeyCode.S))
            {
                movement.MoveBackward();
            }

            if (Input.GetMouseButtonDown(0))
            {
                shooting.Shoot(spider);
            }
        }
    }
}
