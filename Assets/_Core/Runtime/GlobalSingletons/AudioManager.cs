using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }
    
    [Header("Audio Mixer")]
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private string musicVolumeParam = "musicVolume";
    [SerializeField] private string sfxVolumeParam = "sfxVolume";
    
    [Header("Audio Sources")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;

    [Header("Audio Clips")]
    [SerializeField] private AudioClip[] musicClips;
    [SerializeField] private AudioClip[] sfxClips;
    
    [Header("Music Settings")]
    [SerializeField] private float fadeDuration = 0.3f;
    
    public int CurrentMusicIndex => _currentMusicIndex;
    public int MusicClipsLength => musicClips?.Length ?? 0;
    public int SfxClipsLength => sfxClips?.Length ?? 0;
    
    private Coroutine _fadeCoroutine;
    private int _currentMusicIndex = -1;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else if (Instance != this)
            Destroy(gameObject);
        
        if (audioMixer == null)
            Debug.LogError("[AudioManager] AudioMixer chưa được gán.");
        if (musicSource == null)
            Debug.LogError("[AudioManager] Music AudioSource chưa được gán.");
        if (sfxSource == null)
            Debug.LogError("[AudioManager] SFX AudioSource chưa được gán.");
    }

    // MUSIC

    // ReSharper disable Unity.PerformanceAnalysis
    public void PlayMusic(int index)
    {
        if (musicClips == null || musicClips.Length == 0)
        {
            Debug.LogWarning("[AudioManager] Mảng musicClips đang trống.");
            return;
        }
        if (index < 0 || index >= musicClips.Length)
        {
            Debug.LogWarning($"[AudioManager] Chỉ số {index} ngoài phạm vi musicClips.");
            return;
        }
        if (_currentMusicIndex == index && musicSource.isPlaying) return;
        if (_fadeCoroutine != null) StopCoroutine(_fadeCoroutine);
        _fadeCoroutine = StartCoroutine(FadeToMusic(musicClips[index]));
        _currentMusicIndex = index;
    }

    // ReSharper disable Unity.PerformanceAnalysis
    public void RestartCurrentMusic()
    {
        if (_currentMusicIndex < 0 || _currentMusicIndex >= musicClips.Length)
        {
            Debug.LogWarning("[AudioManager] Không có bài nhạc hiện tại để phát lại.");
            return;
        }
        if (_fadeCoroutine != null) StopCoroutine(_fadeCoroutine);
        _fadeCoroutine = StartCoroutine(FadeToMusic(musicClips[_currentMusicIndex], restart: true));
    }

    // ReSharper disable Unity.PerformanceAnalysis
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
        try
        {
            musicSource.Play();
        }
        catch (Exception e)
        {
            Debug.LogError($"[AudioManager] Lỗi khi phát nhạc: {e.Message}");
            yield break;
        }

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
    
    // ReSharper disable Unity.PerformanceAnalysis
    public void PlaySfx(int index)
    {
        if (sfxClips == null || sfxClips.Length == 0)
        {
            Debug.LogWarning("[AudioManager] Mảng sfxClips đang trống.");
            return;
        }
        if (index < 0 || index >= sfxClips.Length)
        {
            Debug.LogWarning($"[AudioManager] Chỉ số {index} ngoài phạm vi sfxClips.");
            return;
        }
        if (sfxClips[index] != null)
        {
            try
            {
                sfxSource.PlayOneShot(sfxClips[index]);
            }
            catch (Exception e)
            {
                Debug.LogError($"[AudioManager] Lỗi khi phát hiệu ứng âm thanh: {e.Message}");
            }
        }
    }
    
    // MIXER CONTROL
    
    public void SetVolume(float musicVol, float sfxVol)
    {
        SetMixerVolume(musicVolumeParam, musicVol);
        SetMixerVolume(sfxVolumeParam, sfxVol);
    }
    
    public void SetMixerVolume(string parameter, float volume)
    {
        if (string.IsNullOrEmpty(parameter))
        {
            Debug.LogWarning("[AudioManager] Tên parameter mixer không hợp lệ.");
            return;
        }
        // Audio Mixer: dB -80 (silence) to 0 (loudest)
        var clampedVol = Mathf.Clamp(volume, 0.0001f, 1f);
        var db = Mathf.Log10(clampedVol) * 20f;
        bool result = audioMixer.SetFloat(parameter, db);
        if (!result) Debug.LogWarning($"[AudioManager] Không thể thiết lập tham số '{parameter}' trong AudioMixer.");
    }
    
    public void MuteMixerGroup(string parameter, bool mute)
    {
        audioMixer.SetFloat(parameter, mute ? -80f : 0f);
    }

    public void MuteAll(bool mute)
    {
        MuteMixerGroup(musicVolumeParam, mute);
        MuteMixerGroup(sfxVolumeParam, mute);
    }
}