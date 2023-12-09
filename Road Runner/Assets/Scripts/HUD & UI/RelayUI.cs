using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Windows;

public class RelayUI : MonoBehaviour
{
    [SerializeField] private Button createServerButton;
    [SerializeField] private TMP_InputField woldSeedInput;

    [SerializeField] private Button joinServerButton;
    [SerializeField] private TMP_InputField serverCodeInput;

    [SerializeField] private TextMeshProUGUI serverCodeDisplay;

    [SerializeField] private TerrainManager terrainManager;

    [SerializeField] private HUDController hudController;


    private void Awake()
    {
        createServerButton.onClick.AddListener(() =>
        {
            CreateRelay(woldSeedInput.text);
        });
        
        joinServerButton.onClick.AddListener(() =>
        {
            JoinRelay(serverCodeInput.text);
        });
    }

    private async void Start()
    {
        await UnityServices.InitializeAsync();

        AuthenticationService.Instance.SignedIn += () =>
        {
            Debug.Log("Signed in " + AuthenticationService.Instance.PlayerId);
        };
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }

    private async void CreateRelay(string worldSeedInput)
    {
        try
        {
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(2);

            string joinRelayCode = await Relay.Instance.GetJoinCodeAsync(allocation.AllocationId);
            
            Debug.Log("Relay Code: " + joinRelayCode);
            serverCodeDisplay.text = joinRelayCode;
            
            RelayServerData relayServerData = new RelayServerData(allocation, "dtls");
            
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);

            NetworkManager.Singleton.StartHost();

            int seed = int.Parse(worldSeedInput);
            terrainManager.Set(seed, gameObject);

            gameObject.SetActive(false);
        }
        catch (RelayServiceException e)
        {
            Debug.Log(e);
        }
    }

    private async void JoinRelay(string joinRelayCode)
    {
        try
        {
            JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinRelayCode);

            Debug.Log("Relay Code: " + joinRelayCode);
            serverCodeDisplay.text = joinRelayCode;

            RelayServerData relayServerData = new RelayServerData(joinAllocation, "dtls");
            
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);

            NetworkManager.Singleton.StartClient();

            terrainManager.Set(gameObject);

            gameObject.SetActive(false);
        }
        catch (RelayServiceException e)
        {
            Debug.Log(e);
        }
    }
}
