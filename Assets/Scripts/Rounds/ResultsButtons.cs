using Unity.Netcode;
using UnityEngine;

public class ResultsButtons : NetworkBehaviour
{
    public void OnRestartPressed()
    {
        RestartServerRpc();
    }

    public void OnQuitPressed()
    {
        QuitGameServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void QuitGameServerRpc()
    {
        // Vuelve al lobby directamente, sin pasar por ConnectionScene
        NetworkManager.Singleton.SceneManager.LoadScene(
            "LobbyScene",
            UnityEngine.SceneManagement.LoadSceneMode.Single
        );
    }

    [ServerRpc(RequireOwnership = false)]
    private void RestartServerRpc()
    {
        NetworkManager.Singleton.SceneManager.LoadScene(
            "BarScene",
            UnityEngine.SceneManagement.LoadSceneMode.Single
        );
    }
}