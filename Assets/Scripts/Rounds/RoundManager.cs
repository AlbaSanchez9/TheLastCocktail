using Unity.Netcode;
using UnityEngine;

public class RoundManager : NetworkBehaviour
{
    public static RoundManager Instance;

    [Header("Round Settings")]
    [SerializeField] private float roundDuration = 300f;

    private float timer;
    private bool roundActive = false;

    private void Awake()
    {
        Instance = this;
    }

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            StartRound();
        }
    }

    public void StartRound()
    {
        timer = roundDuration;
        roundActive = true;

        Debug.Log("RONDA INICIADA");
    }

    private void Update()
    {
        if (!IsServer || !roundActive) return;

        timer -= Time.deltaTime;

        if (timer <= 0)
        {
            EndRound();
        }
    }

    private void EndRound()
    {
        roundActive = false;

        Debug.Log("RONDA TERMINADA");

        ShowResultsClientRpc();
    }

    [ClientRpc]
    private void ShowResultsClientRpc()
    {
        UIResults.Instance.Show();
    }

    public bool IsRoundActive()
    {
        return roundActive;
    }
}