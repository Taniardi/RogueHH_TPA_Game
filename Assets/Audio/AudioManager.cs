using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [SerializeField] public AudioSource BackgroundSource;
    [SerializeField] public AudioSource SFXSource;
    [SerializeField] public AudioSource ButtonSound;

    public AudioClip MainMenuBackground;
    public AudioClip UpgradeMenuBackground;
    public AudioClip DungeonBackground;
    public AudioClip CombatBackground;

    public AudioClip CamFire;
    public AudioClip ButtonHoverSFX;
    public AudioClip ButtonClickSFX;
    public AudioClip CheatCodeActivate;
    public AudioClip UpgradeCoinSound;
    public AudioClip FootStep;
    public AudioClip Punch;
    public AudioClip SwordSlash;
    public AudioClip EnemyAgro;
    public AudioClip Death;
    public AudioClip AuraSkill;

 
    void Start()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
 
    public void ChangeBackground(AudioClip clip)
    {
        BackgroundSource.clip = clip;
        BackgroundSource.loop = true;
        BackgroundSource.Play();
    }

    public void PlaySFX(AudioClip clip, bool isLoop = false)
    {
        SFXSource.clip = clip;
        SFXSource.loop = isLoop;  
        SFXSource.Play();
    }

    public void StopSFX()
    {
        SFXSource.Stop();
    }

    public void PlayButtonSound(AudioClip clip, bool isLoop = false)
    {
        ButtonSound.clip = clip;
        ButtonSound.loop = isLoop;
        ButtonSound.Play();
    }
}
