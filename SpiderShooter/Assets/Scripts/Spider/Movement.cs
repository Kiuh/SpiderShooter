using Mirror;
using UnityEngine;

namespace Spider
{
    [AddComponentMenu("Spider.Movement")]
    public class Movement : NetworkBehaviour
    {
        [SerializeField]
        private Transform mainBody;

        [Header("Linear Movement")]
        [SerializeField]
        private float linearSpeed;

        [Header("Rotation Movement")]
        [SerializeField]
        private float angularSpeed;

        public Vector3 MovingDirection => mainBody.forward;

        public void MoveForward()
        {
            mainBody.Translate(linearSpeed * Time.deltaTime * mainBody.forward, Space.World);
        }

        public void MoveBackward()
        {
            mainBody.Translate(linearSpeed * Time.deltaTime * -mainBody.forward, Space.World);
        }

        public void RotateRight()
        {
            mainBody.Rotate(0, angularSpeed * Time.deltaTime, 0);
        }

        public void RotateLeft()
        {
            mainBody.Rotate(0, -angularSpeed * Time.deltaTime, 0);
        }
    }
}
