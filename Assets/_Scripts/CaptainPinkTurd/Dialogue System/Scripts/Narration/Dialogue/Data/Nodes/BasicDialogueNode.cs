using System;
using UnityEngine;

namespace CaptainPinkTurd.DialogueSystem.Nodes
{
    [Serializable]
    public class BasicDialogueNode : DialogueNode
    {
        [SerializeReference]
        public DialogueNode NextNode;


        public override bool CanBeFollowedByNode(DialogueNode node)
        {
            return NextNode == node;
        }

        public override void Accept(DialogueNodeVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}