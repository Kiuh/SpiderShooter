using Mirror;
using UnityEngine;

namespace SpiderShooter.Spider
{
    [AddComponentMenu("SpiderShooter/Spider.Controller")]
    public class Controller : NetworkBehaviour
    {
        [SerializeField]
        private Movement movement;

        [SerializeField]
        private Shooting shooting;

        [SerializeField]
        private SpiderImpl spider;

        private void FixedUpdate()
        {
            if (!Application.isFocused)
            {
                return;
            }

            if (isLocalPlayer)
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

                if (Input.GetKeyDown(KeyCode.Space))
                {
                    movement.Jump();
                }

                if (Input.GetKeyDown(KeyCode.Delete))
                {
                    spider.CmdKilled();
                }
            }
        }
    }
}
