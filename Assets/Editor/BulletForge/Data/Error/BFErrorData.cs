using UnityEngine;

namespace BulletForge.Data.Error
{
    public class BFErrorData
    {
        public Color Color { get; set; }

        public BFErrorData()
        {
            GenerateRandomColor();
        }

        private void GenerateRandomColor()
        {
            Color = new Color32(
                (byte) Random.Range(65, 256),
                (byte) Random.Range(50, 176),
                (byte) Random.Range(50, 176),
                255
            );
        }
    }
}