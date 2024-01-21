using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LimboUI : MonoBehaviour
{
    // Spawning and Exiting Limbo are the same thing.
    // Limbo means the player doesn't exist in the world.

    [SerializeField] private Button spawnButton;

    [SerializeField] private Button[] selectCharacterButtons;

    private string debugTag = LogColors.GetColoredTag("[Limbo / Character UI]", LogColors.UIColor);

    private void Start()
    {
        spawnButton.onClick.AddListener(Spawn);

        for(int i = 0; i < selectCharacterButtons.Length; i++)
        {
            //int characterNumber = i;
            selectCharacterButtons[i].onClick.AddListener(() => OnSelectCharacterButtonPressed());
        }
    }

    private void OnSelectCharacterButtonPressed()
    {
        spawnButton.gameObject.SetActive(true);
    }

    private void OnEnable()
    {
        // Hoide the spawn button until the player has selected a character
        spawnButton.gameObject.SetActive(false); 
    }

    private void OnDisable()
    {
        Debug.Log(debugTag + "OnDisable");
    }

    private void Spawn()
    {
        Player.LocalInstance.ExitLimbo();
    }
}
