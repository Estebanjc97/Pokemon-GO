using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PokedexController : MonoBehaviour
{
    [SerializeField]
    public List<PokeAPIController> pokeAPIControllers;
    public string pokemonButtonsTag = "Pokemon";

    [Tooltip("Ingresa el número mayor que deseas agregar a la pokedex")]
    public int pokedexMaxValue = 800;

    [Tooltip("Númer de elementos que se visualizarán en pantalla")]
    public int elementsInTable = 10;
    private void Start()
    {
        GameObject[] pokemonButtons=GameObject.FindGameObjectsWithTag(pokemonButtonsTag); //creamos un array con los 10 botones que contienen la informacion de los pokemons

        for (int i = 0; i < pokemonButtons.Length; i++)
        {
            pokeAPIControllers.Add(pokemonButtons[i].GetComponent<PokeAPIController>()); //creamos una lista de las clases que contienen dichos botones
        }
        elementsInTable = pokemonButtons.Length;
    }
    public void NextPokemons()
    {
        foreach(PokeAPIController _pokeAPIController in pokeAPIControllers)
        {
            if(_pokeAPIController.pokemonID<=(pokedexMaxValue-elementsInTable)) //Verificamos que el identificador no vaya a superar el valor maximo de la pokedex luego de realizar el incremento
            {
                _pokeAPIController.pokemonID += 10;
            }
            else
            {
                _pokeAPIController.pokemonID -= (pokedexMaxValue - elementsInTable); //si hemos llegado al final de la pokedex y presionamos para ver la siguiente lista volveremos al inicio
            }
            _pokeAPIController.CallGetPokemonAtIndex();
        }
    }

    public void PreviousPokemons()
    {
        foreach (PokeAPIController _pokeAPIController in pokeAPIControllers)
        {
            if (_pokeAPIController.pokemonID >elementsInTable) //Verificamos que el identificador no vaya a superar el valor maximo de la pokedex luego de realizar el incremento
            {
                _pokeAPIController.pokemonID -= 10;
            }
            else
            {
                _pokeAPIController.pokemonID += (pokedexMaxValue - elementsInTable); //si hemos llegado al inicio de la pokedex y presionamos para ver la anterior lista iremos al final
            }
            _pokeAPIController.CallGetPokemonAtIndex();
        }
    }
}
