using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using SimpleJSON;
using UnityEngine.UI;

public class PokeAPIController : MonoBehaviour
{
    public int pokemonID = 1;

    [Header("Buttom info")]
    public Text pokeNameText, pokeNumText;

    //Private strings
    string[] pokeTypeNames;
    string[] pokemonAbilities;
    string pokeName;
    string pokemonHeightString;
    string pokBaseXP;

    [Header("Pokemon Info")]
    public RawImage pokeRawImage;
    public Text[] pokeTypeTextArray;
    public Text[] pokemonAbilitiesTextArray;
    public Text pokeNameTextExpand;
    public Text pokemonIDExpand;
    public Text pokemonBaseXP;
    public Text pokemonHeightText;
   
    private readonly string basePokeURL = "https://pokeapi.co/api/v2/";
    UnityWebRequest pokeSpriteRequest, pokeInfoRequest; //Esta variable nos permite llamar los datos JSON
    Button myButton;

    private void Start()
    {
        CallGetPokemonAtIndex();
        myButton = GetComponent<Button>();
    }
    
    public void CallGetPokemonAtIndex()
    {
        StartCoroutine(GetPokemonAtIndex(pokemonID));
    }
    IEnumerator GetPokemonAtIndex(int pokemonIndex)
    {
        // Establecemos la URL correspondiente al identificador del pokemon asignado

        string pokemonURL = basePokeURL + "pokemon/" + pokemonIndex.ToString();
        // Example URL: https://pokeapi.co/api/v2/pokemon/151

        pokeInfoRequest = UnityWebRequest.Get(pokemonURL);

        yield return pokeInfoRequest.SendWebRequest();

        if (pokeInfoRequest.isNetworkError || pokeInfoRequest.isHttpError)//si al momento de actualizar la lista
        {                                                               
            pokeNameText.text = ""; //de elementos hay algun boton con un ID de pokemon inválido eliminamos los 
            pokeNumText.text = ""; //textos de dicho botón
            myButton.interactable = false;   //y pór último lo inabilitamos
            yield break;
        }

        JSONNode pokeInfo = JSON.Parse(pokeInfoRequest.downloadHandler.text);

        pokeName = pokeInfo["name"];
        string pokeSpriteURL = pokeInfo["sprites"]["front_default"];

        pokemonHeightString = pokeInfo["height"];
        pokBaseXP = pokeInfo["base_experience"];

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

        
        // Conseguimos el sprite del pokemon

        pokeSpriteRequest = UnityWebRequestTexture.GetTexture(pokeSpriteURL);

        yield return pokeSpriteRequest.SendWebRequest();

        if (pokeSpriteRequest.isNetworkError || pokeSpriteRequest.isHttpError)
        {
            Debug.LogError(pokeSpriteRequest.error);
            yield break;
        }

        // Establecemos los valores de la UI

        pokeNameText.text = pokeName;
        pokeNumText.text = "#" + pokemonID;
    }
    
    public void UpdatePokemonInfo()
    {
        pokeRawImage.texture = DownloadHandlerTexture.GetContent(pokeSpriteRequest);
        pokemonHeightText.text = pokemonHeightString;
        pokeNameTextExpand.text = pokeName;
        pokemonIDExpand.text = pokemonID.ToString();
        pokemonBaseXP.text = pokBaseXP;

        foreach (Text poketypes in pokeTypeTextArray)
        {
            poketypes.text = "";
        }

        foreach(Text pokeabilities in pokemonAbilitiesTextArray)
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

}
