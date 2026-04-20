using System;
using System.Linq;
using UnityEngine;

namespace CaptainPinkTurd.DialogueSystem.Nodes
{
    [Serializable]
    public class DialogueChoice
    {
        [SerializeField]
        public string ChoicePreview;
        [SerializeReference]
        public DialogueNode ChoiceNode;
    }


    [Serializable]
    public class ChoiceDialogueNode : DialogueNode
    {
        public DialogueChoice[] Choices;


        public override bool CanBeFollowedByNode(DialogueNode node)
        {
            return Choices.Any(x => x.ChoiceNode == node);
        }

        public override void Accept(DialogueNodeVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}