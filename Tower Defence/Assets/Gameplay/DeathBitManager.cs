using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Gameplay
{
    [RequireComponent(typeof(UnityEngine.Camera))]
    public class DeathBitManager: MonoBehaviour
    {
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

        private static readonly List<Matrix4x4> Bits = new();
        private static readonly List<Matrix4x4> Bytes = new();
        
        private RenderParams _bitRender;
        private RenderParams _byteRender;
        private UnityEngine.Camera _camera;

        private const float CatchRadius = 2f;
        [Tooltip("How much energy 1 bit is worth")]
        private const int BitValue = 1;
        [Tooltip("How much energy 1 byte is worth")]
        private const int ByteValue = 3;
        [Tooltip("How much to scale bit material by")]
        private const float BitScale = 0.4f;
        [Tooltip("How much to scale byte material by")]
        private const float ByteScale = 0.6f;

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
            _camera = GetComponent<UnityEngine.Camera>();
        }

        public static void DropEnergy(Vector3 position, int value, Quaternion? rotation = null, Vector3? scale = null)
        {
            rotation ??= Quaternion.identity;
            scale ??= Vector3.one;
            
            int valueLeft = value;

            while (valueLeft > 0)
            {
                Vector3 placePos = position + new Vector3(0.8f * Random.value - 0.4f, 0.8f * Random.value - 0.4f, -1f);
                if (valueLeft < ByteValue)
                {
                    Bits.Add(Matrix4x4.TRS(placePos, rotation.Value, scale.Value * BitScale));
                    valueLeft -= BitValue;
                }
                else
                {
                    if (Random.value >= 0.5f)
                    {
                        Bits.Add(Matrix4x4.TRS(placePos, rotation.Value, scale.Value * BitScale));
                        valueLeft -= BitValue;
                    }
                    else
                    {
                        Bytes.Add(Matrix4x4.TRS(placePos, rotation.Value, scale.Value * ByteScale));
                        valueLeft -= ByteValue;
                    }
                }
            }
            
        }

        private void LateUpdate()
        {
            foreach (Matrix4x4 particle in Bits)
            {
                Graphics.RenderMesh(_bitRender, mesh, 0, particle);
            }
            foreach (Matrix4x4 particle in Bytes)
            {
                Graphics.RenderMesh(_byteRender, mesh, 0, particle);
            }
        }

        private void FixedUpdate()
        {
            Vector3 mousePos = Mouse.current.position.ReadValue();
            mousePos = _camera.ScreenToWorldPoint(mousePos);
            
            for (var i=0; i < Bits.Count; ++i)
            {
                if ((mousePos - Bits[i].GetPosition()).sqrMagnitude < CatchRadius * CatchRadius)
                {
                    Bits.RemoveAt(i);
                    GameStats.Energy += BitValue;
                    i--;
                }
            }
            
            for (var i=0; i < Bytes.Count; ++i)
            {
                if ((mousePos - Bytes[i].GetPosition()).sqrMagnitude < CatchRadius * CatchRadius)
                {
                    Bytes.RemoveAt(i);
                    GameStats.Energy += ByteValue;
                    i--;
                }
            }
        }
    }
}