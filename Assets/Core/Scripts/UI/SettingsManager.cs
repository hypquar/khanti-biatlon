using System.IO;
using UnityEditor.Overlays;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    [Header("Audio")]
    [SerializeField] private AudioMixer _masterAudioMixer;

    [SerializeField] private Slider _effectsSlider;
    [SerializeField] private Slider _musicSlider;

    public void SetQualityLevel(int qualityLevel)
    {
        QualitySettings.SetQualityLevel(qualityLevel);
    }

    public void ChangeEffectsVolume(float volume)
    {
        volume = _effectsSlider.value;
        ChangeAudioVolume("EffectsVolume", volume);
    }

    public void ChangeMusicVolume(float volume)
    {
        volume = _musicSlider.value;
        ChangeAudioVolume("MusicVolume", volume);
    }

    private void ChangeAudioVolume(string audioGroup, float volume)
    {
        _masterAudioMixer.SetFloat(audioGroup, volume);
    }

}