using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterCardUI : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI characterNameText;
    [SerializeField] private Button loadCharacterButton;
    [SerializeField] private Button deleteCharacterButton;

    [Header("File Saving Info")]
    [SerializeField] private int characterNumber = 0;

    private void Start()
    {
        loadCharacterButton.onClick.AddListener(OnLoadCharacterButtonClicked);
        deleteCharacterButton.onClick.AddListener(OnDeleteCharacterButtonClicked);
    }

    public void SetUI(string name)
    {
        characterNameText.text = name;
    }

    private void OnLoadCharacterButtonClicked()
    {
        CharacterPersistanceManager.Instance.FindAllAndLoad(characterNumber);
    }

    private void OnDeleteCharacterButtonClicked()
    {
        CharacterPersistanceManager.Instance.DeleteCharacter(characterNumber);
    }
}
