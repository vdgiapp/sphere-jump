using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviourSingleton<AudioManager>
{
    [Header("Audio Sources")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;

    [Header("Music Settings")]
    [SerializeField] private float fadeDuration = 0.2f;
    
    private AudioSingleton _audioSingleton;
    private Coroutine _fadeCoroutine;
    private int _currentMusicIndex = -1;

    protected override void OnSingletonAwake()
    {
        _audioSingleton = AudioSingleton.Instance;
        if (_audioSingleton == null)
        {
            Debug.LogError("[AudioManager] Audio Singleton Instance is null");
        }
    }

    // MUSIC

    public void PlayMusic(int index)
    {
        if (index < 0 || index >= _audioSingleton.musicClips.Count) return;
        if (_currentMusicIndex == index && musicSource.isPlaying) return;

        if (_fadeCoroutine != null) StopCoroutine(_fadeCoroutine);
        _fadeCoroutine = StartCoroutine(FadeToMusic(_audioSingleton.musicClips[index]));
        _currentMusicIndex = index;
    }

    public void RestartCurrentMusic()
    {
        if (_currentMusicIndex < 0 || _currentMusicIndex >= _audioSingleton.musicClips.Count) return;

        if (_fadeCoroutine != null) StopCoroutine(_fadeCoroutine);
        _fadeCoroutine = StartCoroutine(FadeToMusic(_audioSingleton.musicClips[_currentMusicIndex], restart: true));
    }

    private IEnumerator FadeToMusic(AudioClip newClip, bool restart = false)
    {
        var t = 0f;
        var startVolume = musicSource.volume;

        // Fade out
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            musicSource.volume = Mathf.Lerp(startVolume, 0f, t / fadeDuration);
            yield return null;
        }

        musicSource.Stop();
        musicSource.clip = newClip;
        musicSource.time = restart ? 0f : musicSource.time;
        musicSource.Play();

        // Fade in
        t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            musicSource.volume = Mathf.Lerp(0f, 1f, t / fadeDuration);
            yield return null;
        }

        musicSource.volume = 1f;
        _fadeCoroutine = null;
    }
    
    
    // SFX
    
    public void PlaySfx(int index)
    {
        if (index >= 0 && index < _audioSingleton.sfxClips.Count && _audioSingleton.sfxClips[index] != null)
        {
            sfxSource.PlayOneShot(_audioSingleton.sfxClips[index]);
        }
    }
    
    // MIXER CONTROL
    
    public void SetVolume(float musicVol, float sfxVol)
    {
        SetMixerVolume(_audioSingleton.musicVolumeParam, musicVol);
        SetMixerVolume(_audioSingleton.sfxVolumeParam, sfxVol);
    }
    
    public void SetMixerVolume(string parameter, float volume)
    {
        // Audio Mixer: dB -80 (silence) to 0 (loudest)
        var db = Mathf.Log10(Mathf.Clamp(volume, 0.0001f, 1f)) * 20f;
        _audioSingleton.audioMixer.SetFloat(parameter, db);
    }
    
    public void MuteMixerGroup(string parameter, bool mute)
    {
        _audioSingleton.audioMixer.SetFloat(parameter, mute ? -80f : 0f);
    }

    public void MuteAll(bool mute)
    {
        MuteMixerGroup(_audioSingleton.musicVolumeParam, mute);
        MuteMixerGroup(_audioSingleton.sfxVolumeParam, mute);
    }
}
