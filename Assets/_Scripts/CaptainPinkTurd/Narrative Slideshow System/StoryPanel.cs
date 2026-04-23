using System;
using UnityEngine;

namespace CaptainPinkTurd.NarrativeSlideshow
{
    [Serializable]
    public class StoryPanel
    {
        [SerializeField] internal Sprite image;
        [SerializeField] internal float fadeTime = .5f;
        [SerializeField][TextArea(3, 10)] internal string text;
    }
}