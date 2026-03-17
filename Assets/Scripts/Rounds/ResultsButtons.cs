using Unity.Netcode;
using UnityEngine;

public class ResultsButtons : NetworkBehaviour
{
    public void OnRestartPressed()
    {
        RestartServerRpc();
    }

    public void OnBackToLobbyPressed()
    {
        BackToLobbyServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void RestartServerRpc()
    {
        NetworkManager.Singleton.SceneManager.LoadScene(
            "BarScene",
            UnityEngine.SceneManagement.LoadSceneMode.Single
        );
    }

    [ServerRpc(RequireOwnership = false)]
    private void BackToLobbyServerRpc()
    {
        NetworkManager.Singleton.SceneManager.LoadScene(
            "MenuScene",
            UnityEngine.SceneManagement.LoadSceneMode.Single
        );
    }
}