using CaptainPinkTurd.UI.Components;
using TMPro;
using UnityEngine;

namespace CaptainPinkTurd.RPG.Dialogue
{
    public class DialogueChoiceButton : ButtonBase
    {
        [Header("Dialogue Choice Button Properties")]
        [SerializeField] private TMP_Text choiceText;
        
        public override void OnButtonClickEvent()
        {
            
        }

        public override void ButtonHoverEvent(bool isHovering, bool playSfx = true)
        {
            
        }
    }
}