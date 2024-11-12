using UnityEngine;

namespace Gameplay
{
    public class DeathEnergy
    {
        public readonly int Value;
        public readonly int SpawnWave;
        public readonly Vector3 Position;
        private readonly Quaternion _rotation;
        private readonly Vector3 _scale;
        
        private float _animationScale;
        private float _startTime;
        public bool IsAnimating;
        
        // How much energy 1 byte is worth
        internal const int ByteValue = 3;
        // How much to scale bit material by
        private const float BitScale = 0.4f;
        // How much to scale byte material by
        private const float ByteScale = 0.6f;

        public Matrix4x4 GetTransform()
        {
            if (!IsAnimating)
                return Matrix4x4.TRS(Position, _rotation, _scale);
            
            float x = Time.time - _startTime;
            // -0.8007x^{2}+0.5654x+1.092
            _animationScale = -32.5f * (x * x) + 6.85f * x + 1;
            return Matrix4x4.TRS(Position, _rotation, _scale * Mathf.Max(0f, _animationScale));
        }

        public DeathEnergy(Vector3 position, Quaternion rotation, Vector3 scale, int value, int spawnWave)
        {
            Position = position;
            _rotation = rotation;
            _scale = scale * (value < ByteValue ? BitScale : ByteScale);
            Value = value;
            SpawnWave = spawnWave;
            
            _startTime = 1f;
            _animationScale = 1f;
            IsAnimating = false;
        }

        public void Animate()
        {
            if (IsAnimating)
                return;
            
            _startTime = Time.time;
            IsAnimating = true;
            GameStats.Energy += Value;
        }

        public bool HasFinishedAnimating()
        {
            return _animationScale < 0.2f;
        }
    }
}