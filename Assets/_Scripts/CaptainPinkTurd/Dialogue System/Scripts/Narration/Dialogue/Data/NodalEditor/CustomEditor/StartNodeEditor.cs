using UnityEngine;
using XNodeEditor;

namespace CaptainPinkTurd.DialogueSystem.NodalEditor.Editor
{
    [CustomNodeEditor(typeof(StartNode))]
    public class StartEditor : NodeEditor {

        public override void OnBodyGUI() {
            serializedObject.Update();

            NodeEditorGUILayout.PortField(GUIContent.none, target.GetOutputPort("m_FirstNode"), GUILayout.MinWidth(0));

            serializedObject.ApplyModifiedProperties();
        }
    }
}
