using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BulletForge
{
    public class RunPattern : MonoBehaviour
    {
        public Pattern pattern;

        private Pattern patternInstance;

        private void Start()
        {
            patternInstance = Instantiate(pattern);
            patternInstance.ResetPatern();
        }

        private void FixedUpdate()
        {
            patternInstance.position = transform.localPosition;
            patternInstance.rotation = transform.eulerAngles.z;
            patternInstance.UpdatePatern(Time.fixedDeltaTime);
        }
    }
}
