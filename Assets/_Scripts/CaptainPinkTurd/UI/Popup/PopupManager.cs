using System.Collections.Generic;
using CaptainPinkTurd.Core.CustomDataStructure;
using UnityEngine;
using CaptainPinkTurd.Core.DesignPattern.Singleton;
using CaptainPinkTurd.Core.Enum;
using CaptainPinkTurd.Core.Extensions;
using CaptainPinkTurd.Game;

namespace CaptainPinkTurd.UI.Popup
{
    public class PopupManager : Singleton<PopupManager>
    {
        [SerializeField] private SerializeKeyValuePair<PopupIdentifier, GameObject>[] popupDictionary;

        private readonly Stack<GameObject> popupInstances = new Stack<GameObject>();
        private bool canTriggerPopup = true;
        
        public bool CanTriggerPopup => canTriggerPopup;

        protected override void Awake()
        {
            base.Awake();

            canTriggerPopup = true;
            
            foreach (var popup in popupDictionary)
            {
                if(popup.Key.initOnAwake) popup.Value.SetActive(true);   
                popup.Value.SetActive(false);
            }
        }
        private void OnEnable()
        {
            GameManager.Instance.OnGameOver.Subscribe(OnGameOverEvents, EPriority.VeryLow);
        }
        private void OnDisable()
        {
            if (!gameObject.scene.isLoaded) return;
            
            GameManager.Instance.OnGameOver.Unsubscribe(OnGameOverEvents);
        }
        private void OnGameOverEvents()
        {
            canTriggerPopup = false;
        }
        
        public void ShowPopup(PopupIdentifier popupId, EPopupShowBehaviour behaviour = EPopupShowBehaviour.KeepPrevious)
        {
            if (!canTriggerPopup) return;
            
            if (popupDictionary.TryGetValue(popupId, out GameObject popupInstance))
            {
                popupInstance.SetActive(true);
                
                // If we should hide the previous panel, and we have one
                if (behaviour == EPopupShowBehaviour.HidePrevious && GetInstancePopupCount() > 0)
                {
                    // Get the last panel
                    var lastPopup = popupInstances.Pop();
                    lastPopup.SetActive(false);
                }
                
                // Add this new panel to the queue
                popupInstances.Push(popupInstance);
            }
            else
            {
                Debug.LogError($"No popup with {popupId} ID found in the pool");
            }
        }

        public void ClosePopup(PopupIdentifier popupId)
        {
            if (!canTriggerPopup) return;
        }
    
        public void CloseLastPopup()
        {
            if (!AnyPopupShowing() || !canTriggerPopup) return;
            
            var lastPopup = popupInstances.Pop();
            lastPopup.SetActive(false);
    
            // If we have more popups in the queue, reopen them
            if (GetInstancePopupCount() <= 0) return;
            
            lastPopup = popupInstances.Peek();
            if (lastPopup && !lastPopup.activeInHierarchy)
            {
                lastPopup.SetActive(true);
            }
        }
        public bool AnyPopupShowing()
        {
            return GetInstancePopupCount() > 0;
        }
        public int GetInstancePopupCount()
        {
            return popupInstances.Count;
        }
    }
}