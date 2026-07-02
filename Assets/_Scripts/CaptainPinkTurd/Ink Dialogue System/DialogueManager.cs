using System.Collections.Generic;
using CaptainPinkTurd.Core;
using CaptainPinkTurd.Core.DesignPattern.Singleton;
using CaptainPinkTurd.Core.DesignPattern.SOAP.Events;
using CaptainPinkTurd.Input;
using Ink.Runtime;
using UnityEngine;
using UnityEngine.InputSystem;

namespace CaptainPinkTurd.InkDialogue
{
    public class DialogueManager : Singleton<DialogueManager>
    {
        [Header("Ink Story")]
        [SerializeField] private TextAsset inkJson;
        
        [Header("Dialogue Events")]
        [SerializeField] private VoidEvent onDialogueStart;
        [SerializeField] private VoidEvent onDialogueEnd;
            
        private InkExternalFunctions inkExternalFunctions;
        private InkDialogueVariables inkDialogueVariables;
        private Story story;
        private InputSystemActions playerInput;
        private Animator layoutAnimator;

        private string currentSpeaker = "Default";
        private int currentChoiceIndex = -1;

        private const string SPEAKER_TAG = "speaker";
        private const string PORTRAIT_TAG = "portrait";
        private const string LAYOUT_TAG = "layout";
        
        public GameEvent OnDialogueStart { get; private set; } 
        public GameEvent OnDialogueEnd { get; private set; } 
        public GameEvent<DialogueInfo> OnDisplayDialogue { get; private set; } 
        
        public bool DialogueIsPlaying { get; private set; }
        public bool DialogueIsTyping { get; internal set; }

        protected override void Awake()
        {
            base.Awake();
            //layoutAnimator = dialoguePanel.GetComponent<Animator>();
            
            story = new Story(inkJson.text);
            inkExternalFunctions = new InkExternalFunctions();
            inkDialogueVariables = new InkDialogueVariables(story);
            
            playerInput = new InputSystemActions();
            DialogueIsPlaying = false;

            OnDialogueStart = new GameEvent();
            OnDisplayDialogue = new GameEvent<DialogueInfo>();
            OnDialogueEnd = new GameEvent();
        }

        private void OnEnable()
        {
            playerInput.Enable();
            playerInput.Player.Confirm.performed += ContinueOrExitStory;
            
            OnDialogueStart.Subscribe(onDialogueStart.Raise);
            OnDialogueEnd.Subscribe(onDialogueEnd.Raise);
        }

        private void OnDisable()
        {
            playerInput.Disable();
            playerInput.Player.Confirm.performed -= ContinueOrExitStory;
            
            OnDialogueStart.Unsubscribe(onDialogueStart.Raise);
            OnDialogueEnd.Unsubscribe(onDialogueEnd.Raise);
        }

        public void EnterDialogue(string knotName)
        {
            if (DialogueIsPlaying) return;
            
            DialogueIsPlaying = true;
            OnDialogueStart.Raise();

            if (!knotName.Equals(""))
            {
                story.ChoosePathString(knotName);
            }
            else
            {
                Debug.Log("No knot name provided");
            }
            
            //reset portrait, layout and speaker
            currentSpeaker = "Default";
            //layoutAnimator.Play("left");
            
            //start listening for variables
            inkDialogueVariables.SyncVariablesAndStartListening(story);
            
            ContinueOrExitStory(default);
        }

        private void ExitDialogue()
        {
            DialogueIsPlaying = false;
            inkDialogueVariables.StopListening(story);
            story.ResetState();
            OnDialogueEnd.Raise();
        }
        
        //if race condition ever happens to input in the future, then you should implement an input events and context to your input assembly and
        //follow the same structure as the guy who made this system
        private void ContinueOrExitStory(InputAction.CallbackContext ctx)
        {
            if (DialogueIsTyping)
            {
                OnDisplayDialogue.Raise(new DialogueInfo());
                return;
            }
            
            if (story.currentChoices.Count > 0 && currentChoiceIndex != -1)
            {
                story.ChooseChoiceIndex(currentChoiceIndex);
                currentChoiceIndex = -1;
            }
            
            if(story.canContinue)
            {
                string dialogueLine = story.Continue(); //calling continue first is important in here if you want to get the correct execution order

                while (IsLineBlank(dialogueLine) && story.canContinue)
                {
                    dialogueLine = story.Continue();
                }
                if (IsLineBlank(dialogueLine) && !story.canContinue)
                {
                    ExitDialogue();
                }
                else
                {
                    HandleTags(story.currentTags);
                    DialogueIsTyping = true;
                    OnDisplayDialogue.Raise(new DialogueInfo()
                    {
                        speaker = currentSpeaker,
                        line = dialogueLine,
                        choices = story.currentChoices
                    });
                }
            }
            else if(story.currentChoices.Count == 0)
            {
                ExitDialogue();
            }
        }

        public void UpdateChoiceIndex(int index) => currentChoiceIndex = index;
        public void UpdateInkDialogueVariable(string name, Ink.Runtime.Object value) => inkDialogueVariables.UpdateVariableState(name, value);
        private bool IsLineBlank(string dialogueLine) => dialogueLine.Trim().Equals("") || dialogueLine.Trim().Equals("\n");
        
        private void HandleTags(List<string> tags)
        {
            // loop through each tag and handle it accordingly
            foreach (string tag in tags) 
            {
                // parse the tag
                string[] splitTag = tag.Split(':');
                if (splitTag.Length != 2) 
                {
                    Debug.LogError("Tag could not be appropriately parsed: " + tag);
                }
                string tagKey = splitTag[0].Trim();
                string tagValue = splitTag[1].Trim();
            
                // handle the tag
                switch (tagKey) 
                {
                    case SPEAKER_TAG:
                        currentSpeaker = tagValue;
                        //displayNameText.text = tagValue;
                        break;
                    case PORTRAIT_TAG:
                        //portraitAnimator.Play(tagValue);
                        break;
                    case LAYOUT_TAG:
                        layoutAnimator.Play(tagValue);
                        break;
                    default:
                        Debug.LogWarning("Tag came in but is not currently being handled: " + tag);
                        break;
                }
            }
        }
    }
}