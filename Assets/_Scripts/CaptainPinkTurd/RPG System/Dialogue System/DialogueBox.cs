using System;
using CaptainPinkTurd.Core.DesignPattern.SOAP.Events;
using TMPro;
using UnityEngine;

namespace CaptainPinkTurd.RPG.Dialogue
{
    public class DialogueBox : MonoBehaviour
    {
        [SerializeField] private TMP_Text dialogueText;
        [SerializeField] private float dialogueTextWidthWithPortrait = 460f;
        [SerializeField] private float dialogueTextWidthWithoutPortrait = 550f;
        [SerializeField] private BoolEvent onDialogueToggle;

        private void OnEnable()
        {
            onDialogueToggle.Raise(true);
        }

        private void OnDisable()
        {
            onDialogueToggle.Raise(false);
        }

        private void Start()
        {
            gameObject.SetActive(false);
        }

        public void OnPortraitToggle(bool isVisible)
        {
            dialogueText.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal,
                isVisible ? dialogueTextWidthWithPortrait : dialogueTextWidthWithoutPortrait);
        }
    }
}