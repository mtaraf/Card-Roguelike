using System;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [Header("Audio Sources")]
    public AudioSource musicSource;
    public AudioSource sfxSource;

    [Header("Audio Clips")]
    public AudioClip backgroundMusic;
    public AudioClip clickSound;
    public AudioClip attackSound;
    public AudioClip damageSound;
    public AudioClip victorySound;
    public AudioClip healSound;
    public AudioClip buffSound;
    public AudioClip blockSound;
    public AudioClip debuffSound;
    public AudioClip goldSound;

    void Awake()
    {
        // Singleton pattern
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // Persist across scenes
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        playMusic(backgroundMusic);
    }

    public void playMusic(AudioClip clip)
    {
        if (musicSource && clip)
        {
            musicSource.clip = clip;
            musicSource.loop = true;
            musicSource.Play();
        }
    }

    public void playSFX(AudioClip clip)
    {
        if (sfxSource && clip)
        {
            sfxSource.PlayOneShot(clip);
        }
    }

    public void playHeal()
    {
        if (sfxSource)
        {
            sfxSource.PlayOneShot(healSound);
        }
    }

    public void playVictory()
    {
        if (sfxSource)
        {
            sfxSource.PlayOneShot(victorySound);
        }
    }
    
    public void playDamage()
    {
        if (sfxSource)
        {
            sfxSource.PlayOneShot(damageSound);
        }
    }

    public void playAttack()
    {
        if (sfxSource)
        {
            sfxSource.PlayOneShot(attackSound);
        }
    }

    public void playBuff()
    {
        if (sfxSource)
        {
            sfxSource.PlayOneShot(buffSound);
        }
    }

    public void playClick()
    {
        if (sfxSource)
        {
            sfxSource.PlayOneShot(clickSound);
        }
    }

    public void playBlock()
    {
        if (sfxSource)
        {
            sfxSource.PlayOneShot(blockSound);
        }
    }

    public void playGoldSound()
    {
        if (sfxSource)
        {
            sfxSource.PlayOneShot(goldSound);
        }
    }

    public void playDebuff()
    {
        if (sfxSource)
        {
            sfxSource.PlayOneShot(debuffSound);
        }
    }

    public void setBackgroundMusicVolume(float volume)
    {
        musicSource.volume = volume;
    }

    public void setSFXVolume(float volume)
    {
        sfxSource.volume = volume;
    }

    public float getBackgroundMusicVolume()
    {
        return musicSource.volume;
    }

    public float getSFXVolume()
    {
        return sfxSource.volume;
    }

    public Tuple<float,float> getAudioVolumes()
    {
        return new Tuple<float, float>(musicSource.volume, sfxSource.volume);
    }
}
