using FIMSpace;
using System;
using UnityEngine;

namespace SpiderShooter.Spider
{
    [Serializable]
    public class SpiderAnimator
    {
        public bool AnimationHolder = false;

        private bool lockDeath = false;

        private Movement controller;
        private Animator animator;

        private string lastAnimation = "";

        private bool waitForIdle = false;

        private string idleAnimationState = "Idle";
        private string runAnimationState = "Run";
        private int locomotionLayer = 0;

        public SpiderAnimator(Movement controller, Animator animator)
        {
            this.controller = controller;
            this.animator = animator;
        }

        public void PlayDeath()
        {
            lockDeath = true;
            CrossfadeTo("Death");
        }

        public void ContinueRegular()
        {
            lockDeath = false;
        }

        public void Animate(float acceleration)
        {
            if (lockDeath)
            {
                return;
            }

            if (AnimationHolder)
            {
                if (waitForIdle)
                {
                    AnimatorStateInfo nextInfo = animator.GetNextAnimatorStateInfo(locomotionLayer);
                    if (!nextInfo.IsName(lastAnimation))
                    {
                        if (animator.IsInTransition(locomotionLayer))
                        {
                            waitForIdle = false;
                        }
                        else
                        {
                            AnimatorStateInfo info = animator.GetCurrentAnimatorStateInfo(
                                locomotionLayer
                            );
                            if (!info.IsName(lastAnimation))
                            {
                                waitForIdle = false;
                            }
                        }
                    }

                    if (!waitForIdle)
                    {
                        AnimationHolder = false;
                    }

                    return;
                }
            }

            if (AnimationHolder)
            {
                if (!animator.IsInTransition(locomotionLayer))
                {
                    AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(
                        locomotionLayer
                    );

                    if (stateInfo.normalizedTime > 0.7f)
                    {
                        AnimationHolder = false;
                    }
                }
            }

            if (!AnimationHolder)
            {
                if (controller.Grounded)
                {
                    if (acceleration is < (-0.1f) or > 0.1f)
                    {
                        CrossfadeTo(runAnimationState, 0.25f, locomotionLayer);
                        LerpValue("AnimationSpeed", acceleration / controller.MaxSpeed * 8f);
                    }
                    else
                    {
                        CrossfadeTo(idleAnimationState, 0.25f, locomotionLayer);
                    }
                }
                else
                {
                    if (controller.CharacterRigidbody.velocity.y > 0f)
                    {
                        CrossfadeTo("Jump", 0.15f, locomotionLayer);
                    }
                    else
                    {
                        CrossfadeTo("Falling", 0.24f, locomotionLayer);
                    }
                }
            }
        }

        protected void CrossfadeTo(string animation, float time = 0.25f, int animationLayer = 0)
        {
            if (lastAnimation != animation)
            {
                animator.CrossFadeInFixedTime(animation, time, animationLayer);
                lastAnimation = animation;
            }
        }

        private void LerpValue(string parameter, float value)
        {
            FAnimatorMethods.LerpFloatValue(animator, parameter, value, 5f);
        }
    }
}
