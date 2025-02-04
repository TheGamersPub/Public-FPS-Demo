using UnityEngine;
using System;

[System.Serializable]
public enum AudioType
{
    None,
    Music,
    SFX,
    MenuSound,
    Voice
}

[System.Serializable]
public class Sound
{
    public string name;
    public bool loop;
    public bool playOnAwake;
    public AudioClip clip;
    public AudioType type;
    [HideInInspector]
    public AudioSource audioSource;
    [Range(0f, 3f)]
    public float volume = 1;
    [Range(.1f, 3f)]
    public float pitch = 1;

}

public class SoundSystem : MonoBehaviour
{
    public Sound[] sounds;

    public static SoundSystem Instance;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            return;

        foreach (Sound sound in sounds)
        {
            sound.audioSource = gameObject.AddComponent<AudioSource>();
            sound.audioSource.loop = sound.loop;
            sound.audioSource.clip = sound.clip;
            sound.audioSource.volume = sound.volume;
            sound.audioSource.pitch= sound.pitch;
        }
    }

    private void Start()
    {
        foreach (Sound sound in sounds)
        {
            if (sound.playOnAwake)
                Play(sound.name);
        }
    }

    public void Play(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);

        if (!s.Equals(default(Sound)))
            s.audioSource.Play();
        else
            Debug.LogError("Audio" + name + " not found");
    }
    
    public void PlayOneShot(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);

        if (!s.Equals(default(Sound)))
            s.audioSource.PlayOneShot(s.audioSource.clip);
        else
            Debug.LogError("Audio" + name + " not found");
    }

    public void ChangeAudioVolumeByType(AudioType type, float newVolume)
    {
        foreach (Sound sound in sounds)
        {
            if (sound.type == type)
            { 
                sound.audioSource.volume = (newVolume / 100) * sound.volume;
            }
        }
    }

}
