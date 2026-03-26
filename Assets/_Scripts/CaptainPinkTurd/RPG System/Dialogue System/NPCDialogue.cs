using System;
using CaptainPinkTurd.AudioSystem;
using CaptainPinkTurd.Core.CustomDataStructure;
using UnityEngine;

namespace CaptainPinkTurd.RPG.Dialogue
{
    [CreateAssetMenu(fileName = "New NPC Dialogue", menuName = "Scriptable Objects/RPG System/NPC Dialogue")]
    public class NPCDialogue : ScriptableObject
    {
        [Header("NPC Config")]
        [Tooltip("Portrait can be null if this npc doesn't have a portrait.")]
        public Sprite npcPortrait;
        
        [Header("Dialogue Config")]
        public SerializeKeyValuePair<DialogueLineSettings, string>[] dialogueLines;
        
        [Header("Voice Config")]
        public SoundData voiceSound;
    }

    [Serializable]
    public class DialogueLineSettings
    {
        public bool autoProgressToNextLine;
        public float autoProgressDelay = 1.5f;
        public float dialogueSpeed = 0.03f;
    }
}