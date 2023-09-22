using FIMSpace.Basics;
using SpiderShooter.Common;
using UnityEngine;

namespace SpiderShooter.Spider
{
    [AddComponentMenu("SpiderShooter/Spider.Movement")]
    public class Movement : FBasic_RigidbodyMovement
    {
        [Tooltip("Just lerp speed for rotating object")]
        public float RotationSpeed = 5f;

        [SerializeField]
        [InspectorReadOnly]
        private bool movingBackward = false;

        [SerializeField]
        private Animator animator;

        private SpiderAnimator spiderAnimator;

        private float turbo = 0f;

        protected override void Start()
        {
            base.Start();

            CharacterRigidbody = GetComponent<Rigidbody>();
            spiderAnimator = new SpiderAnimator(this, animator);
            onlyForward = true;
            diagonalMultiplier = 1f;
        }

        public void ResetForces()
        {
            CharacterRigidbody.velocity = Vector3.zero;
            accelerationForward = 0;
        }

        protected override void Update()
        {
            CheckGroundPlacement();

            // Additional animation stuff
            spiderAnimator.Animate(accelerationForward);

            // Hard coded turbo
            turbo = Input.GetKey(KeyCode.LeftShift)
                ? Mathf.Lerp(turbo, 1f, Time.deltaTime * 10f)
                : Mathf.Lerp(turbo, 0f, Time.deltaTime * 10f);

            // Remembering unchanged direction to apply it back after all rotation calculations
            // to avoid shuttering rotation bug when moving diagonally
            float preTargetDir = targetDirection;
            bool diagonalOverrides = false;

            if (Grounded)
            {
                // Modify target rotation value if we are walking forward and to sides
                if (verticalValue != 0f)
                {
                    if (!movingBackward)
                    {
                        targetDirection += 35f * horizontalValue;
                    }
                    else
                    {
                        targetDirection -= 35f * horizontalValue;
                    }
                }
                else // or going only to sides
                {
                    if (horizontalValue != 0f)
                    {
                        lastTargetVelocityForward = newVelocityForward;
                        accelerationForward = Mathf.Min(
                            1f + turbo,
                            accelerationForward + (AccelerationSpeed * Time.fixedDeltaTime)
                        );
                        targetDirection = inputDirection + (90f * horizontalValue);

                        diagonalOverrides = true;
                    }
                }
            }

            // Smooth change of modified target rotation value
            animatedDirection = Mathf.LerpAngle(
                transform.localRotation.eulerAngles.y,
                targetDirection,
                Time.deltaTime * RotationSpeed
            );

            RotationCalculations();

            if (!diagonalOverrides)
            {
                targetDirection = preTargetDir;
            }
        }

        protected override void FixedUpdate()
        {
            lastTargetVelocityRight = Vector3.zero;
            base.FixedUpdate();
        }

        /// <summary>
        /// Just rotate object to a new value of target rotation
        /// </summary>
        protected override void RotationCalculations()
        {
            transform.rotation = Quaternion.Euler(0f, animatedDirection, 0f);
        }

        protected override void MoveForward(bool backward)
        {
            if (!backward) // Just moving forward
            {
                lastTargetVelocityForward = newVelocityForward;

                accelerationForward = Mathf.Min(
                    1f + turbo,
                    accelerationForward + (AccelerationSpeed * Time.fixedDeltaTime)
                );
            }
            else
            {
                lastTargetVelocityForward = newVelocityForward;

                accelerationForward = onlyForward
                    ? Mathf.Min(
                        1f + turbo,
                        accelerationForward + (AccelerationSpeed * Time.fixedDeltaTime)
                    )
                    : Mathf.Max(
                        -1f + turbo,
                        accelerationForward - (AccelerationSpeed * Time.fixedDeltaTime)
                    );
            }

            movingBackward = backward;

            targetDirection = backward ? inputDirection + 180f : inputDirection;
        }

        protected override void Jump()
        {
            base.Jump();

            // We rolling all the time forward, so we must do some tweaks for sideways
            // keys - when character jumps it must use sideways velocity calculations
            if (horizontalValue != 0f && verticalValue == 0f)
            {
                CharacterRigidbody.velocity += new Vector3(
                    -newVelocityRight.z,
                    0f,
                    newVelocityRight.x
                );
            }
        }

        /// <summary>
        /// Overriding break movement to store velocity in a different way when stopping
        /// </summary>
        protected override void StoppingMovement()
        {
            Vector3 vel = new(0f, 0f, 1f);
            vel = transform.TransformDirection(vel);
            vel *= MaxSpeed;

            CharacterRigidbody.velocity = new Vector3(
                vel.x * accelerationForward,
                CharacterRigidbody.velocity.y,
                vel.z * accelerationForward
            );
            targetVelocity = CharacterRigidbody.velocity;

            if (inputAxes.x == 0f)
            {
                if (accelerationForward > 0f)
                {
                    accelerationForward = Mathf.Max(
                        0f,
                        accelerationForward - (DecelerateSpeed * Time.fixedDeltaTime)
                    );
                }
                else if (accelerationForward < 0f)
                {
                    accelerationForward = Mathf.Min(
                        0f,
                        accelerationForward + (DecelerateSpeed * Time.fixedDeltaTime)
                    );
                }
            }
        }
    }
}
