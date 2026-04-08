using CaptainPinkTurd.Game.Enemy;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace CaptainPinkTurd.SpawnSystem.TurnBased
{
    [CreateAssetMenu(fileName = "SpawnSettings", menuName = "Scriptable Objects/Spawn System/Spawn Settings")]
    public class SpawnSettings : ScriptableObject
    {
        [SerializeField] internal EnemyUnitBase[] enemiesType;
        [SerializeField] internal RangedFloat[] enemySpawnChanceByCount;
        [SerializeField] internal int[] numberToSpawnByCount;
        [SerializeField] internal int[] turnWaitForSpawnByCount;
    }
    
    [System.Serializable]
    public struct RangedFloat
    {
        [Range(0f, 1f)]
        public float value;
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(SpawnSettings))]
    public class SpawnSettingsEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
    
            // Draw everything except the custom-handled arrays
            SerializedProperty prop = serializedObject.GetIterator();
            bool enterChildren = true;
            while (prop.NextVisible(enterChildren))
            {
                enterChildren = false;
                if (prop.name != "enemySpawnChanceByCount" &&
                    prop.name != "numberToSpawnByCount" &&
                    prop.name != "turnWaitForSpawnByCount")
                {
                    EditorGUILayout.PropertyField(prop, true);
                }
            }
    
            EditorGUILayout.Space();
            DrawIndexedArray("enemySpawnChanceByCount", 0);
            DrawIndexedArray("numberToSpawnByCount", 0);
            DrawIndexedArray("turnWaitForSpawnByCount", 1);
    
            serializedObject.ApplyModifiedProperties();
        }
    
        private void DrawIndexedArray(string propertyName, int offset)
        {
            SerializedProperty arrayProp = serializedObject.FindProperty(propertyName);
    
            EditorGUILayout.LabelField(ObjectNames.NicifyVariableName(propertyName), EditorStyles.boldLabel);
    
            // Let designer change array size
            EditorGUI.indentLevel++;
            arrayProp.arraySize = EditorGUILayout.IntField("Size", arrayProp.arraySize);
            EditorGUI.indentLevel--;
    
            EditorGUI.indentLevel++;
            for (int i = 0; i < arrayProp.arraySize; i++)
            {
                SerializedProperty element = arrayProp.GetArrayElementAtIndex(i);
                string label = $"Enemies in Scene: {i + offset}";

                if (element.propertyType == SerializedPropertyType.Float)
                {
                    element.floatValue = EditorGUILayout.FloatField(label, element.floatValue);
                }
                else if (element.propertyType == SerializedPropertyType.Integer)
                {
                    element.intValue = EditorGUILayout.IntField(label, element.intValue);
                }
                else if (element.propertyType == SerializedPropertyType.Generic)
                {
                    EditorGUILayout.PropertyField(element, new GUIContent(label), true);
                }
                else
                {
                    EditorGUILayout.PropertyField(element, new GUIContent(label));
                }
            }
            EditorGUI.indentLevel--;
            EditorGUILayout.Space();
        }
    }
#endif
}
