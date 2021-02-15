using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using SimpleJSON;
using UnityEngine.UI;

public class PokeAPIController : MonoBehaviour
{
    public int pokemonID = 1;
    public RawImage pokeRawImage;
    public Text pokeNameText, pokeNumText;
    string[] pokeTypeNames;
    public Text[] pokeTypeTextArray;
    [SerializeField]

    private readonly string basePokeURL = "https://pokeapi.co/api/v2/";
    UnityWebRequest pokeSpriteRequest, pokeInfoRequest;

    private void Start()
    {
        CallGetPokemonAtIndex();
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

        if (pokeInfoRequest.isNetworkError || pokeInfoRequest.isHttpError)
        {
            Debug.LogError(pokeInfoRequest.error);
            yield break;
        }

        JSONNode pokeInfo = JSON.Parse(pokeInfoRequest.downloadHandler.text);

        string pokeName = pokeInfo["name"];
        string pokeSpriteURL = pokeInfo["sprites"]["front_default"];

        JSONNode pokeTypes = pokeInfo["types"];
        pokeTypeNames = new string[pokeTypes.Count];

        for (int i = 0, j = pokeTypes.Count - 1; i < pokeTypes.Count; i++, j--)
        {
            pokeTypeNames[j] = pokeTypes[i]["type"]["name"];
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


        for (int i = 0; i < pokeTypeNames.Length; i++)
        {
            pokeTypeTextArray[i].text = pokeTypeNames[i];
        }
    }
}
