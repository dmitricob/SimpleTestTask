using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Core
{
    public class UnitySoundSystem : ISoundSystem
    {
        private Dictionary<string, AudioClip> _audioClips;
        private AudioSource _audioSource;

        public UnitySoundSystem(AudioSource audioSource, List<AudioClip> clips)
        {
            _audioClips = clips.ToDictionary(clip => clip.name);
            _audioSource = audioSource;
        }

        public void PlaySound(string soundName)
        {
            if (_audioClips.TryGetValue(soundName, out var clip))
            {
                _audioSource.PlayOneShot(clip);
            }
            else
            {
                Debug.LogWarning($"Sound '{soundName}' not found.");
            }
        }
    }

    public interface ISoundSystem
    {
        void PlaySound(string soundName);
        
    }

    public static class Const
    {
        public static class Sounds
        {
            public const string CardFlip = "CardFlip";
            public const string CardMatch = "CardMatch";
            public const string CardMismatch = "CardMismatch";
            public const string GameFinished = "GameFinished";
        }
    }
}