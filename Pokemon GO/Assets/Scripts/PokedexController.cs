using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using SimpleJSON;

public class PokedexController : MonoBehaviour
{
    [SerializeField]
    public List<PokeAPIController> pokeAPIControllers;
    public string pokemonButtonsTag = "Pokemon";

    [Tooltip("Ingresa el número mayor que deseas agregar a la pokedex")]
    public int pokedexMaxValue = 800;

    [Tooltip("Númer de elementos que se visualizarán en pantalla")]
    public int elementsInTable = 10;
    int getPokeAPIController = 0; //variable para que la asignación de la clase solo se haga por primera vez

    UnityWebRequest pokemonInfo, pokeSpriteRequest;
    private readonly string basePokeURL = "https://pokeapi.co/api/v2/";
    public Text errorMessageText;
    string pokeName;
    string pokemonHeightString;
    string pokBaseXP;
    string pokeID;
    string[] pokeTypeNames;
    string[] pokemonAbilities;
    public Text[] pokeTypeTextArray;
    public Text[] pokemonAbilitiesTextArray;

    public Text pokemonHeightText;
    public Text pokeNameTextExpand;
    public Text pokemonIDExpand;
    public Text pokemonBaseXP;

    public RawImage pokeRawImage;

    private void Start()
    {
        StartCoroutine(TryGetPokemonByName("bulbasaur"));
    }
    public void SearchPokemon(InputField searchInputField)
    {
        StartCoroutine(TryGetPokemonByName(searchInputField.text));
    }
    IEnumerator TryGetPokemonByName(string name) //esta función es casi la misma que la corroutina que está en PokeAPIController con la diferencia que aqui se realiza la búsqueda por nombre y no por id
    {
        string pokemonURL = basePokeURL + "pokemon/" + name.ToLower();
        pokemonInfo = UnityWebRequest.Get(pokemonURL);
        yield return pokemonInfo.SendWebRequest();

        if (pokemonInfo.isNetworkError || pokemonInfo.isHttpError)
        {
            errorMessageText.text = "Pokemon no encontrado";
            yield break;
        }
        else
        {
            errorMessageText.text = "";
        }

        JSONNode pokeInfo = JSON.Parse(pokemonInfo.downloadHandler.text);

        pokeName = pokeInfo["name"];
        string pokeSpriteURL = pokeInfo["sprites"]["front_default"];

        pokemonHeightString = pokeInfo["height"];
        pokBaseXP = pokeInfo["base_experience"];
        pokeID = pokeInfo["id"];

        JSONNode pokeTypes = pokeInfo["types"];
        JSONNode pokeAbilities = pokeInfo["abilities"];

        pokeTypeNames = new string[pokeTypes.Count];
        pokemonAbilities = new string[pokeAbilities.Count];

        for (int i = 0, j = pokeTypes.Count - 1; i < pokeTypes.Count; i++, j--)
        {
            pokeTypeNames[j] = pokeTypes[i]["type"]["name"];
        }
        for (int i = 0, j = pokeAbilities.Count - 1; i < pokeAbilities.Count; i++, j--)
        {
            pokemonAbilities[j] = pokeAbilities[i]["ability"]["name"];
        }


        pokeSpriteRequest = UnityWebRequestTexture.GetTexture(pokeSpriteURL);

        yield return pokeSpriteRequest.SendWebRequest();

        if (pokeSpriteRequest.isNetworkError || pokeSpriteRequest.isHttpError)
        {
            Debug.LogError(pokeSpriteRequest.error);
            yield break;
        }


        UpdatePokemonInfo();
    }
    public void UpdatePokemonInfo()
    {
        pokeRawImage.texture = DownloadHandlerTexture.GetContent(pokeSpriteRequest);
        pokemonHeightText.text = pokemonHeightString;
        pokeNameTextExpand.text = pokeName;
        pokemonIDExpand.text = pokeID;
        pokemonBaseXP.text = pokBaseXP;

        foreach (Text poketypes in pokeTypeTextArray)
        {
            poketypes.text = "";
        }

        foreach (Text pokeabilities in pokemonAbilitiesTextArray)
        {
            pokeabilities.text = "x";
        }

        for (int i = 0; i < pokeTypeNames.Length; i++)
        {
            pokeTypeTextArray[i].text = pokeTypeNames[i];
        }
        for (int i = 0; i < pokemonAbilities.Length; i++)
        {
            pokemonAbilitiesTextArray[i].text = pokemonAbilities[i];
        }
    }
    public void GetPokeAPIControllers()
    {
        if(getPokeAPIController==0)
        {
            GameObject[] pokemonButtons = GameObject.FindGameObjectsWithTag(pokemonButtonsTag); //creamos un array con los 10 botones que contienen la informacion de los pokemons

            for (int i = 0; i < pokemonButtons.Length; i++)
            {
                pokeAPIControllers.Add(pokemonButtons[i].GetComponent<PokeAPIController>()); //creamos una lista de las clases que contienen dichos botones
            }
            elementsInTable = pokemonButtons.Length;
            getPokeAPIController = 1;
        }
        
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
