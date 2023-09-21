using FIMSpace.Basics;
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
        private FBasic_CharacterMovementBase characterController;

        private void Update()
        {
            if (ServerStorage.Singleton.GameEnds)
            {
                return;
            }

            if (!Application.isFocused)
            {
                return;
            }

            if (isLocalPlayer)
            {
                Vector2 inputValue = Vector2.zero;

                if (Input.GetKey(KeyCode.A))
                {
                    inputValue.x = -1;
                }
                else if (Input.GetKey(KeyCode.D))
                {
                    inputValue.x = 1;
                }

                if (Input.GetKey(KeyCode.W))
                {
                    inputValue.y = 1;
                }
                else if (Input.GetKey(KeyCode.S))
                {
                    inputValue.y = -1;
                }

                SetInputAxis(inputValue);
                SetInputDirection(Camera.main.transform.eulerAngles.y);
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    Jump();
                }

                if (Input.GetMouseButtonDown(0))
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
