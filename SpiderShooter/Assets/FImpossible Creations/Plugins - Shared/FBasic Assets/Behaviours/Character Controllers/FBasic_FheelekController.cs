﻿using UnityEngine;

namespace FIMSpace.Basics
{
    /// <summary>
    /// FM: Derived from FBasic_RigidbodyMovement to override some methods for "Fellek's" animations and behaviour
    /// </summary>
    public class FBasic_FheelekController : FBasic_RigidbodyMovement
    {
        [Tooltip("Just lerping speed for rotating object")]
        public float RotationSpeed = 5f;

        protected bool movingBackward = false;

        // Separated script to controll object's mecanim animator
        protected FBasic_FheelekAnimator fheelekAnimator;

        private float turbo = 0f;

        // References to transforms to animate
        private Transform wheel;
        private Transform fBody;

        protected override void Start()
        {
            base.Start();

            CharacterRigidbody = GetComponent<Rigidbody>();

            wheel = transform.Find("Wheel");
            fBody = transform.Find("Skeleton");

            fheelekAnimator = new FBasic_FheelekAnimator(this);

            onlyForward = true;

            diagonalMultiplier = 1f;
        }

        protected override void Update()
        {
            CheckGroundPlacement();

            wheel.localRotation *= Quaternion.Euler(
                accelerationForward * 480f * Time.deltaTime,
                0f,
                0f
            );

            // Additional animation stuff
            fheelekAnimator.Animate(accelerationForward);

            // Hard coded turbo
            turbo = Input.GetKey(KeyCode.LeftShift)
                ? Mathf.Lerp(turbo, 1f, Time.deltaTime * 10f)
                : Mathf.Lerp(turbo, 0f, Time.deltaTime * 10f);

            #region Horizontal movement modification

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

            #endregion

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
            fBody.rotation = Quaternion.Euler(
                0f,
                Mathf.LerpAngle(
                    fBody.rotation.eulerAngles.y,
                    targetDirection,
                    Time.deltaTime * RotationSpeed * 1.25f
                ),
                0f
            );
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

            // We rolling all the time forward, so we must do some tweaks for sideways keys - when character jumps it must use sideways velocity calculations
            if (horizontalValue != 0f && verticalValue == 0f)
            {
                CharacterRigidbody.velocity += new Vector3(
                    -newVelocityRight.z,
                    0f,
                    newVelocityRight.x
                );
            }

            fheelekAnimator.Jump();
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
