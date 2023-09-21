using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeSettings : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private Slider sfxPlayerSlider;
    [SerializeField] private Slider sfxBossSlider;

    void Start()
    {
        if (PlayerPrefs.HasKey("musicVolume") && PlayerPrefs.HasKey("sfxVolume") && PlayerPrefs.HasKey("sfxPlayerVolume") && PlayerPrefs.HasKey("sfxBossVolume"))
        {
            LoadMusicVolume();
            Debug.Log("VolumeSettings LoadMusicVolume");
        }
        else
        {
            SetMusicVolume();
            SetSFXVolume();
            SetSFXPlayerVolume();
            SetSFXBossVolume();
            Debug.Log("not VolumeSettings LoadMusicVolume");
        }
    }

    public void SetMusicVolume()
    {
        float volume = musicSlider.value;
        audioMixer.SetFloat("Music", Mathf.Log10(volume)*20);
        PlayerPrefs.SetFloat("musicVolume", volume);
    }

    public void SetSFXVolume()
    {
        float volume = sfxSlider.value;
        audioMixer.SetFloat("SFX", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("sfxVolume", volume);
    }

    public void SetSFXPlayerVolume()
    {
        float volume = sfxPlayerSlider.value;
        audioMixer.SetFloat("SFXPlayer", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("sfxPlayerVolume", volume);
    }

    public void SetSFXBossVolume()
    {
        float volume = sfxBossSlider.value;
        audioMixer.SetFloat("SFXBoss", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("sfxBossVolume", volume);
    }

    public void LoadMusicVolume()
    {
        musicSlider.value = PlayerPrefs.GetFloat("musicVolume");
        sfxSlider.value = PlayerPrefs.GetFloat("sfxVolume");
        sfxPlayerSlider.value = PlayerPrefs.GetFloat("sfxPlayerVolume");
        sfxBossSlider.value = PlayerPrefs.GetFloat("sfxBossVolume");
        SetMusicVolume();
        SetSFXVolume();
        SetSFXPlayerVolume();
        SetSFXBossVolume();
    }
}
