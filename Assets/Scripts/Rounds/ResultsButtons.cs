using Unity.Netcode;
using UnityEngine;

public class ResultsButtons : NetworkBehaviour
{
    public void OnRestartPressed() => RoundCleaner.Instance?.CleanAndRestart();
    public void OnQuitPressed() => RoundCleaner.Instance?.CleanAndGoLobby();
}