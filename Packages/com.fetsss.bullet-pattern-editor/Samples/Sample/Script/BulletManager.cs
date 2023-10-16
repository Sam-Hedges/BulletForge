using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BB
{
    public class BulletManager : MonoBehaviour
    {
        public static BulletManager Main
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<BulletManager>();
                }
                if (instance == null)
                {
                    GameObject bulletManager = new GameObject("Bullet Manager");
                    bulletManager.AddComponent(typeof(BulletManager));
                    //Instantiate(bulletManager);
                    instance = FindObjectOfType<BulletManager>();
                }
                return instance;
            }
            private set
            {
                instance = value;
            }
        }
        private static BulletManager instance;

        [SerializeField]
        private GameObject bulletPrefab;
        private Stack<Bullet> pool = new Stack<Bullet>();
        private List<Bullet> activeBullets = new List<Bullet>();

        private void Awake()
        {
            Main = this;
            if (bulletPrefab == null)
            {
                bulletPrefab = new GameObject();
                bulletPrefab.transform.parent = transform;
                bulletPrefab.AddComponent(typeof(SpriteRenderer));
                bulletPrefab.AddComponent(typeof(Bullet));
            }
        }

        public void Destroy(Bullet bullet)
        {
            bullet.gameObject.SetActive(false);
            if (activeBullets.Contains(bullet))
            {
                activeBullets.Remove(bullet);
            }
            pool.Push(bullet);
        }

        public Bullet Spawn()
        {
            if (pool.Count <= 0)
            {
                Bullet b = Instantiate(bulletPrefab, transform).GetComponent<Bullet>();
                b.gameObject.SetActive(true);
                activeBullets.Add(b);
                return b;
            }
            else
            {
                Bullet b = pool.Pop();
                b.gameObject.SetActive(true);
                activeBullets.Add(b);
                return b;
            }
        }

        public List<Bullet> BulletsInRange(Vector2 center, float radius)
        {
            List<Bullet> bullets = new List<Bullet>();
            foreach (Bullet bullet in activeBullets)
            {
                Vector2 p = bullet.transform.localPosition;
                if ((p - center).sqrMagnitude <= radius * radius)
                {
                    bullets.Add(bullet);
                }
            }
            return bullets;
        }
    }
}
