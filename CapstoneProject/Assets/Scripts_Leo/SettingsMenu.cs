using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.Audio;

public class SettingsMenu : MonoBehaviour
{
    [SerializeField] private AudioMixer mixer;
    [SerializeField] private Slider sliderMusicVol;
    [SerializeField] private Slider sliderSFXVol;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AdjustMusicVolume()
    {
        mixer.SetFloat(name: "MusicVol", sliderMusicVol.value);
    }

    public void AdjustSFXVolume()
    {
        mixer.SetFloat(name: "SFXVol", sliderSFXVol.value);
    }
}
