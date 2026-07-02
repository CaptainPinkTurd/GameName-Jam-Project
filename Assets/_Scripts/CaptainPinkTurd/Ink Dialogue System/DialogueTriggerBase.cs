using UnityEngine;

namespace CaptainPinkTurd.InkDialogue
{
    public abstract class DialogueTriggerBase : MonoBehaviour
    {
        [Header("Dialogue Trigger Base Configs")]
        [SerializeField] protected string knotName;
        
        protected void StartDialogue()
        {
            if (DialogueManager.Instance.DialogueIsPlaying) return;

            DialogueManager.Instance.EnterDialogue(knotName);
        }
    }
}