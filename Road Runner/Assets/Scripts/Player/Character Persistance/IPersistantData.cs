using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPersistantData
{
    void LoadData(CharacterData allCharacterData);

    void SaveData(ref CharacterData allCharacterData);
}
