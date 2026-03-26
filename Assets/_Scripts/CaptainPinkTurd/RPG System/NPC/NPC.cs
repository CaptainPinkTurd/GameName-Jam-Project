using System.Collections;
using CaptainPinkTurd.AudioSystem;
using CaptainPinkTurd.Core.DesignPattern.SOAP.Events;
using CaptainPinkTurd.Core.DesignPattern.SOAP.Variables;
using CaptainPinkTurd.Core.Interfaces;
using CaptainPinkTurd.RPG.Dialogue;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CaptainPinkTurd.RPG.NPC
{
    public class NPC : MonoBehaviour, IInteractable
    {
        [SerializeField] private GameObject dialogueBox;
        [SerializeField] private NPCDialogue dialogueData;
        [SerializeField] private TMP_Text dialogueText;
        [SerializeField] private Image portraitImage;
        [SerializeField] private BoolEvent onPortraitToggle;

        private int dialogueIndex;
        private bool isTyping; 
        private bool isDialogueActive;
        public bool CanInteract => !isDialogueActive;
        
        public void Interact()
        {
            if (!dialogueData)
            {
                Debug.LogError($"No dialogue data assigned to {name}");
                return;
            }

            if (isDialogueActive)
            {
                NextLine();
            }
            else
            {
                StartDialogue();
            }
        }

        private void StartDialogue()
        {
            isDialogueActive = true;
            dialogueIndex = 0;

            if (dialogueData.npcPortrait)
            {
                portraitImage.gameObject.SetActive(true);
                portraitImage.sprite = dialogueData.npcPortrait;
            }
            else
            {
                portraitImage.gameObject.SetActive(false);
            }
            
            dialogueBox.SetActive(true);
            onPortraitToggle.Raise(portraitImage.gameObject.activeInHierarchy); //dialogue box need to be active first

            StartCoroutine(TypeLine());
        }
        private void NextLine()
        {
            if (isTyping)
            {
                StopAllCoroutines();
                dialogueText.SetText(dialogueData.dialogueLines[dialogueIndex].Value);
                isTyping = false;
            }
            else if (dialogueData.dialogueLines.Length > ++dialogueIndex)
            {
                StartCoroutine(TypeLine());
            }
            else
            {
                EndDialogue();
            }
        }

        private void EndDialogue()
        {
            StopAllCoroutines();
            isDialogueActive = false;
            dialogueText.SetText("");
            dialogueBox.SetActive(false);
        }

        private IEnumerator TypeLine()
        {
            isTyping = true;
            dialogueText.SetText("");
            
            foreach(char letter in dialogueData.dialogueLines[dialogueIndex].Value)
            {
                dialogueText.text += letter;
                SoundManager.Instance.CreateSoundBuilder().WithPosition(transform.position).Play(dialogueData.voiceSound);
                yield return new WaitForSeconds(dialogueData.dialogueLines[dialogueIndex].Key.dialogueSpeed);
            }
            
            isTyping = false;

            if (dialogueData.dialogueLines.Length > dialogueIndex && dialogueData.dialogueLines[dialogueIndex].Key.autoProgressToNextLine)
            {
                yield return new WaitForSeconds(dialogueData.dialogueLines[dialogueIndex].Key.autoProgressDelay);    
                NextLine();
            }
        }
    }
}
