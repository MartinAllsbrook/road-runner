using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LimboUI : MonoBehaviour
{
    // Spawning and Exiting Limbo are the same thing.
    // Limbo means the player doesn't exist in the world.

    [SerializeField] private Button spawnButton; 

    private void Start()
    {
        spawnButton.onClick.AddListener(Spawn);
    }

    private void Spawn()
    {
        PlayerSpawner.localPlayerSpawner.ExitLimbo();
    }
}
