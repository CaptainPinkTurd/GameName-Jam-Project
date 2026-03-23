using CaptainPinkTurd.AudioSystem;
using CaptainPinkTurd.Core;
using UnityEngine;
using UnityEngine.EventSystems;

namespace CaptainPinkTurd.UI
{
    public abstract class ButtonDropdownBase : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        [Header("Button Dropdown Base Properties")]
        [SerializeField] protected SoundData hoverSfx;
        [SerializeField] protected SoundData buttonClickSfx;
        
        internal GameEvent<ButtonDropdownBase> onButtonHover = new GameEvent<ButtonDropdownBase>();
        internal bool isInactive;

        protected bool turnOffHoverStateWhenExit = true;
        
        private void OnEnable()
        {
            ButtonHoverEvent(false);
        }
        public void OnPointerEnter(PointerEventData eventData)
        {
            ButtonHoverEvent(true);
        }
        public void OnPointerExit(PointerEventData eventData)
        {
            if (!turnOffHoverStateWhenExit) return;
            
            ButtonHoverEvent(false);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            OnButtonClickEvent();
        }
        internal abstract void OnButtonClickEvent();
        internal abstract void ButtonHoverEvent(bool isHovering, bool playSfx = true);
    }
}
