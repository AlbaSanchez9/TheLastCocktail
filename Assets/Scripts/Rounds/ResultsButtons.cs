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
        QuitClientRpc();
    }

    [ClientRpc]
    private void QuitClientRpc()
    {
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
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