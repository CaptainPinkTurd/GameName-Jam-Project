using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

namespace CaptainPinkTurd.Core.Utilities
{
    public class TypewriterText : MonoBehaviour
    {
        [Header("Typewriter Configs")]
        public TMP_Text textUI;
        public float typingSpeed = 0.03f;
        
        [Header("Voice")]
        public AudioSource audioSource;
        public AudioClip[] voiceClips;
        public float minPitch = 0.9f;
        public float maxPitch = 1.1f;
        [Tooltip("Play sound every X characters")]
        public int soundFrequency = 1;

        private Coroutine typingCoroutine;
        private Action onTypingEnd;

        private bool isTyping;
        private string currentLine;
        
        public void StartTyping(string line, Action onTypingEnd = null)
        {
            currentLine = line;

            if (typingCoroutine != null)
            {
                StopCoroutine(typingCoroutine);
            }

            this.onTypingEnd = onTypingEnd;
            typingCoroutine = StartCoroutine(TypeText(line));
        }

        private IEnumerator TypeText(string line)
        {
            List<DialogueToken> tokens = ParseLine(line, out string cleanText);

            textUI.text = cleanText;
            textUI.maxVisibleCharacters = 0;

            isTyping = true;

            float currentSpeed = typingSpeed;
            int visibleCharIndex = 0;

            foreach (var token in tokens)
            {
                switch (token.type)
                {
                    case TypewriterTokenType.Character:

                        visibleCharIndex++;
                        textUI.maxVisibleCharacters = visibleCharIndex;

                        if (visibleCharIndex % soundFrequency == 0 &&
                            !char.IsWhiteSpace(token.character))
                        {
                            PlayVoice();
                        }

                        yield return new WaitForSeconds(currentSpeed);
                        break;

                    case TypewriterTokenType.Pause:
                        yield return new WaitForSeconds(token.value);
                        break;

                    case TypewriterTokenType.Speed:
                        currentSpeed = token.value;
                        break;
                }
            }

            onTypingEnd?.Invoke();
            isTyping = false;
        }
        private List<DialogueToken> ParseLine(string line, out string cleanText)
        {
            List<DialogueToken> tokens = new List<DialogueToken>();
            StringBuilder builder = new StringBuilder();

            for (int i = 0; i < line.Length; i++)
            {
                // Handle TMP rich text <color=red>Hello</color>
                if (line[i] == '<')
                {
                    int end = line.IndexOf('>', i);
                    if (end != -1)
                    {
                        string richTag = line.Substring(i, end - i + 1);
                        builder.Append(richTag); // keep in display
                        i = end;
                        continue;
                    }
                }

                // Handle custom tags [pause=...] / [speed=...]
                if (line[i] == '[')
                {
                    int end = line.IndexOf(']', i);

                    // Missing closing bracket → treat as normal text
                    if (end == -1)
                    {
                        Debug.LogError($"Missing closing bracket for tag in line: {line}, treat it as visible text instead");
                        builder.Append(line[i]);
                        tokens.Add(new DialogueToken { type = TypewriterTokenType.Character, character = line[i] });
                        continue;
                    }

                    string tag = line.Substring(i + 1, end - i - 1);

                    if (TryParseTag(tag, out DialogueToken token))
                    {
                        tokens.Add(token);
                    }
                    else
                    {
                        // Invalid tag → treat as visible text
                        Debug.LogError($"Invalid tag '{tag}' in line: {line}, treat it as visible text instead");
                        for (int j = i; j <= end; j++)
                        {
                            builder.Append(line[j]);
                            tokens.Add(new DialogueToken
                            {
                                type = TypewriterTokenType.Character,
                                character = line[j]
                            });
                        }
                    }

                    i = end;
                    continue;
                }

                // Normal character
                builder.Append(line[i]);
                tokens.Add(new DialogueToken
                {
                    type = TypewriterTokenType.Character,
                    character = line[i]
                });
            }

            cleanText = builder.ToString();
            return tokens;
        }
        private bool TryParseTag(string tag, out DialogueToken token)
        {
            token = null;

            string[] parts = tag.Split('=');
            if (parts.Length != 2) return false;

            string key = parts[0];
            string valueStr = parts[1];

            if (!float.TryParse(valueStr, NumberStyles.Float, CultureInfo.InvariantCulture, out float value))
                return false;

            switch (key)
            {
                case "pause":
                case "Pause":
                case "PAUSE":
                case "wait":
                case "Wait":
                case "WAIT":
                    token = new DialogueToken { type = TypewriterTokenType.Pause, value = value };
                    return true;

                case "speed":
                case "Speed":
                case "SPEED":
                    token = new DialogueToken { type = TypewriterTokenType.Speed, value = value };
                    return true;

                default:
                    return false;
            }
        }

        void PlayVoice()
        {
            if (voiceClips.Length == 0) return;

            var clip = voiceClips[Random.Range(0, voiceClips.Length)];
            audioSource.pitch = Random.Range(minPitch, maxPitch);
            audioSource.PlayOneShot(clip);
        }

        public void SkipTyping()
        {
            if (!isTyping) return;

            StopCoroutine(typingCoroutine);
            textUI.text = currentLine;
            onTypingEnd?.Invoke();
            isTyping = false;
        }
    }
    public enum TypewriterTokenType
    {
        Character,
        Pause,
        Speed
    }
    
    public class DialogueToken
    {
        public TypewriterTokenType type;
        public char character;
        public float value;
    }
}