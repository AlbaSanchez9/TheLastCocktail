using UnityEngine;

public class LauncherUI : MonoBehaviour
{
    [SerializeField] private Launcher launcher;

    public async void OnHostButtonPressed()
    {
        await launcher.StartAsHost();
    }

    public async void OnJoinButtonPressed()
    {
        await launcher.StartAsClientAuto();
    }
}