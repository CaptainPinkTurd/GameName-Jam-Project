using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace CaptainPinkTurd.InkDialogue
{
    public class DialogueChoiceButton : MonoBehaviour, ISelectHandler
    {
        [Header("Components")]
        [SerializeField] private Button button;
        [SerializeField] private TMP_Text choiceText;

        private int choiceIndex = -1;
        
        public Button Button => button;

        public void SetChoiceText(string choiceTextString)
        {
            choiceText.text = choiceTextString;
        }

        public void SetChoiceIndex(int choiceIndex)
        {
            this.choiceIndex = choiceIndex;
        }

        public void SelectButton()
        {
            button.Select();
        }

        public void OnSelect(BaseEventData eventData)
        {
            DialogueManager.Instance.UpdateChoiceIndex(choiceIndex);
        }
    }
}