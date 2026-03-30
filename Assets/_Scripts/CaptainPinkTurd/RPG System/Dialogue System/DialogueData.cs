using System;
using CaptainPinkTurd.AudioSystem;
using CaptainPinkTurd.Core.CustomDataStructure;
using UnityEngine;

namespace CaptainPinkTurd.RPG.Dialogue
{
    [CreateAssetMenu(fileName = "New Dialogue Data", menuName = "Scriptable Objects/RPG System/Dialogue Data")]
    public class DialogueData : ScriptableObject
    {
        [Header("NPC Config")]
        [Tooltip("Portrait can be null if this npc doesn't have a portrait.")]
        public Sprite npcPortrait;
        
        [Header("Dialogue Config")]
        public DialogueLine[] dialogueLines;
        
        [Header("Voice Config")]
        public SoundData voiceSound;
    }

    [Serializable]
    public class DialogueLine
    {
        [TextArea(3, 10)]
        public string text;
        public bool autoProgressToNextLine;
        public float autoProgressDelay = 1f;
        public float dialogueSpeed = 0.03f;
        public SerializeKeyValuePair<string, int>[] branchingOptions;
        public bool endDialogue;
    }
}