using Mirror;
using SpiderShooter.Networking;
using UnityEngine;

namespace SpiderShooter.Spider
{
    [AddComponentMenu("SpiderShooter/Spider.Controller")]
    public class Controller : NetworkBehaviour
    {
        [SerializeField]
        private Shooting shooting;

        [SerializeField]
        private SpiderImpl spider;

        [SerializeField]
        private FBasics.FBasic_CharacterMovementBase characterController;

        private void Update()
        {
            if (RoomPlayer.Singleton.GameEnds)
            {
                return;
            }

            if (!Application.isFocused)
            {
                return;
            }

            if (isLocalPlayer && !spider.DeathLock)
            {
                Vector2 inputValue = Vector2.zero;

                if (Input.GetKey(KeyCode.A) || Input.GetKeyDown(KeyCode.A))
                {
                    inputValue.x = -1;
                }
                else if (Input.GetKey(KeyCode.D) || Input.GetKeyDown(KeyCode.D))
                {
                    inputValue.x = 1;
                }

                if (Input.GetKey(KeyCode.W) || Input.GetKeyDown(KeyCode.W))
                {
                    inputValue.y = 1;
                }
                else if (Input.GetKey(KeyCode.S) || Input.GetKeyDown(KeyCode.S))
                {
                    inputValue.y = -1;
                }

                SetInputAxis(inputValue);
                SetInputDirection(Camera.main.transform.eulerAngles.y);
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    Jump();
                }

                if (Input.GetMouseButton(0))
                {
                    shooting.Shoot(spider);
                }

                if (Input.GetKeyDown(KeyCode.Delete))
                {
                    spider.CmdKilled();
                }
            }
        }

        public void SetInputAxis(Vector2 inputAxis)
        {
            characterController.SetInputAxis(inputAxis);
        }

        public void Jump()
        {
            characterController.SetJumpInput();
        }

        public void SetInputDirection(float yDirection)
        {
            characterController.SetInputDirection(yDirection);
        }
    }
}
