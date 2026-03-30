using System.Collections;
using CaptainPinkTurd.AudioSystem;
using CaptainPinkTurd.Core.CustomDataStructure;
using CaptainPinkTurd.Core.Interfaces;
using UnityEngine;

namespace CaptainPinkTurd.RPG.Dialogue
{
    public class DialogueInteractable : MonoBehaviour, IInteractable
    {
        [Header("Dialogue Interactable Properties")]
        [SerializeField] private DialogueData dialogueData;

        private DialogueBox dialogueBox;
        private int dialogueIndex;
        private bool isTyping; 
        private bool isDialogueActive;
        public bool CanInteract => !isDialogueActive;
        
        private void Start()
        {
            dialogueBox = DialogueBox.Instance;
        }
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

            dialogueBox.SetNPCPortrait(dialogueData.npcPortrait);
            dialogueBox.ShowDialogueUI(true);

            DisplayCurrentLine();
        }
        private void NextLine()
        {
            if (isTyping)
            {
                // Skip typing animation and show the full line
                StopAllCoroutines();
                dialogueBox.SetDialogueText(dialogueData.dialogueLines[dialogueIndex].text);
                isTyping = false;
            }
            
            // Clear choices
            dialogueBox.ClearChoices();
            
            // Check if dialogue has ended
            if (dialogueData.dialogueLines.Length > dialogueIndex &&
                dialogueData.dialogueLines[dialogueIndex].endDialogue)
            {
                EndDialogue();
                return;
            }
            
            // Check if there are choices and display
            for (int i = 0; i < dialogueData.dialogueLines.Length; i++)
            {
                if(dialogueData.dialogueLines[i].branchingOptions.Length == 0 || 
                   i != dialogueIndex) continue;

                DisplayChoices(dialogueData.dialogueLines[i].branchingOptions);
                return;
            }
                
            if (dialogueData.dialogueLines.Length > ++dialogueIndex)
            {
                // Go on to the next line 
                DisplayCurrentLine();
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
            dialogueBox.SetDialogueText("");
            dialogueBox.ShowDialogueUI(false);
        }

        private void DisplayChoices(SerializeKeyValuePair<string, int>[] choices)
        {
            dialogueBox.ShowChoiceBox(true);
            for (int i = 0; i < choices.Length; i++)
            {
                var nextDialogueIndex = choices[i].Value;
                dialogueBox.CreateChoiceButton(choices[i].Key, 
                    () => ChooseOption(nextDialogueIndex));
            }
        }

        private void ChooseOption(int nextIndex)
        {
            dialogueIndex = nextIndex;
            dialogueBox.ClearChoices();
            DisplayCurrentLine();
        }

        private void DisplayCurrentLine()
        {
            StopAllCoroutines();
            StartCoroutine(TypeLine());
        }

        private IEnumerator TypeLine()
        {
            isTyping = true;
            dialogueBox.SetDialogueText("");
            string dialogueText = "";
            
            foreach(char letter in dialogueData.dialogueLines[dialogueIndex].text)
            {
                dialogueText += letter;
                dialogueBox.SetDialogueText(dialogueText);
                
                SoundManager.Instance.CreateSoundBuilder().WithPosition(transform.position).Play(dialogueData.voiceSound);
                
                yield return new WaitForSeconds(dialogueData.dialogueLines[dialogueIndex].dialogueSpeed);
            }
            
            isTyping = false;

            if (dialogueData.dialogueLines.Length > dialogueIndex && dialogueData.dialogueLines[dialogueIndex].autoProgressToNextLine)
            {
                yield return new WaitForSeconds(dialogueData.dialogueLines[dialogueIndex].autoProgressDelay);    
                NextLine();
            }
        }
    }
}