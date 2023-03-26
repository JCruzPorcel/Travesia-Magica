using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeController : MonoBehaviour
{
    [SerializeField] private AudioMixer mixer;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;

    private float musicVolume = .5f;
    private float sfxVolume = .5f;

    private const string MusicVolumeKey = "Music";
    private const string SFXVolumeKey = "SFX";

    private void Start()
    {
        if (PlayerPrefs.HasKey(MusicVolumeKey))
        {
            musicVolume = PlayerPrefs.GetFloat(MusicVolumeKey);
            musicSlider.value = musicVolume;
            mixer.SetFloat(MusicVolumeKey, Mathf.Log10(musicVolume) * 20);
        }

        if (PlayerPrefs.HasKey(SFXVolumeKey))
        {
            sfxVolume = PlayerPrefs.GetFloat(SFXVolumeKey);
            sfxSlider.value = sfxVolume;
            mixer.SetFloat(SFXVolumeKey, Mathf.Log10(sfxVolume) * 20);
        }

        musicSlider.onValueChanged.AddListener(SetMusicVolume);
        sfxSlider.onValueChanged.AddListener(SetSFXVolume);
    }

    public void SetMusicVolume(float volume)
    {
        musicVolume = volume;
        PlayerPrefs.SetFloat(MusicVolumeKey, musicVolume);
        PlayerPrefs.Save();
        mixer.SetFloat(MusicVolumeKey, Mathf.Log10(musicVolume) * 20);
    }

    public void SetSFXVolume(float volume)
    {
        sfxVolume = volume;
        PlayerPrefs.SetFloat(SFXVolumeKey, sfxVolume);
        PlayerPrefs.Save();
        mixer.SetFloat(SFXVolumeKey, Mathf.Log10(sfxVolume) * 20);
    }
}
