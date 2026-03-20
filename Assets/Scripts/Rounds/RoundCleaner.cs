using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class RoundCleaner : NetworkBehaviour
{
    public static RoundCleaner Instance;

    private void Awake() => Instance = this;

    public void CleanAndRestart()
    {
        if (!IsServer) { CleanAndRestartRpc(); return; }
        CleanAndRestartInternal();
    }

    public void CleanAndGoLobby()
    {
        if (!IsServer) { CleanAndGoLobbyRpc(); return; }
        CleanAndGoLobbyInternal();
    }

    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    private void CleanAndRestartRpc() => CleanAndRestartInternal();

    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    private void CleanAndGoLobbyRpc() => CleanAndGoLobbyInternal();

    private void CleanScene()
    {
        var toDestroy = new List<NetworkObject>();

        foreach (var netObj in NetworkManager.SpawnManager.SpawnedObjectsList)
        {
            if (netObj.GetComponent<GameManager>() != null) continue;
            if (netObj.GetComponent<RoundManager>() != null) continue;
            if (netObj.GetComponent<CustomerManager>() != null) continue;
            if (netObj.GetComponent<NetworkPlayer>() != null) continue;
            if (netObj.GetComponent<RoundCleaner>() != null) continue;
            toDestroy.Add(netObj);
        }

        foreach (var netObj in toDestroy)
        {
            if (netObj != null && netObj.IsSpawned)
                netObj.Despawn();
        }
    }

    private void CleanAndRestartInternal()
    {
        CleanScene();
        NetworkManager.Singleton.SceneManager.LoadScene(
            "BarScene",
            UnityEngine.SceneManagement.LoadSceneMode.Single
        );
    }

    private void CleanAndGoLobbyInternal()
    {
        CleanScene();
        NetworkManager.Singleton.SceneManager.LoadScene(
            "LobbyScene",
            UnityEngine.SceneManagement.LoadSceneMode.Single
        );
    }
}