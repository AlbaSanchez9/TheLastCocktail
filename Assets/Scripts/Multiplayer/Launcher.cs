using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Multiplayer;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(NetworkManager))]
[RequireComponent(typeof(UnityTransport))]
public class Launcher : MonoBehaviour
{
    [SerializeField] private string gameSceneName = "GameScene";
    private static bool instanceExists = false;
    private ISession currentSession;

    void Awake()
    {
        if (instanceExists)
        {
            Destroy(gameObject);
            return;
        }

        instanceExists = true;
        DontDestroyOnLoad(gameObject);
    }

    async void Start()
    {
        await InitializeServicesAsync();

        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
        NetworkManager.Singleton.OnServerStarted += HandleServerStarted;
    }

    private async Task InitializeServicesAsync()
    {
        await UnityServices.InitializeAsync();

        if (!AuthenticationService.Instance.IsSignedIn)
            await AuthenticationService.Instance.SignInAnonymouslyAsync();

        Debug.Log("Servicios inicializados. Player ID: " + AuthenticationService.Instance.PlayerId);
    }


    public async Task StartAsHost()
    {
        try
        {
            var options = new SessionOptions()
            {
                MaxPlayers = 4
            }.WithRelayNetwork();

            currentSession = await MultiplayerService.Instance.CreateSessionAsync(options);

            Debug.Log("Sesión creada. Código: " + currentSession.Code);

            NetworkManager.Singleton.StartHost();
        }
        catch (System.Exception e)
        {
            Debug.LogError("Error al crear sesión: " + e.Message);
        }
    }

    public async Task StartAsClientAuto()
    {
        try
        {
            var sessions = await MultiplayerService.Instance.QuerySessionsAsync(
                new QuerySessionsOptions()
            );

            if (sessions.Sessions.Count == 0)
            {
                Debug.LogError("No hay sesiones disponibles");
                return;
            }

            currentSession = await MultiplayerService.Instance.JoinSessionByIdAsync(
                sessions.Sessions[0].Id
            );

            Debug.Log("Unido automáticamente a sesión");

            NetworkManager.Singleton.StartClient();
        }
        catch (System.Exception e)
        {
            Debug.LogError("Error al unirse: " + e.Message);
        }
    }

    private void HandleServerStarted()
    {
        Debug.Log("Server iniciado");
    }

    private void OnClientConnected(ulong clientId)
    {
        if (NetworkManager.Singleton.IsServer)
        {
            Debug.Log("Cliente conectado: " + clientId);

            if (NetworkManager.Singleton.ConnectedClients.Count > 1)
            {
                if (SceneManager.GetActiveScene().name != gameSceneName)
                {
                    NetworkManager.Singleton.SceneManager.LoadScene(
                        gameSceneName,
                        LoadSceneMode.Single
                    );
                }
            }
        }
    }

    async void OnDestroy()
    {
        instanceExists = false;

        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
            NetworkManager.Singleton.OnServerStarted -= HandleServerStarted;
        }

        if (currentSession != null)
        {
            await currentSession.LeaveAsync();
            currentSession = null;
        }
    }
}