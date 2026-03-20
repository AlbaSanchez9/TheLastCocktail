using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using System.Threading.Tasks;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using System.Collections.Generic;

[RequireComponent(typeof(NetworkManager))]
[RequireComponent(typeof(UnityTransport))]
public class Launcher : MonoBehaviour
{
    [SerializeField] private string lobbySceneName = "LobbyScene";

    public int maxConnection = 20;
    public UnityTransport transport;

    private Lobby currentLobby;
    private float heartBeatTimer;

    private static bool instanceExists = false;

    async void Awake()
    {
        if (instanceExists)
        {
            Destroy(gameObject);
            return;
        }
        instanceExists = true;
        DontDestroyOnLoad(gameObject);

        await UnityServices.InitializeAsync();

        // Solo hace sign in si no está ya autenticado
        if (!AuthenticationService.Instance.IsSignedIn)
            await AuthenticationService.Instance.SignInAnonymouslyAsync();

        JoinOrCreated();
    }

    void Start()
    {
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
        NetworkManager.Singleton.OnServerStarted += HandleServerStarted;
    }

    public async void JoinOrCreated()
    {
        // Si ya está corriendo no hacer nada
        if (NetworkManager.Singleton.IsHost || NetworkManager.Singleton.IsClient)
        {
            Debug.Log("Ya conectado, saltando JoinOrCreated");
            return;
        }

        try
        {
            currentLobby = await LobbyService.Instance.QuickJoinLobbyAsync();
            string relayJoinCode = currentLobby.Data["JOIN_CODE"].Value;

            JoinAllocation allocation = await RelayService.Instance.JoinAllocationAsync(relayJoinCode);
            transport.SetClientRelayData(
                allocation.RelayServer.IpV4,
                (ushort)allocation.RelayServer.Port,
                allocation.AllocationIdBytes,
                allocation.Key,
                allocation.ConnectionData,
                allocation.HostConnectionData
            );

            NetworkManager.Singleton.StartClient();
        }
        catch
        {
            StartAsHost();
        }
    }

    public async void StartAsHost()
    {
        // Si ya está corriendo no hacer nada
        if (NetworkManager.Singleton.IsHost || NetworkManager.Singleton.IsClient)
        {
            Debug.Log("Ya conectado, saltando StartAsHost");
            return;
        }

        Allocation allocation = await RelayService.Instance.CreateAllocationAsync(maxConnection);
        string newJoinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

        Debug.Log($"Join Code: {newJoinCode}");

        transport.SetHostRelayData(
            allocation.RelayServer.IpV4,
            (ushort)allocation.RelayServer.Port,
            allocation.AllocationIdBytes,
            allocation.Key,
            allocation.ConnectionData
        );

        CreateLobbyOptions lobbyOptions = new CreateLobbyOptions
        {
            IsPrivate = false,
            Data = new Dictionary<string, DataObject>
        {
            { "JOIN_CODE", new DataObject(DataObject.VisibilityOptions.Public, newJoinCode) }
        }
        };

        currentLobby = await LobbyService.Instance.CreateLobbyAsync("Lobby", maxConnection, lobbyOptions);

        NetworkManager.Singleton.StartHost();
    }

    private void HandleServerStarted()
    {
        Debug.Log("Server iniciado, cargando LobbyScene...");
        NetworkManager.Singleton.SceneManager.LoadScene(
            lobbySceneName,
            LoadSceneMode.Single
        );
    }

    private void OnClientConnected(ulong clientId)
    {
        if (!NetworkManager.Singleton.IsServer) return;
        Debug.Log($"Cliente conectado: {clientId}");
    }

    void OnDestroy()
    {
        instanceExists = false;
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
            NetworkManager.Singleton.OnServerStarted -= HandleServerStarted;
        }
    }

    private void Update()
    {
        if (heartBeatTimer > 15)
        {
            heartBeatTimer -= 15;
            if (currentLobby != null && currentLobby.HostId == AuthenticationService.Instance.PlayerId)
                LobbyService.Instance.SendHeartbeatPingAsync(currentLobby.Id);
        }
        heartBeatTimer += Time.deltaTime;
    }

    private async void OnApplicationQuit()
    {
        if (currentLobby != null)
        {
            try { await LobbyService.Instance.DeleteLobbyAsync(currentLobby.Id); }
            catch { }
        }
    }
}