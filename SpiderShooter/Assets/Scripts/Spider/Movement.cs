using UnityEngine;

namespace SpiderShooter.Spider
{
    [AddComponentMenu("SpiderShooter/Spider.Movement")]
    public class Movement : MonoBehaviour
    {
        [SerializeField]
        private Rigidbody rigidBody;

        [Header("Linear Movement")]
        [SerializeField]
        private float linearSpeed;

        [SerializeField]
        private float maxLinearSpeed;

        [Header("Rotation Movement")]
        [SerializeField]
        private float angularSpeed;

        [SerializeField]
        private float maxAngularSpeed;

        [Header("Jump Movement")]
        [SerializeField]
        private float jumpForce;

        private void Awake()
        {
            rigidBody.maxLinearVelocity = maxLinearSpeed;
            rigidBody.maxAngularVelocity = maxAngularSpeed;
        }

        private void OnValidate()
        {
            rigidBody.maxLinearVelocity = maxLinearSpeed;
            rigidBody.maxAngularVelocity = maxAngularSpeed;
        }

        public void MoveForward()
        {
            rigidBody.AddForce(transform.forward * linearSpeed);
        }

        public void MoveBackward()
        {
            rigidBody.AddForce(-transform.forward * linearSpeed);
        }

        public void RotateRight()
        {
            rigidBody.angularVelocity += transform.up * angularSpeed * Time.fixedDeltaTime;
        }

        public void RotateLeft()
        {
            rigidBody.angularVelocity += -transform.up * angularSpeed * Time.fixedDeltaTime;
        }

        public void Jump()
        {
            rigidBody.AddForce(transform.up * jumpForce);
        }
    }
}
