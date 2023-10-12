using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    public Sound[] Music;
    public Sound[] SoundFX;
    void Awake()
    {
        DontDestroyOnLoad(this);
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        foreach (var s in Music)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
        }
        foreach (var s in SoundFX)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
        }
    }
    public void PlayMusic(string name)
    {
        Sound s = Array.Find(Music, sound => sound.name == name);
        s.source.Play();
    }
    public void StopMusic(string name)
    {
        Sound s = Array.Find(Music, sound => sound.name == name);
        s.source.Stop();
    }
    public void StopAllMusic()
    {
        foreach (var s in Music)
        {
            s.source.Stop();
        }
    }
    public void PlaySoundFX(string name)
    {
        Sound s = Array.Find(SoundFX, sound => sound.name == name);
        s.source.Play();
    }
    public void StopSoundFX(string name)
    {
        Sound s = Array.Find(SoundFX, sound => sound.name == name);
        s.source.Stop();
    }
    public void StopAllSoundFX()
    {
        foreach (var s in SoundFX)
        {
            s.source.Stop();
        }
    }
}
