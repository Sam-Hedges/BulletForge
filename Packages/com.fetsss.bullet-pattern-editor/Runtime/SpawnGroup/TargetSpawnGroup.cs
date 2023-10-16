using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace BB
{
    public class TargetSpawnGroup : SpawnGroup
    {
        public float maxRotationSpeed = 0;

        private Transform player;

        protected override void UpdatePosition(float deltaTime)
        {
            base.UpdatePosition(deltaTime);
            Vector2 p = GetPlayerPosition();
            Vector2 dir = new Vector2(p.x, p.y) - position;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            if (maxRotationSpeed < 1000)
            {
                rotation = Mathf.MoveTowardsAngle(rotation, angle, maxRotationSpeed * deltaTime);
            }
            else
            {
                rotation = angle;
            }
        }

        protected virtual Vector2 GetPlayerPosition()
        {
            if (!isSimulation)
            {
                if (player == null)
                {
                    player = GameObject.FindGameObjectWithTag("Player").transform;
                }
                return player.transform.localPosition;
            }
            else
            {
                return new Vector2(0, -3);
            }
        }
    }
}
