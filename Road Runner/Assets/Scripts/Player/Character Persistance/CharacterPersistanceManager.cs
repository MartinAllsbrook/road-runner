using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CharacterPersistanceManager : MonoBehaviour
{
    public static CharacterPersistanceManager Instance { get; private set; }

    [Header("Data File Settings")]
    [SerializeField] private string fileName = "RR_CharacterData";
    [SerializeField] private string fileType = ".game";
    [SerializeField] private bool useEncryption = true;

    private CharacterDataFileHandler characterDataFileHandler;

    private CharacterData characterData;
    private List<IPersistantData> persistantDataObjects;

    private int loadedCharacterNumber = 0;

    // String to start debugs with
    private string debugTag = "<color=#ffff00ff>[CharacterPersistanceManager] </color>";

    private void Awake()
    {
        Debug.Log(debugTag + "Awake, Creating Singleton");
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogError(debugTag + "Another CPM already exists. Deleting new instance.");
            Destroy(this);
        }
    }

    private void Start()
    {
        Debug.Log(debugTag + "Start, Creating FileHandler");

        this.characterDataFileHandler = new CharacterDataFileHandler(Application.persistentDataPath, fileName, fileType, useEncryption);
    }

    public void FindAllAndLoad(int characterNumber)
    {
        this.persistantDataObjects = FindAllPersistantDataObjects();

        LoadCharacter(characterNumber);
    }

    public void LoadCharacter(int characterNumber)
    {
        Debug.Log(debugTag + "Loading character " + characterNumber);
        loadedCharacterNumber = characterNumber;

        string fileNameSuffix = characterNumber.ToString();
        this.characterData = characterDataFileHandler.Load(fileNameSuffix);

        // if these is no character data, create a new character
        if (characterData == null)
        {
            Debug.LogWarning(debugTag + "No character data found. Creating new character.");
            NewCharacter();
        }

        // Push data to all relevant systems
        foreach (IPersistantData persistantDataObject in persistantDataObjects)
        {
            Debug.Log(debugTag + "Loading data for " + persistantDataObject.GetType().ToString());
            persistantDataObject.LoadData(characterData);
        }
        
        Debug.Log(debugTag + "Loaded Character " + characterNumber);
    }

    public void NewCharacter()
    {
        characterData = new CharacterData();
    }

    public void SaveCharacter()
    {
        // Pull characterData from all relevant systems
        foreach (IPersistantData persistantDataObject in persistantDataObjects)
        {
            persistantDataObject.SaveData(ref characterData);
        }

        Debug.Log(debugTag + "Saving Character " + loadedCharacterNumber);

        // Save character data to CharacterDataFileHandler
        string fileNameSuffix = loadedCharacterNumber.ToString();
        characterDataFileHandler.Save(characterData, fileNameSuffix);
    }

    public void DeleteCharacter()
    {
        Debug.Log(debugTag + "Deleting Character " + loadedCharacterNumber);

        // Delete character data from CharacterDataFileHandler
        string fileNameSuffix = loadedCharacterNumber.ToString();
        characterDataFileHandler.Delete(fileNameSuffix);
    }

    public void DeleteCharacter(int characterNumber)
    {
        Debug.Log(debugTag + "Deleting Character " + characterNumber);

        // Delete character data from CharacterDataFileHandler
        string fileNameSuffix = characterNumber.ToString();
        characterDataFileHandler.Delete(fileNameSuffix);
    }

    private void OnDisable()
    {
        Debug.Log("Player Disable. Saving character.");
        // Semi temporary but I kind of like it so we'll see
        SaveCharacter();
    }

    private List<IPersistantData> FindAllPersistantDataObjects()
    {
        IEnumerable<IPersistantData> persistantDataObjects = FindObjectsOfType<MonoBehaviour>().OfType<IPersistantData>();

        return persistantDataObjects.ToList();
        // return new List<IPersistantData>(persistantDataObjects);
    }
}
