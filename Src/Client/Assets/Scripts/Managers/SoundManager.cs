using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : MonoSingleton<SoundManager>
{
    public AudioMixer audioMixer;

    public AudioSource musicAudioSource;
    public AudioSource soundAudioSource;

    const string MusicPath = "Music/";
    const string SoundPath = "Sound/";

    private bool musicOn;
    public bool MusicOn
    {
        get { return musicOn; }
        set
        {
            musicOn = value;
            this.MusicMute(!musicOn);//“Ù¿÷æ≤“Ù
        }
    }

    private bool soundOn;
    public bool SoundOn
    {
        get { return soundOn; }
        set
        {
            soundOn = value;
            this.SoundMute(!soundOn);//“Ù–ßæ≤“Ù
        }
    }

    private int musicVolume;
    public int MusicVolume
    {
        get { return musicVolume; }
        set
        {
            if (musicVolume != value)
            {
                musicVolume = value;
                if (musicOn) this.SetVolume("MusicVolume", musicVolume);
            }
        }
    }

    private int soundVolume;
    public int SoundVolume
    {
        get { return soundVolume; }
        set
        {
            if (soundVolume != value)
            {
                soundVolume = value;
                if (soundOn) this.SetVolume("SoundVolume", soundVolume);
            }
        }
    }

    private void Start()
    {
        this.musicVolume = Config.MusicVolume;
        this.soundVolume = Config.SoundVolume;
        this.musicOn = Config.MusicOn;
        this.soundOn = Config.SoundOn;
    }

    /// <summary>
    /// “Ù¿÷æ≤“Ù
    /// </summary>
    /// <param name="mute"></param>
    public void MusicMute(bool mute)
    {
        this.SetVolume("MusicVolume", mute ? 0 : musicVolume);
    }

    /// <summary>
    /// “Ù–ßæ≤“Ù
    /// </summary>
    /// <param name="mute"></param>
    public void SoundMute(bool mute)
    {
        this.SetVolume("SoundVolume", mute ? 0 : soundVolume);
    }

    private void SetVolume(string name, float value)
    {
        float volume = value * 0.5f - 50f;
        this.audioMixer.SetFloat(name, volume);
    }

    public void PlayMusic(string name)
    {
        AudioClip clip = Resloader.Load<AudioClip>(MusicPath + name);
        if (clip == null)
        {
            Debug.LogWarningFormat("Play Music : {0} Not existed", name);
            return;
        }
        if (musicAudioSource.isPlaying)
        {
            musicAudioSource.Stop();
        }

        musicAudioSource.clip = clip;
        musicAudioSource.Play();
    }

    public void PlaySound(string name)
    {
        AudioClip clip = Resloader.Load<AudioClip>(SoundPath + name);
        if (clip == null)
        {
            Debug.LogWarningFormat("Play Sound : {0} Not existed", name);
            return;
        }
        soundAudioSource.PlayOneShot(clip);
    }

}
