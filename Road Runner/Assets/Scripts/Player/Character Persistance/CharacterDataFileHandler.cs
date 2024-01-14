using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class CharacterDataFileHandler
{
    private string dataDirectoryPath = "";
    private string dataFileName = "";

    public CharacterDataFileHandler(string dataDirectoryPath, string dataFileName)
    {
        this.dataDirectoryPath = dataDirectoryPath;
        this.dataFileName = dataFileName;
    }

    // Load character data from file, Returns null if no data found
    public CharacterData Load()
    {
        // Create full path to file
        string fullPath = Path.Combine(dataDirectoryPath, dataFileName);

        CharacterData loadedCharacterData = null;
        if (File.Exists(fullPath))
        {
            try
            {
                // Read json from file
                string jsonDataToLoad = "";
                using (FileStream stream = new FileStream(fullPath, FileMode.Open))
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        jsonDataToLoad = reader.ReadToEnd();

                        // Copilot wated to deserialize here
                    }
                }

                // Deserialize json to character data
                loadedCharacterData = JsonUtility.FromJson<CharacterData>(jsonDataToLoad);
            }
            catch (Exception e)
            {
                Debug.LogError("Failed to load character data from " + fullPath + "\n" + e.Message);
            }
        }
        return loadedCharacterData;
    }

    public void Save(CharacterData characterData)
    {
        // Create full path to file
        string fullPath = Path.Combine(dataDirectoryPath, dataFileName);
        
        try
        {
            // Create directory if it doesn't exist
            if (!Directory.Exists(dataDirectoryPath))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(fullPath));
            }

            // Serialize character data to json
            string jsonDataToStore = JsonUtility.ToJson(characterData, true);

            // Write json to file
            using (FileStream stream = new FileStream(fullPath, FileMode.Create))
            {
                using (StreamWriter writer = new StreamWriter(stream))
                {
                    writer.Write(jsonDataToStore);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to save character data to " + fullPath + "\n" + e.Message);
        }
    }
}
