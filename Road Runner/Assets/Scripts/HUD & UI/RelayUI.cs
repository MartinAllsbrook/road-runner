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
using QFSW.QC;
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

    // The standard server creation and joining methods are below, but they only work with terrain generation, not so good for testing.
    #region Relay With Terrain

    private async void CreateRelay(string worldSeedInput)
    {
        try
        {
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(2); // Wait for allocation to be created

            string joinRelayCode = await Relay.Instance.GetJoinCodeAsync(allocation.AllocationId); // Get the join code for the allocation

            DisplayJoinCode(joinRelayCode);

            RelayServerData relayServerData = new RelayServerData(allocation, "dtls"); // Create the relay server data using "Datagram Transport Layer Security" as the transport protocol
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData); // Set the relay server data (what we just created) in the UnityTransport component
            NetworkManager.Singleton.StartHost();

            int seed = int.Parse(worldSeedInput);
            terrainManager.Set(seed, gameObject); // Send the seed and a reference to this UI to the TerrainManager so it can generate the terrain and disable this UI when it is done.

            gameObject.SetActive(false); // I'm not sure why this is here, when it is also done in the TerrainManager. I vaguely remember it fixing something.
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
            JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinRelayCode); // Wait for allocation associated with the join code to be found and retrieved

            DisplayJoinCode(joinRelayCode);

            RelayServerData relayServerData = new RelayServerData(joinAllocation, "dtls"); // Create the relay server data using "Datagram Transport Layer Security" as the transport protocol
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData); // Set the relay server data (what we just created) in the UnityTransport component
            NetworkManager.Singleton.StartClient();

            terrainManager.Set(gameObject);

            gameObject.SetActive(false);
        }
        catch (RelayServiceException e)
        {
            Debug.Log(e);
        }
    }

    #endregion

    // The below methods are for creating a testing without terrain generation, they should only be used for debugging.
    #region Relay Without Terrain (Debug & Testing Only)

    [Command("CreateRelayDebug")]
    private async void CreateRelayDebug()
    {
        try
        {
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(2); // Wait for allocation to be created

            string joinRelayCode = await Relay.Instance.GetJoinCodeAsync(allocation.AllocationId); // Get the join code for the allocation

            DisplayJoinCode(joinRelayCode);

            RelayServerData relayServerData = new RelayServerData(allocation, "dtls"); // Create the relay server data using "Datagram Transport Layer Security" as the transport protocol
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData); // Set the relay server data (what we just created) in the UnityTransport component
            NetworkManager.Singleton.StartHost();

            gameObject.SetActive(false);
        }
        catch (RelayServiceException e)
        {
            Debug.Log(e);
        }
    }

    [Command("JoinRelayDebug")]
    private async void JoinRelayDebug(string joinRelayCode)
    {
        try
        {
            JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinRelayCode); // Wait for allocation associated with the join code to be found and retrieved

            DisplayJoinCode(joinRelayCode);

            RelayServerData relayServerData = new RelayServerData(joinAllocation, "dtls"); // Create the relay server data using "Datagram Transport Layer Security" as the transport protocol
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData); // Set the relay server data (what we just created) in the UnityTransport component
            NetworkManager.Singleton.StartClient();

            gameObject.SetActive(false);
        }
        catch (RelayServiceException e)
        {
            Debug.Log(e);
        }
    }

    #endregion

    private void DisplayJoinCode(string joinRelayCode)
    {
        Debug.Log("Relay Code: " + joinRelayCode);
        serverCodeDisplay.text = joinRelayCode;
    }
}
