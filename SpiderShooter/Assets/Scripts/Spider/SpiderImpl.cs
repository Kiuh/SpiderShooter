using UnityEngine;

namespace Assets.Scripts.Spider
{
    [SelectionBase]
    [AddComponentMenu("Spider.SpiderImpl")]
    public class SpiderImpl : MonoBehaviour
    {
        [SerializeField]
        private float health;

        [SerializeField]
        private float bulletSpeed;
        public float BulletSpeed => bulletSpeed;

        [SerializeField]
        private float bulletDamage;
        public float BulletDamage => bulletDamage;

        public void TakeDamage(float damage)
        {
            health -= damage;
            if (health <= 0)
            {
                Destroy(gameObject);
            }
        }
    }
}
