using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))] 
public class MusicController : MonoBehaviour
{
   
    public static MusicController instance;
    public AudioSource backgroundAudioSource, fxAudioSource;
    public AudioClip pokemonAppearsSound, pokemonCaughtSound, pokemonRunSound;
    private void Start()
    {
        if(instance==null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        PlayBackgroundMusic();
    }

    public void PlayBackgroundMusic()
    {
        backgroundAudioSource.Play();
    }
    public void PlayPokemonRun()
    {
        fxAudioSource.clip = pokemonRunSound;
        fxAudioSource.Play();
    }

    public void PlayPokemonCaught()
    {
        fxAudioSource.clip = pokemonCaughtSound;
        fxAudioSource.Play();
    }
    public void PlayPokemonAppears()
    {
        fxAudioSource.clip = pokemonAppearsSound;
        fxAudioSource.Play();
    }
}
