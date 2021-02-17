/*==============================================================================
Copyright (c) 2019 PTC Inc. All Rights Reserved.

Copyright (c) 2010-2014 Qualcomm Connected Experiences, Inc.
All Rights Reserved.
Confidential and Proprietary - Protected under copyright and other laws.
==============================================================================*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;
using UnityEngine.UI;


/// <summary>
/// A custom handler that implements the ITrackableEventHandler interface.
///
/// Changes made to this file could be overwritten when upgrading the Vuforia version.
/// When implementing custom event handler behavior, consider inheriting from this class instead.
/// </summary>
public class TargetRecognition : MonoBehaviour, ITrackableEventHandler
{
    public List<GameObject> pokemons;
    public GameObject Canvas, pokeball;
    public Text pokemonDescription;
    int pokemonRandom;
    bool canCatch = false;
    public Transform pokeballSpawn;
    GameObject musicController;
    #region PROTECTED_MEMBER_VARIABLES

    protected TrackableBehaviour mTrackableBehaviour;
    protected TrackableBehaviour.Status m_PreviousStatus;
    protected TrackableBehaviour.Status m_NewStatus;

    #endregion // PROTECTED_MEMBER_VARIABLES

    #region UNITY_MONOBEHAVIOUR_METHODS

    protected virtual void Start()
    {
        mTrackableBehaviour = GetComponent<TrackableBehaviour>();
        if (mTrackableBehaviour)
            mTrackableBehaviour.RegisterTrackableEventHandler(this);
    }

    protected virtual void OnDestroy()
    {
        if (mTrackableBehaviour)
            mTrackableBehaviour.UnregisterTrackableEventHandler(this);
    }

    #endregion // UNITY_MONOBEHAVIOUR_METHODS

    #region PUBLIC_METHODS

    /// <summary>
    ///     Implementation of the ITrackableEventHandler function called when the
    ///     tracking state changes.
    /// </summary>
    public void OnTrackableStateChanged(
        TrackableBehaviour.Status previousStatus,
        TrackableBehaviour.Status newStatus)
    {
        m_PreviousStatus = previousStatus;
        m_NewStatus = newStatus;

        Debug.Log("Trackable " + mTrackableBehaviour.TrackableName +
                  " " + mTrackableBehaviour.CurrentStatus +
                  " -- " + mTrackableBehaviour.CurrentStatusInfo);

        if (newStatus == TrackableBehaviour.Status.DETECTED ||
            newStatus == TrackableBehaviour.Status.TRACKED ||
            newStatus == TrackableBehaviour.Status.EXTENDED_TRACKED)
        {
            OnTrackingFound();
        }
        else if (previousStatus == TrackableBehaviour.Status.TRACKED &&
                 newStatus == TrackableBehaviour.Status.NO_POSE)
        {
            OnTrackingLost();
        }
        else
        {
            // For combo of previousStatus=UNKNOWN + newStatus=UNKNOWN|NOT_FOUND
            // Vuforia is starting, but tracking has not been lost or found yet
            // Call OnTrackingLost() to hide the augmentations
            OnTrackingLost();
        }
    }

    #endregion // PUBLIC_METHODS

    #region PROTECTED_METHODS

    protected virtual void OnTrackingFound()
    {
        if (mTrackableBehaviour)
        {
            if(pokemons.Count>0) //verificamos que aún podamos atrapar pokemones disponibles
            {
                pokemonRandom = Random.Range(0, pokemons.Count); //mostramos un pokemon de la lista aleatoriamente
                pokemons[pokemonRandom].SetActive(true);
                pokemonDescription.text = "Ha aparecido un: " + pokemons[pokemonRandom].gameObject.name + " salvaje";
                Canvas.SetActive(true);
                canCatch = true; //permitimos lanzar la pokeball
                musicController = GameObject.FindGameObjectWithTag("MusicController");
                musicController.GetComponent<MusicController>().PlayPokemonAppears();
            }
            else
            {
                StartCoroutine(NoPokemonsAvailable()); //si no hay pokemones disponibles llamamos a la corroutina
            }
        }
    }

    IEnumerator NoPokemonsAvailable()
    {
        pokemonDescription.text = "No hay más pokemones en la zona, vuelve más tarde";
        musicController.GetComponent<MusicController>().PlayPokemonRun();
        Canvas.SetActive(true);
        canCatch = false;
        yield return new WaitForSeconds(1.8f);
        Canvas.SetActive(false);
    }
    protected virtual void OnTrackingLost()
    {
        if (mTrackableBehaviour)
        {
            if (pokemons.Count > 0) //si aun hay pokemones disponibles para atrapar, y se pierde el reconocimiento del marcador
            {           
                pokemons[pokemonRandom].SetActive(false); //desactivamos el pokemon
            }
            canCatch = false; //no podemos usar la pokeball
            Canvas.SetActive(false); //y desactivamos el canvas
            musicController.GetComponent<MusicController>().PlayPokemonRun();
        }
    }


    #endregion // PROTECTED_METHODS

    IEnumerator pokemonRun() //se llama cuando al lanzar la pokeball NO se atrapa al pokemon
    {
        float delay = 1.5f;
        pokemonDescription.text = "El pokemon ha huido";
        musicController.GetComponent<MusicController>().PlayPokemonRun();
        pokemons[pokemonRandom].SetActive(false);
        canCatch = false;
        yield return new WaitForSeconds(delay);
        Canvas.SetActive(false);
    }

    public void Catch()
    {
        StartCoroutine(ICatch());
    }
    IEnumerator ICatch()
    { 
        if(canCatch)
        {
            int randomCatch = Random.Range(0, 2); //50% de probabilidad entre atrapar o no atrapar al pokemon
            GameObject newPokeball= Instantiate(pokeball, pokeballSpawn.position, Quaternion.identity); //lanzamos una pokeball que tiene una animacion simple que se reproduce al instanciarse
            newPokeball.transform.SetParent(pokeballSpawn);

            float clipLenght = newPokeball.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).length; //conseguimos la duracion de la animación que tiene la pokebola

            yield return new WaitForSeconds(clipLenght); //esperamos hasta que haya terminado la animación

            if(randomCatch==0) //si atrapamos al pokemon
            {
                pokemonDescription.text = "¡Has atrapado este excelente pokemón!";
                musicController.GetComponent<MusicController>().PlayPokemonCaught();
                pokemons[pokemonRandom].SetActive(false);
                pokemons.RemoveAt(pokemonRandom); //lo eliminamos de la lista
                yield return new WaitForSeconds(1.5f); //y luego de un tiempo
                Canvas.SetActive(false); //eliminamos el canvas
            }
            else
            {
                StartCoroutine(pokemonRun());
            }
        }
    }
}

