using Turrets.Gunner;
using UnityEditor;

namespace Editor.Turrets
{
    [CustomEditor(typeof(Gunner), true)]
    public class GunnerEditor : DynamicTurretEditor
    {
        // PROPERTIES
        private SerializedProperty _bulletPrefab;
        private SerializedProperty _attackEffect;
        
        private SerializedProperty _spinMultiplier;
        private SerializedProperty _maxFireRate;
        private SerializedProperty _spinCooldown;

        protected new void OnEnable()
        {
            base.OnEnable();

            _bulletPrefab = serializedObject.FindProperty("bulletPrefab");
            _attackEffect = serializedObject.FindProperty("attackEffect");
            _spinMultiplier = serializedObject.FindProperty("spinMultiplier");
            _maxFireRate = serializedObject.FindProperty("maxFireRate");
            _spinCooldown = serializedObject.FindProperty("spinCooldown");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            serializedObject.Update();

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Gunner", EditorStyles.boldLabel);

            EditorGUILayout.PropertyField(_bulletPrefab);
            EditorGUILayout.PropertyField(_attackEffect);
            
            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(_spinMultiplier);
            EditorGUILayout.PropertyField(_spinCooldown);
            EditorGUILayout.PropertyField(_maxFireRate);

            serializedObject.ApplyModifiedProperties();
        }
    }
}
