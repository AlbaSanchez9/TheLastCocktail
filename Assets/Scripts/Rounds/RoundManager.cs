using Unity.Netcode;
using UnityEngine;

public class RoundManager : NetworkBehaviour
{
    public static RoundManager Instance;

    [SerializeField] private float roundDuration = 300f;
    private bool roundActive = false;

    private NetworkVariable<float> timeRemaining = new NetworkVariable<float>(
        0f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    private NetworkVariable<int> lostCustomers = new NetworkVariable<int>(
        0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    private void Awake() => Instance = this;

    public override void OnNetworkSpawn()
    {
        if (IsServer) StartRound();
    }

    public void StartRound()
    {
        timeRemaining.Value = roundDuration;
        lostCustomers.Value = 0;
        roundActive = true;
    }

    private void Update()
    {
        if (!IsServer || !roundActive) return;
        timeRemaining.Value -= Time.deltaTime;
        if (timeRemaining.Value <= 0)
        {
            timeRemaining.Value = 0;
            EndRound();
        }
    }

    private void EndRound()
    {
        roundActive = false;
        ShowResultsClientRpc();
    }

    [ClientRpc]
    private void ShowResultsClientRpc() => UIResults.Instance.Show();

    public void AddLostCustomer()
    {
        if (!IsServer) return;
        lostCustomers.Value++;
    }

    public bool IsRoundActive() => roundActive;
    public float GetTimeRemaining() => timeRemaining.Value;
    public int GetLostCustomers() => lostCustomers.Value;
}