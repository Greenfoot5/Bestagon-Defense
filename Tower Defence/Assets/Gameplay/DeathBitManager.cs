using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

namespace Gameplay
{
    public class DeathBitManager: MonoBehaviour
    {
        // If energy drops are enabled at all
        public static bool dropsEnergy = true;
        
        [Tooltip("Set to Unity's default Quad mesh")]
        [SerializeField]
        private Mesh mesh;
        [Tooltip("The material to use for death bits")]
        // this is what it looks like, you want some unlit shader with transparency
        // - ideally opaque with alpha clipping to cut down on overdraw, but if you fade them or they're transparent that's fine
        [SerializeField]
        private Material bitMaterial;
        [Tooltip("The material to use for death bytes")]
        [SerializeField]
        private Material byteMaterial;

        internal static readonly List<DeathEnergy> Particles = new();
        
        private RenderParams _bitRender;
        private RenderParams _byteRender;
        private UnityEngine.Camera _camera;

        private const float CatchRadius = 2f;
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void Init()
        {
            Particles.Clear();
        }

        private void Start()
        {
            _bitRender = new RenderParams(bitMaterial)
            {
                layer = gameObject.layer
            };
            _byteRender = new RenderParams(byteMaterial)
            {
                layer = gameObject.layer
            };
            _camera = UnityEngine.Camera.main;
            Particles.Clear();

            GameStats.OnRoundProgress += CleanMap;
        }

        public static void DropEnergy(Vector3 position, int value, Quaternion? rotation = null, Vector3? scale = null)
        {
            if (!dropsEnergy)
            {
                GameStats.Energy += value;
                return;
            }

            rotation ??= Quaternion.identity;
            scale ??= Vector3.one;
            
            int valueLeft = value;

            while (valueLeft > 0)
            {
                int particleValue = Random.Range(1, Math.Min(4, valueLeft));
                Vector3 placePos = position + new Vector3(0.8f * Random.value - 0.4f, 0.8f * Random.value - 0.4f, -1f);
                
                Particles.Add(new DeathEnergy(placePos, rotation.Value, scale.Value, particleValue, GameStats.Rounds));
                valueLeft -= particleValue;
            }
        }

        private void LateUpdate()
        {
            foreach (DeathEnergy particle in Particles)
            {
                if (particle.Value < DeathEnergy.ByteValue)
                    Graphics.RenderMesh(_bitRender, mesh, 0, particle.GetTransform());
                else
                    Graphics.RenderMesh(_byteRender, mesh, 0, particle.GetTransform());
            }
        }

        private void FixedUpdate()
        {
            Vector3 mousePos = Mouse.current.position.ReadValue();
            mousePos = _camera.ScreenToWorldPoint(mousePos);

            for (var i = 0; i < Particles.Count; ++i)
            {
                if (((Vector2)(mousePos - Particles[i].Position)).sqrMagnitude < CatchRadius * CatchRadius)
                {
                    Particles[i].Animate();
                }

                if (Particles[i].HasFinishedAnimating())
                {
                    Particles.RemoveAt(i);
                    i--;
                }
            }
        }

        private void CleanMap()
        {
            for (var i=0; i < Particles.Count; ++i)
            {
                if (Particles[i].SpawnWave <= GameStats.Rounds - 3)
                {
                    Particles[i].Animate();
                }

                if (Particles[i].HasFinishedAnimating())
                {
                    Particles.RemoveAt(i);
                    i--;
                }
            }
        }
    }
}