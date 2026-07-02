using System.Collections.Generic;
using CaptainPinkTurd.Core.Utilities;
using CaptainPinkTurd.Core.Utils;
using CaptainPinkTurd.UI.LayoutUI;
using Ink.Runtime;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using ZLinq;

namespace CaptainPinkTurd.InkDialogue
{
    public class DialoguePanelUI : MonoBehaviour
    {
        [Header("Dialogue Panel UI Properties")] 
        [SerializeField] private GameObject contentParent;
        [SerializeField] private TMP_Text dialogueText;
        [SerializeField] private TypewriterText typewriterText;
        [SerializeField] private GraphicRaycaster graphicRaycaster;
        [SerializeField] private LayoutGroupController choiceGroup;
        [SerializeField] private List<GameObject> objectsHiddenWhileTyping;

        private void Awake()
        {
            DialogueFinished();
        }

        private void OnEnable()
        {
            StartCoroutine(CoroutineUtils.WaitForCondition(() => DialogueManager.HasInstance, () =>
            {
                DialogueManager.Instance.OnDialogueStart.Subscribe(DialogueStarted);
                DialogueManager.Instance.OnDialogueEnd.Subscribe(DialogueFinished);
                DialogueManager.Instance.OnDisplayDialogue.Subscribe(DisplayDialogue);
            }));
        }

        private void OnDisable()
        {
            if (!DialogueManager.HasInstance) return;
            
            DialogueManager.Instance.OnDialogueStart.Unsubscribe(DialogueStarted);
            DialogueManager.Instance.OnDialogueEnd.Unsubscribe(DialogueFinished);
            DialogueManager.Instance.OnDisplayDialogue.Unsubscribe(DisplayDialogue);
        }

        private void DialogueStarted()
        {
            contentParent.SetActive(true);
        }
        private void DialogueFinished()
        {
            contentParent.SetActive(false);
            ResetPanel();
        }
        private void ResetPanel()
        {
            dialogueText.text = "";
            choiceGroup.RemoveAllLayoutElements();
            graphicRaycaster.enabled = true;
        }
        private void DisplayDialogue(DialogueInfo dialogueInfo)
        {
            if (typewriterText.IsTyping)
            {
                typewriterText.SkipTyping();
                return;
            }

            SetHiddenObjectsActive(false);
            typewriterText.StartTyping(dialogueInfo.speaker, dialogueInfo.line, dialogueText.alignment, () =>
            {
                DialogueManager.Instance.DialogueIsTyping = false;
                SetHiddenObjectsActive(true);
                DisplayChoices(dialogueInfo.choices);
            });
        }
        
        private void DisplayChoices(List<Choice> dialogueChoices)
        {
            graphicRaycaster.enabled = false;
            
            choiceGroup.AddLayoutElements(dialogueChoices.Count);
            var choiceButtons = choiceGroup.CurrentLayoutElements.AsValueEnumerable().Reverse().ToArray();
            var index = 0;
            foreach (Choice choice in dialogueChoices)
            {
                var buttonObject = choiceButtons[index];
                if(buttonObject.TryGetComponent(out DialogueChoiceButton choiceButton))
                {
                    var choiceIndex = index;
                    
                    choiceButton.Button.onClick.RemoveAllListeners();
                    choiceButton.Button.onClick.AddListener(choiceGroup.RemoveAllLayoutElements);
                    
                    choiceButton.SetChoiceText(choice.text);
                    choiceButton.SetChoiceIndex(choiceIndex);

                    if (choiceIndex == 0)
                    {
                        //Player/Confirm and UI/Submit may share the Space key; selecting now would let this same keypress
                        //immediately fire UI/Submit on this button and wipe the choices we just created. Wait a frame so that press expires first.
                        StartCoroutine(CoroutineUtils.WaitForNextFrames(() =>
                        {
                            choiceButton.SelectButton();
                            DialogueManager.Instance.UpdateChoiceIndex(choiceIndex);
                        }));
                    }
                }
                else
                {
                    Debug.LogError(choiceButton + " no button component found on choice UI.");
                }
                
                index++;
            }
        }
        private void SetHiddenObjectsActive(bool active)
        {
            foreach (GameObject go in objectsHiddenWhileTyping)
            {
                go.SetActive(active);
            }
        }
    }
}