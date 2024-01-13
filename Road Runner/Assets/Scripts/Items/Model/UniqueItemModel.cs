using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Inventory;

public class UniqueItemModel : MonoBehaviour
{
    [SerializeField] private Transform[] modificationPoints;

    private UniqueItemModel[] _modificationModels;

    public void BuildModel(UniqueItemID uniqueItemID)
    {
        // Destroy any existing modification models

        if (_modificationModels != null)
        {
            foreach (UniqueItemModel modModel in _modificationModels)
            {
                // TODO: Make this better
                if (modModel != null)
                {
                    Destroy(modModel.gameObject);
                }
            }
        }

        int numModifications = uniqueItemID.Modifications.Length;
        if (numModifications != modificationPoints.Length)
        {
            Debug.LogError("UIID calls for " + numModifications + " mod points, this MP has " + modificationPoints.Length);
            return;
        }
    
        // Spawn the modification models
        _modificationModels = new UniqueItemModel[numModifications];
        for (int i = 0; i < uniqueItemID.Modifications.Length; i++)
        {
            UniqueItemID modUIID = uniqueItemID.Modifications[i];
            if (modUIID.BaseItemID == ItemID.Empty)
                continue;

            UniqueItemModel modModel = ItemSODictionary[modUIID.BaseItemID].ModelPrefab;

            Vector3 position = modificationPoints[i].position;
            Quaternion rotation = modificationPoints[i].rotation;

            _modificationModels[i] = Instantiate(modModel, position, rotation, transform);
            _modificationModels[i].BuildModel(modUIID);
        }
    }

}
