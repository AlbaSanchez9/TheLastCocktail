using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class LobbyUIManager2Players : NetworkBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI player1Text;
    public TextMeshProUGUI player2Text;
    public Button readyButton;
    public Button startButton;

    [SerializeField] private string barSceneName = "BarScene";

    private Dictionary<ulong, int> playerSlot = new Dictionary<ulong, int>();
    private HashSet<ulong> readyPlayers = new HashSet<ulong>();

    void Start()
    {
        readyButton.onClick.AddListener(OnReadyClicked);
        startButton.onClick.AddListener(OnStartClicked);
        startButton.gameObject.SetActive(false);
        startButton.interactable = false;
    }

    public override void OnNetworkSpawn()
    {
        readyPlayers.Clear();
        playerSlot.Clear();
        InvokeRepeating(nameof(UpdatePlayers), 1f, 1f);
    }

    void UpdatePlayers()
    {
        var clients = new List<NetworkClient>(NetworkManager.Singleton.ConnectedClientsList);
        clients.Sort((a, b) => a.ClientId.CompareTo(b.ClientId));

        for (int i = 0; i < clients.Count && i < 2; i++)
        {
            ulong clientId = clients[i].ClientId;
            playerSlot[clientId] = i;
            string ready = readyPlayers.Contains(clientId) ? "READY" : "NOT READY";
            if (i == 0) player1Text.text = $"Player {clientId} - {ready}";
            if (i == 1) player2Text.text = $"Player {clientId} - {ready}";
        }

        CheckAllReady();
    }

    void OnReadyClicked() => SetReadyRpc();
    void OnStartClicked() => StartGameRpc();

    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    void SetReadyRpc(RpcParams rpcParams = default)
    {
        ulong clientId = rpcParams.Receive.SenderClientId;

        if (!readyPlayers.Contains(clientId))
            readyPlayers.Add(clientId);
        else
            readyPlayers.Remove(clientId);

        // Serializa ids como string separado por |
        UpdateReadyStateClientRpc(string.Join("|", readyPlayers));
    }

    [ClientRpc]
    void UpdateReadyStateClientRpc(string readyIdsJoined)
    {
        readyPlayers.Clear();
        if (readyIdsJoined.Length > 0)
        {
            foreach (var id in readyIdsJoined.Split('|'))
                if (ulong.TryParse(id, out ulong clientId))
                    readyPlayers.Add(clientId);
        }
        CheckAllReady();
    }

    void CheckAllReady()
    {
        int totalPlayers = NetworkManager.Singleton.ConnectedClients.Count;
        bool allReady = totalPlayers == 2 && readyPlayers.Count == 2;
        startButton.gameObject.SetActive(allReady);
        startButton.interactable = allReady && IsServer;
    }

    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    void StartGameRpc()
    {
        if (readyPlayers.Count == 2)
        {
            NetworkManager.Singleton.SceneManager.LoadScene(
                barSceneName,
                UnityEngine.SceneManagement.LoadSceneMode.Single
            );
        }
    }
}