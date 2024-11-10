using Turrets.Choker;
using UnityEditor;

namespace Editor.Turrets
{
    [CustomEditor(typeof(Choker), true)]
    public class ChokerEditor : DynamicTurretEditor
    {
        // PROPERTIES
        private SerializedProperty _bulletPrefab;
        private SerializedProperty _attackEffect;
        
        private SerializedProperty _partSpread;
        private SerializedProperty _partCount;

        protected new void OnEnable()
        {
            base.OnEnable();

            _bulletPrefab = serializedObject.FindProperty("bulletPrefab");
            _attackEffect = serializedObject.FindProperty("attackEffect");
            _partSpread = serializedObject.FindProperty("partSpread");
            _partCount = serializedObject.FindProperty("partCount");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            serializedObject.Update();

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Choker", EditorStyles.boldLabel);

            EditorGUILayout.PropertyField(_bulletPrefab);
            EditorGUILayout.PropertyField(_attackEffect);
            
            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(_partSpread);
            EditorGUILayout.PropertyField(_partCount);

            serializedObject.ApplyModifiedProperties();
        }
    }
}
