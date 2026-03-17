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
    [SerializeField] private string gameSceneName = "GameScene";
    private static bool instanceExists = false;

    public int maxConnection = 20;
    public UnityTransport transport;

    private Lobby currentLobby;
    private float heartBeatTimer;

    async void Awake()
    {
        // Si ya existe una instancia, destruir esta
        if (instanceExists)
        {
            Destroy(gameObject);
            return;
        }
        instanceExists = true;
        DontDestroyOnLoad(gameObject);

        await UnityServices.InitializeAsync();
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
        try
        {
            currentLobby = await LobbyService.Instance.QuickJoinLobbyAsync();
            string relayjoinCode = currentLobby.Data["JOIN_CODE"].Value;

            JoinAllocation allocation = await RelayService.Instance.JoinAllocationAsync(relayjoinCode);

            transport.SetClientRelayData(allocation.RelayServer.IpV4, (ushort)allocation.RelayServer.Port, allocation.AllocationIdBytes, allocation.Key, allocation.ConnectionData, allocation.HostConnectionData);

            NetworkManager.Singleton.StartClient();
        }
        catch
        {
            StartAsHost();
        }
    }

    public async void StartAsHost()
    {
        Allocation allocation = await RelayService.Instance.CreateAllocationAsync(maxConnection);
        string newJoinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

        Debug.Log(newJoinCode);

        transport.SetHostRelayData(allocation.RelayServer.IpV4, (ushort)allocation.RelayServer.Port, allocation.AllocationIdBytes, allocation.Key, allocation.ConnectionData);

        CreateLobbyOptions lobbyOptions = new CreateLobbyOptions();
        lobbyOptions.IsPrivate = false;
        lobbyOptions.Data = new Dictionary<string, DataObject>();
        DataObject dataObject = new DataObject(DataObject.VisibilityOptions.Public, newJoinCode);
        lobbyOptions.Data.Add("JOIN_CODE", dataObject);

        currentLobby = await LobbyService.Instance.CreateLobbyAsync("Lobby Name", maxConnection, lobbyOptions);

        NetworkManager.Singleton.StartHost();
    }

    public async void StartAsClient()
    {
        currentLobby = await LobbyService.Instance.QuickJoinLobbyAsync();
        string relayjoinCode = currentLobby.Data["JOIN_CODE"].Value;

        JoinAllocation allocation = await RelayService.Instance.JoinAllocationAsync(relayjoinCode);

        transport.SetClientRelayData(allocation.RelayServer.IpV4, (ushort)allocation.RelayServer.Port, allocation.AllocationIdBytes, allocation.Key, allocation.ConnectionData, allocation.HostConnectionData);

        NetworkManager.Singleton.StartClient();
    }

    private void HandleServerStarted()
    {
        Debug.Log("Server iniciado");
    }

    private void OnClientConnected(ulong clientId)
    {
        if (!NetworkManager.Singleton.IsServer) return;

        Debug.Log("Cliente conectado: " + clientId);

        //if (NetworkManager.Singleton.ConnectedClients.Count >= 2)
        //{
        //    NetworkManager.Singleton.SceneManager.LoadScene(
        //        gameSceneName,
        //        LoadSceneMode.Single
        //    );
        //}
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
            try
            {
                await LobbyService.Instance.DeleteLobbyAsync(currentLobby.Id);
            }
            catch { }
        }
    }
}