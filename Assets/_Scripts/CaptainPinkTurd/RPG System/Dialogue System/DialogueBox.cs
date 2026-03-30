using CaptainPinkTurd.Core.DesignPattern.Singleton;
using CaptainPinkTurd.Core.DesignPattern.SOAP.Events;
using CaptainPinkTurd.Core.Extensions;
using CaptainPinkTurd.Core.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace CaptainPinkTurd.RPG.Dialogue
{
    public class DialogueBox : Singleton<DialogueBox>
    {
        [Header("Dialogue Box UI Properties")]
        [SerializeField] private TMP_Text dialogueText;
        [SerializeField] private float dialogueTextWidthWithPortrait = 460f;
        [SerializeField] private float dialogueTextWidthWithoutPortrait = 550f;
        [SerializeField] private Image portraitImage;
        [SerializeField] private GameObject choiceBox;
        [SerializeField] private Transform choiceContainer;
        [SerializeField] private GameObject choiceButtonPrefab;
        
        [Header("Dialogue Box Events")]
        [SerializeField] private BoolEvent onDialogueBoxShow;
        
        private void Start()
        {
            ShowChoiceBox(false);
            ShowDialogueUI(false);
        }
        
        public void ShowDialogueUI(bool show)
        {
            foreach (var child in gameObject.transform.Children())
            {
                if (choiceBox == child.gameObject) continue;
                
                child.gameObject.SetActive(show);
            }
            
            onDialogueBoxShow.Raise(show);
        }

        public void SetNPCPortrait(Sprite sprite)
        {
            if (sprite)
            {
                portraitImage.gameObject.SetActive(true);
                portraitImage.sprite = sprite;
            }
            else
            {
                portraitImage.gameObject.SetActive(false);
            }
            OnPortraitToggle(portraitImage.gameObject.activeInHierarchy);
        }
        private void OnPortraitToggle(bool isVisible)
        {
            dialogueText.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal,
                isVisible ? dialogueTextWidthWithPortrait : dialogueTextWidthWithoutPortrait);
        }
        public void SetDialogueText(string text)
        {
            dialogueText.SetText(text);
        }

        public void ShowChoiceBox(bool show) => choiceBox.SetActive(show);
        public void ClearChoices()
        {
            foreach (Transform child in choiceContainer)
            {
                if (!child.gameObject.activeInHierarchy) continue;
                
                ObjectPoolManager.Instance.ReturnObjectToPool(child.gameObject);
            }
            
            ShowChoiceBox(false);
        }
        public void CreateChoiceButton(string choiceText, UnityAction onClick)
        {
            var choiceButton = ObjectPoolManager.Instance.SpawnObject(choiceButtonPrefab, choiceContainer);
            choiceButton.GetComponentInChildren<TMP_Text>().SetText(choiceText);
            choiceButton.GetComponent<Button>().onClick.RemoveAllListeners();
            choiceButton.GetComponent<Button>().onClick.AddListener(onClick);
        }
    }
}