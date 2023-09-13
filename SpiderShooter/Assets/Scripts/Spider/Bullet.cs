using Assets.Scripts.Spider;
using Common;
using UnityEngine;

namespace Spider
{
    [SelectionBase]
    [RequireComponent(typeof(Collider))]
    [AddComponentMenu("Spider.Bullet")]
    public class Bullet : MonoBehaviour
    {
        [SerializeField]
        [InspectorReadOnly]
        private Vector3 flyingDirection;

        [SerializeField]
        [InspectorReadOnly]
        private float flyingSpeed;

        [SerializeField]
        [InspectorReadOnly]
        private float damage;

        public void SetFlyingDirection(Vector3 direction)
        {
            flyingDirection = direction;
        }

        public void SetFlyingSpeed(float speed)
        {
            flyingSpeed = speed;
        }

        public void SetDamage(float damage)
        {
            this.damage = damage;
        }

        private void Update()
        {
            transform.Translate(flyingSpeed * Time.deltaTime * flyingDirection);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.tag is "Environment")
            {
                Destroy(gameObject);
            }
            if (other.gameObject.tag is "SpiderBody")
            {
                SpiderImpl spider = other.gameObject.GetComponentInParent<SpiderImpl>();
                spider.TakeDamage(damage);
                Destroy(gameObject);
            }
        }
    }
}
