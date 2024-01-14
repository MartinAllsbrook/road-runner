using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CharacterPersistanceManager : MonoBehaviour
{
    public static CharacterPersistanceManager Instance { get; private set; }

    [Header("Data File Settings")]
    [SerializeField] private string fileName = "RR_CharacterData.game";

    private CharacterDataFileHandler characterDataFileHandler;

    private CharacterData characterData;
    private List<IPersistantData> persistantDataObjects;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogError("CharacterPersistanceManager already exists. Deleting new instance.");
            Destroy(this);
        }
    }

    private void Start()
    {
        this.characterDataFileHandler = new CharacterDataFileHandler(Application.persistentDataPath, fileName);

        this.persistantDataObjects = FindAllPersistantDataObjects();

        LoadCharacter(); // Temporary
    }

    public void NewCharacter()
    {
        characterData = new CharacterData();
    }

    public void LoadCharacter()
    {
        // Load character data from CharacterDataFileHandler
        this.characterData = characterDataFileHandler.Load();

        // if these is no character data, create a new character
        if (characterData == null)
        {
            Debug.LogWarning("No character data found. Creating new character.");
            NewCharacter();
        }

        // Push data to all relevant systems
        foreach (IPersistantData persistantDataObject in persistantDataObjects)
        {
            persistantDataObject.LoadData(characterData);
        }
        
        Debug.Log("Loaded " + characterData.CName + " with " + characterData.Food + " health");
    }

    public void SaveCharacter()
    {
        // Pull characterData from all relevant systems
        foreach (IPersistantData persistantDataObject in persistantDataObjects)
        {
            persistantDataObject.SaveData(ref characterData);
        }

        Debug.Log("Saving " + characterData.CName + " with " + characterData.Food + " health");

        // Save character data to CharacterDataFileHandler
        characterDataFileHandler.Save(characterData);
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
