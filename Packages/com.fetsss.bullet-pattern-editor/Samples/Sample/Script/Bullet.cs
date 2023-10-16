using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BB
{
    public class Bullet : MonoBehaviour, IBullet
    {
        private SpawnGroup parent;

        private SpriteRenderer spr;
        public float radius;
        public float lifeTime;

        public float rotation;
        public float speed;
        public Vector2 scale;

        private Transform player;

        private void Awake()
        {
            spr = GetComponent<SpriteRenderer>();
        }

        void FixedUpdate()
        {
            lifeTime -= Time.fixedDeltaTime;

            Vector3 dir = new Vector3(Mathf.Cos(rotation * Mathf.Deg2Rad), Mathf.Sin(rotation * Mathf.Deg2Rad));
            transform.localPosition += dir * speed * Time.fixedDeltaTime;

            if (lifeTime <= 0)
            {
                Destroy();
                return;
            }

            if (player == null) player = GameObject.FindGameObjectWithTag("Player")?.transform;

            if (player != null)
            {
                Vector2 rot = dir;
                Vector2 dif = player.position - transform.position;
                if (new Vector2((dif.x * rot.x + dif.y * rot.y) / scale.x, (-dif.x * rot.y + dif.y * rot.x) / scale.y).sqrMagnitude < radius * radius)
                {
                    player.SendMessage("Hit");
                    Destroy();
                }
            }

        }

        public void Destroy()
        {
            if (parent != null)
            {
                ((BulletSpawnGroup)parent).RemoveBullet();
            }
            BulletManager.Main.Destroy(this);
        }

        public void SetPosition(Vector2 position)
        {
            transform.localPosition = position;
        }

        public void SetRotation(float rotation)
        {
            transform.eulerAngles = new Vector3(0, 0, rotation);
            this.rotation = rotation;
        }

        public void SetSpeed(float speed)
        {
            this.speed = speed;
        }

        public void UpdateLocalPosition(Vector2 deltaPosition, float deltaAngle)
        {
            transform.localPosition += new Vector3(deltaPosition.x, deltaPosition.y);
            SetRotation(rotation + deltaAngle);
            if (parent != null)
            {
                Vector2 d = transform.localPosition - new Vector3(parent.position.x, parent.position.y);
                Vector2 rot = new Vector2(Mathf.Cos(deltaAngle * Mathf.Deg2Rad), -Mathf.Sin(deltaAngle * Mathf.Deg2Rad));
                transform.localPosition = parent.position + new Vector2(d.x * rot.x + d.y * rot.y, -d.x * rot.y + d.y * rot.x);
            }
        }

        public IBullet SpawnBullet(SpawnGroup parentSpawnGroup)
        {
            Bullet bullet = BulletManager.Main.Spawn();

            spr = GetComponent<SpriteRenderer>();
            bullet.spr = bullet.GetComponent<SpriteRenderer>();

            bullet.spr.sprite = spr.sprite;
            bullet.SetPosition(transform.localPosition);
            bullet.SetRotation(rotation);
            bullet.lifeTime = lifeTime;
            bullet.radius = radius;
            bullet.speed = speed;
            bullet.scale = scale;
            bullet.parent = parentSpawnGroup;

            bullet.transform.localScale = transform.localScale;

            return bullet;
        }
    }
}
