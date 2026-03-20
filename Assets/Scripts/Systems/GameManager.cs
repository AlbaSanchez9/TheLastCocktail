using Unity.Netcode;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance;

    [SerializeField] private PricingConfig pricing;

    private NetworkVariable<int> money = new NetworkVariable<int>(
        0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    private NetworkVariable<int> correctDrinks = new NetworkVariable<int>(
        0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    private NetworkVariable<int> wrongDrinks = new NetworkVariable<int>(
        0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    private NetworkVariable<int> correctSnacks = new NetworkVariable<int>(
        0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    private NetworkVariable<int> wrongSnacks = new NetworkVariable<int>(
        0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    private void Awake() => Instance = this;

    private void Start()
    {
        if (!IsServer) return;
        money.Value = 0;
        correctDrinks.Value = 0;
        wrongDrinks.Value = 0;
        correctSnacks.Value = 0;
        wrongSnacks.Value = 0;
    }

    public void AddMoney(int amount)
    {
        if (!IsServer) { AddMoneyRpc(amount); return; }
        money.Value += amount;
    }

    public void ApplyLostCustomerPenalty(bool wantedDrink, bool wantedSnack, bool drinkServed, bool snackServed)
    {
        if (!IsServer) { ApplyPenaltyRpc(wantedDrink, wantedSnack, drinkServed, snackServed); return; }
        ApplyPenaltyInternal(wantedDrink, wantedSnack, drinkServed, snackServed);
    }

    private void ApplyPenaltyInternal(bool wantedDrink, bool wantedSnack, bool drinkServed, bool snackServed)
    {
        int penalty = 0;
        if (wantedDrink && wantedSnack)
        {
            if (drinkServed && !snackServed) penalty = pricing.partialDrinkServedPenalty;
            else if (!drinkServed && snackServed) penalty = pricing.partialSnackServedPenalty;
            else if (!drinkServed && !snackServed) penalty = pricing.lostBothPenalty;
        }
        else if (wantedDrink) penalty = drinkServed ? 0 : pricing.lostDrinkOnlyPenalty;
        else if (wantedSnack) penalty = snackServed ? 0 : pricing.lostSnackOnlyPenalty;

        money.Value = Mathf.Max(0, money.Value - penalty);
        Debug.Log($"Penalización: -${penalty} | Total: ${money.Value}");
    }

    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    private void AddMoneyRpc(int amount) => money.Value += amount;

    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    private void ApplyPenaltyRpc(bool wantedDrink, bool wantedSnack, bool drinkServed, bool snackServed)
        => ApplyPenaltyInternal(wantedDrink, wantedSnack, drinkServed, snackServed);

    public void AddCorrectDrink() { if (IsServer) correctDrinks.Value++; else AddCorrectDrinkRpc(); }
    public void AddWrongDrink() { if (IsServer) wrongDrinks.Value++; else AddWrongDrinkRpc(); }
    public void AddCorrectSnack() { if (IsServer) correctSnacks.Value++; else AddCorrectSnackRpc(); }
    public void AddWrongSnack() { if (IsServer) wrongSnacks.Value++; else AddWrongSnackRpc(); }

    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    private void AddCorrectDrinkRpc() => correctDrinks.Value++;
    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    private void AddWrongDrinkRpc() => wrongDrinks.Value++;
    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    private void AddCorrectSnackRpc() => correctSnacks.Value++;
    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    private void AddWrongSnackRpc() => wrongSnacks.Value++;

    public int GetMoney() => money.Value;
    public int GetCorrectDrinks() => correctDrinks.Value;
    public int GetWrongDrinks() => wrongDrinks.Value;
    public int GetCorrectSnacks() => correctSnacks.Value;
    public int GetWrongSnacks() => wrongSnacks.Value;
    public int GetTotalScore() => money.Value;
    public PricingConfig GetPricing() => pricing;
}