using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class CharacterDataFileHandler
{
    private string dataDirectoryPath = "";
    private string genericDataFileName = "";
    private string dataFileType = "";
    private bool useEncryption = false;
    private readonly string encryptionKey = "Very Secret Key Phrase 123456789123456789 Bababooey";

    public CharacterDataFileHandler(string dataDirectoryPath, string genericDataFileName, string dataFileType, bool useEncryption)
    {
        this.dataDirectoryPath = dataDirectoryPath;
        this.genericDataFileName = genericDataFileName;
        this.dataFileType = dataFileType;
        this.useEncryption = useEncryption;
    }

    // Load character data from file, Returns null if no data found
    public CharacterData Load(string fileNameSuffix)
    {
        string fullFileName = CreateFileName(fileNameSuffix);
        string fullPath = Path.Combine(dataDirectoryPath, fullFileName);

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

                        // Copilot wanted to deserialize here
                    }
                }

                // Decrypt json if encryption is enabled
                if (useEncryption)
                {
                    jsonDataToLoad = EncryptDecrypt(jsonDataToLoad);
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

    public void Save(CharacterData characterData, string fileNameSuffix)
    {
        string fullFileName = CreateFileName(fileNameSuffix);
        string fullPath = Path.Combine(dataDirectoryPath, fullFileName);
        
        try
        {
            // Create directory if it doesn't exist
            if (!Directory.Exists(dataDirectoryPath))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(fullPath));
            }

            // Serialize character data to json
            string jsonDataToStore = JsonUtility.ToJson(characterData, true);

            // Encrypt json if encryption is enabled
            if (useEncryption)
            {
                jsonDataToStore = EncryptDecrypt(jsonDataToStore);
            }

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

    public void Delete(string fileNameSuffix)
    {
        string fullFileName = CreateFileName(fileNameSuffix);
        string fullPath = Path.Combine(dataDirectoryPath, fullFileName);

        try
        {
            // Delete file if it exists
            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to delete character data at " + fullPath + "\n" + e.Message);
        }
    }

    private string EncryptDecrypt(string input)
    {
        string output = "";
        for (int i = 0; i < input.Length; i++)
        {
            char character = input[i];
            character = (char)(character ^ encryptionKey[i % encryptionKey.Length]);
            output += character;
        }
        return output;
    }

    private string CreateFileName(string fileNameSuffix)
    {
        return genericDataFileName + fileNameSuffix + dataFileType;
    }
}
