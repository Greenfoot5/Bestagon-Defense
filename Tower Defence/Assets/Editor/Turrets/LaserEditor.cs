using Turrets.Laser;
using UnityEditor;

namespace Editor.Turrets
{
    [CustomEditor(typeof(Laser), true)]
    public class LaserEditor : DynamicTurretEditor
    {
        // PROPERTIES
        private SerializedProperty _lineRenderer;
        private SerializedProperty _impactEffect;
        private SerializedProperty _laserDuration;
        private SerializedProperty _laserCooldown;

        protected new void OnEnable()
        {
            base.OnEnable();

            _lineRenderer = serializedObject.FindProperty("lineRenderer");
            _impactEffect = serializedObject.FindProperty("impactEffect");
            _laserDuration = serializedObject.FindProperty("laserDuration");
            _laserCooldown = serializedObject.FindProperty("laserCooldown");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            serializedObject.Update();

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Laser", EditorStyles.boldLabel);

            EditorGUILayout.PropertyField(_lineRenderer);
            EditorGUILayout.PropertyField(_impactEffect);
            EditorGUILayout.PropertyField(_laserDuration);
            EditorGUILayout.PropertyField(_laserCooldown);

            serializedObject.ApplyModifiedProperties();
        }
    }
}
