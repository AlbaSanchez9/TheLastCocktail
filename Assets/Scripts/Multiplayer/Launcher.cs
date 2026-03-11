using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(NetworkManager))]
[RequireComponent(typeof(UnityTransport))]
public class Launcher : MonoBehaviour
{
    [SerializeField] private string gameSceneName = "GameScene";

    private static bool instanceExists = false;

    void Awake()
    {
        // Si ya existe una instancia, destruir esta
        if (instanceExists)
        {
            Destroy(gameObject);
            return;
        }

        instanceExists = true;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
        NetworkManager.Singleton.OnServerStarted += HandleServerStarted;
    }

    public void StartAsHost()
    {
        NetworkManager.Singleton.StartHost();
    }

    public void StartAsClient()
    {
        NetworkManager.Singleton.StartClient();
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

            if (SceneManager.GetActiveScene().name != gameSceneName)
            {
                NetworkManager.Singleton.SceneManager.LoadScene(
                    gameSceneName,
                    LoadSceneMode.Single
                );
            }
        }
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
}