using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BB
{
    public interface IBullet
    {
        /// <summary>
        /// Set position of the bullet
        /// </summary>
        public void SetPosition(Vector2 position);

        /// <summary>
        /// Set roattion of the bullet, in euler angle
        /// </summary>
        public void SetRotation(float rotation);

        /// <summary>
        /// Set speed of the bullet
        /// </summary>
        public void SetSpeed(float speed);

        /// <summary>
        /// Update the position of the bullet according to the parent spawngroup
        /// This is only needed if the bullet is spawned as local simulation space
        /// See the provided Bullet class to see how it is implemented
        /// </summary>
        public void UpdateLocalPosition(Vector2 deltaPosition, float deltaAngle);

        /// <summary>
        /// Create a new Bullet object that impliment the IBullet interface
        /// The new Bullet should already be in the scene and able to move on its own
        /// </summary>
        /// <param name="parentSpawnGroup">The SpawnGroup that spawns the bullet</param>
        /// <returns>The new bullet</returns>
        public IBullet SpawnBullet(SpawnGroup parentSpawnGroup);
    }
}