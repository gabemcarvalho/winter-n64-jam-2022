using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Audio;
using E7.Introloop;

public class AudioManager : MonoBehaviour
{
    public static Func<AudioManager> GetInstance;
    
    [SerializeField] private Sound[] sounds;

    private Dictionary<string, Sound> soundDictionary;

    private IntroloopPlayer introloopPlayer;
    [SerializeField] private IntroloopAudio overworldMusic;
    private float overworldMusicTime;
    [SerializeField] public IntroloopAudio winStinger;
    [SerializeField] public IntroloopAudio creditsMusic;

    private AudioSource defaultSource;

    public AudioManager GetSelf() => this;

    void Awake()
    {
        GetInstance = GetSelf;

        introloopPlayer = GetComponent<IntroloopPlayer>();
        defaultSource = GetComponent<AudioSource>();

        soundDictionary = new Dictionary<string, Sound>();

        foreach (Sound sound in sounds)
        {
            sound.source = gameObject.AddComponent<AudioSource>();
            sound.source.clip = sound.clip;
            sound.source.volume = sound.volume;
            sound.source.loop = sound.loop;
            sound.source.outputAudioMixerGroup = defaultSource.outputAudioMixerGroup;

            soundDictionary.Add(sound.name, sound);
        }

        overworldMusicTime = 0.0f;
    }

    private void OnDestroy()
    {
        if (GetInstance == GetSelf)
        {
            GetInstance = null;
        }
    }

    public void PlaySound(string name)
    {
        if (soundDictionary.TryGetValue(name, out Sound sound))
        {
            sound.source.Play();
        }
    }

    public void StopSound(string name)
    {
        if (soundDictionary.TryGetValue(name, out Sound sound))
        {
            sound.source.Stop();
        }
    }

    public void StopMusic(float fadeLengthSeconds)
    {
        introloopPlayer.Stop(fadeLengthSeconds);
    }

    public void PlayMusic(IntroloopAudio loopAudio)
    {
        introloopPlayer.Play(loopAudio);
    }

    public void ResumeOverworldMusic(float fadeLengthSeconds)
    {
        introloopPlayer.Play(overworldMusic, fadeLengthSeconds, overworldMusicTime);
    }

    public void StopOverworldMusic()
    {
        overworldMusicTime = introloopPlayer.GetPlayheadTime();
        StopMusic(0.0f);
    }

    public void PlayCreditsMusic()
    {
        overworldMusic = creditsMusic;
        overworldMusicTime = 0.0f;
        ResumeOverworldMusic(0.0f);
    }
}
