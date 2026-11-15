using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using E7.Introloop;

public class AudioManager : MonoBehaviour
{
    [SerializeField] List<AudioData> sfxList;

    [SerializeField] AudioSource templateSource;
    [SerializeField] IntroloopPlayer musicPlayer;
    [SerializeField] AudioSource sfxPlayer;

    IntroloopAudio currentMusic;

    Dictionary<AudioID, AudioData> sfxLookup;

    public static AudioManager i { get; private set; }

    private void Awake()
    {
        i = this;
        currentMusic = musicPlayer.DefaultIntroloopAudio;
        IntroloopPlayer.SetSingletonInstanceTemplateSource(templateSource);
    }

    private void Start()
    {
        sfxLookup = sfxList.ToDictionary(x => x.ID);
    }

    public void PlaySfx(AudioClip clip, bool pauseMusic = false)
    {
        if (clip == null) return;

        if (pauseMusic)
            StartCoroutine(UnPauseMusic(clip.length));

        sfxPlayer.PlayOneShot(clip);
    }

    public void PlaySfx(AudioID audioID, bool pauseMusic = false)
    {
        if (!sfxLookup.ContainsKey(audioID)) return;

        var audioData = sfxLookup[audioID];
        PlaySfx(audioData.clip, pauseMusic);
    }

    public void PlayMusic(IntroloopAudio clip, bool loop = true, bool fade = false)
    {
        if (clip == null || clip == currentMusic)
            return;

        musicPlayer.Stop();
        currentMusic = clip;
        
        StartCoroutine(PlayMusicAsync(clip));
    }

    IEnumerator PlayMusicAsync(IntroloopAudio clip)
    {
        if (clip != null)
            musicPlayer.DefaultIntroloopAudio = clip;

        musicPlayer.Play();

        yield return null;
    }

    IEnumerator UnPauseMusic(float delay)
    {
        musicPlayer.Pause();
        yield return new WaitForSeconds(delay);
        musicPlayer.Resume(0.15f);
    }
}

public enum AudioID { UISelect, StatusHit, NonStatusHit, Faint, EXPGain, ItemObtained, PokemonObtained, TMObtained, LedgeJump, HealPokemon, CutTree, PCOn, PCAccess, PCOff, TrainerExclaim, ShinySparkle }

[System.Serializable]
public class AudioData
{
    public AudioID ID;
    public AudioClip clip;
}