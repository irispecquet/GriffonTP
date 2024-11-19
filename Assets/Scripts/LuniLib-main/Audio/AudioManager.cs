using System;
using UnityEngine;

namespace Audio
{
    public class AudioManager : MonoBehaviour
    {
        public Sound[] Sounds;

        protected void Awake()
        {
            foreach (Sound s in Sounds)
            {
                s.Source = gameObject.AddComponent<AudioSource>();
                s.Source.clip = s.Clip;
                s.Source.volume = s.Volume;
                s.Source.pitch = s.Pitch;
                s.Source.loop = s.Loop;
            }
        }

        public void PlaySound(string name)
        {
            Sound s = Array.Find(Sounds, sound => sound.Name == name);
            if (s == null)
            {
                Debug.LogWarning("Sound : " + name + " not found");
                return;
            }
            s.Source.Play();
        }

        public void StopSound(string name)
        {
            Sound s = Array.Find(Sounds, sound => sound.Name == name);
            if (s == null)
            {
                Debug.LogWarning("Sound : " + name + " not found in StopSound");
                return;
            }
            s.Source.Stop();
        }
    }
}