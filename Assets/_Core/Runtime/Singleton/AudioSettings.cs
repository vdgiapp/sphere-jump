using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[CreateAssetMenu(fileName = "AudioSingleton", menuName = "Scriptable Objects/Singleton/Audio Singleton", order = 0)]
public class AudioSingleton : ScriptableObjectSingleton<AudioSingleton>
{
    [Header("Audio Mixer")]
    public AudioMixer audioMixer;
    public string musicVolumeParam = "Music";
    public string sfxVolumeParam = "SFX";
    
    [Header("Audio Clips")]
    public List<AudioClip> musicClips;
    public List<AudioClip> sfxClips;
}