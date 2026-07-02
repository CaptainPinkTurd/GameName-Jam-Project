using UnityEngine;

namespace CaptainPinkTurd.InkDialogue
{
    public class AutoDialogueTrigger : DialogueTriggerBase
    {
        [Header("Auto Dialogue Trigger Configs")]
        [SerializeField] private float delay;
        [SerializeField] private bool triggerOnce = true;

        private bool hasTriggered;

        private void Start()
        {
            if (delay > 0f)
            {
                Invoke(nameof(Trigger), delay);
            }
            else
            {
                Trigger();
            }
        }

        public void Trigger()
        {
            if (triggerOnce && hasTriggered) return;
            hasTriggered = true;

            StartDialogue();
        }
    }
}