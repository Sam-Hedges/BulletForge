using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BB
{
    [System.Serializable]
    public class MovementSpawnGroup : SpawnGroup
    {
        public float rotationSpeed = 0;
        public Vector2 movementSpeed;

        protected override void UpdatePosition(float deltaTime)
        {
            base.UpdatePosition(deltaTime);
            Vector2 rot = new Vector2(Mathf.Cos(rotation * Mathf.Deg2Rad), -Mathf.Sin(rotation * Mathf.Deg2Rad));
            position += new Vector2(movementSpeed.x * rot.x + movementSpeed.y * rot.y, -movementSpeed.x * rot.y + movementSpeed.y * rot.x) * deltaTime;
            //position += movementSpeed * deltaTime;
            rotation += rotationSpeed * deltaTime;
        }
    }
}
