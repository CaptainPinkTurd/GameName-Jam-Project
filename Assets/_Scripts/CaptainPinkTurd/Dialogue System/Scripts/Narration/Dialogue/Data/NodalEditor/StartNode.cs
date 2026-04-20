using UnityEngine;
using XNode;

namespace CaptainPinkTurd.DialogueSystem.NodalEditor
{
    [NodeTint("#00FF00"), NodeWidth(100)]
    public class StartNode : Node
    {
        [SerializeField]
        [Output(backingValue = ShowBackingValue.Never, connectionType = ConnectionType.Override)]
        public DialogueNode m_FirstNode;

        public override object GetValue(NodePort port)
        {
            return m_FirstNode;
        }
    }
}
