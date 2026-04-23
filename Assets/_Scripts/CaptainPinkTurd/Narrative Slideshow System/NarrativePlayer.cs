using System;
using CaptainPinkTurd.Core.Utilities;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CaptainPinkTurd.NarrativeSlideshow
{
    public class NarrativePlayer : MonoBehaviour
    {
        [SerializeField] private Image displayImage;
        [SerializeField] private TypewriterText  typewriterText;
        [SerializeField] private StorySequence sequence;
        
        private int currentIndex = 0;

        private void Start()
        {
            Play(sequence);
        }

        public void Play(StorySequence seq)
        {
            sequence = seq;
            currentIndex = 0;
            ShowPanel(true);
        }

        void ShowPanel(bool isFirst)
        {
            var panel = sequence.panels[currentIndex];
            displayImage.sprite = panel.image;

            displayImage.DOFade(1, isFirst ? 0f : panel.fadeTime).OnComplete(() =>
            {
                typewriterText.StartTyping(panel.text, Next);
            });
        }

        public void Next()
        {
            displayImage.DOFade(0, sequence.panels[currentIndex].fadeTime).OnComplete(() =>
            {
                currentIndex++;
                Debug.Log($"Showing next panel: {currentIndex}");

                if (currentIndex >= sequence.panels.Count) return;
                
                ShowPanel(false);
            });
        }
    }
}