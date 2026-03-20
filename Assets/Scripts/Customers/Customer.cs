using System.Collections;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class Customer : NetworkBehaviour
{
    private enum CustomerState { Waiting, Drinking, FinishedDrink, Leaving }

    [Header("Location")]
    [SerializeField] private CustomerLocationType locationType;

    [Header("Patience")]
    [SerializeField] private float patience = 30f;
    [SerializeField] private float partialDeliveryPatienceBonus = 15f;
    [SerializeField] private Slider patienceBar;

    [Header("Drink")]
    [SerializeField] private float drinkTime = 6f;
    [SerializeField] private Transform barPoint;

    [Header("Economy")]
    [SerializeField] private float stealChance = 0.2f;
    [SerializeField] private GameObject moneyPrefab;
    [SerializeField] private float moneyDistanceFromClient = 0.3f;

    [Header("Order Config")]
    [SerializeField] private float drinkOnlyChance = 0.35f;
    [SerializeField] private float snackOnlyChance = 0.20f;

    private SnackType snackOrder;
    private bool snackFailedAlready = false;
    private bool wantsDrink = false;
    private bool wantsSnack = false;
    private bool drinkServed = false;
    private bool snackServed = false;
    private bool patienceBonusApplied = false;

    [SerializeField] private TextMeshProUGUI orderText;
    [SerializeField] private float moveSpeed = 2f;
    private Transform exitPoint;

    private NetworkVariable<float> currentPatience = new NetworkVariable<float>(
        0f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    private NetworkVariable<CustomerState> state = new NetworkVariable<CustomerState>(
        CustomerState.Waiting, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    private float drinkTimer;
    private Order order;
    private Glass servedGlass;
    private ulong servedGlassNetId = 0;
    private GameObject spawnedMoney;
    private CustomerManager manager;
    private Transform tableCenter;
    private bool hasTableCenter = false;

    public override void OnNetworkSpawn()
    {
        if (patienceBar != null)
        {
            patienceBar.maxValue = patience;
            patienceBar.value = currentPatience.Value;
            patienceBar.fillRect.GetComponent<Image>().color = Color.white;
        }

        currentPatience.OnValueChanged += (old, val) =>
        {
            if (patienceBar != null)
            {
                patienceBar.value = val;
                if (val < patience * 0.3f)
                    patienceBar.fillRect.GetComponent<Image>().color = Color.red;
                else
                    patienceBar.fillRect.GetComponent<Image>().color = Color.white;
            }
        };
    }

    private void Start()
    {
        if (!IsServer) return;

        float roll = Random.value;
        if (roll < drinkOnlyChance) { wantsDrink = true; wantsSnack = false; }
        else if (roll < drinkOnlyChance + snackOnlyChance) { wantsDrink = false; wantsSnack = true; }
        else { wantsDrink = true; wantsSnack = true; }

        currentPatience.Value = patience;
        state.Value = CustomerState.Waiting;

        if (patienceBar != null) patienceBar.maxValue = patience;

        UpdateOrderTextClientRpc(
            wantsDrink && order != null ? order.Recipe.CocktailName : "",
            wantsSnack ? snackOrder.ToString() : "",
            wantsDrink, wantsSnack
        );

        StartCoroutine(RotateToTableCenter());
    }

    private IEnumerator RotateToTableCenter()
    {
        yield return null; yield return null; yield return null;
        if (hasTableCenter && tableCenter != null)
        {
            Vector3 dir = tableCenter.position - transform.position;
            dir.y = 0;
            if (dir != Vector3.zero)
                transform.rotation = Quaternion.LookRotation(dir) * Quaternion.Euler(0, 90f, 0);
        }
    }

    [ClientRpc]
    private void UpdateOrderTextClientRpc(string cocktail, string snack, bool hasDrink, bool hasSnack)
    {
        if (orderText == null) return;
        if (hasDrink && hasSnack) orderText.text = $"Bebida: {cocktail}\nSnack: {snack}";
        else if (hasDrink) orderText.text = $"Bebida: {cocktail}";
        else if (hasSnack) orderText.text = $"Snack: {snack}";
    }

    public void SetOrder(Order newOrder) => order = newOrder;
    public void SetManager(CustomerManager m) => manager = m;
    public void SetExitPoint(Transform exit) => exitPoint = exit;
    public void SetSnackOrder(SnackType snack) => snackOrder = snack;
    public void SetTableCenter(Transform center) { tableCenter = center; hasTableCenter = true; }

    public void TryServeDrink(string cocktailName, Glass glass)
    {
        if (!IsServer) { TryServeDrinkRpc(cocktailName, glass.NetworkObjectId); return; }
        TryServeDrinkInternal(cocktailName, glass);
    }

    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    private void TryServeDrinkRpc(string cocktailName, ulong glassNetId)
    {
        if (NetworkManager.SpawnManager.SpawnedObjects.TryGetValue(glassNetId, out var netObj))
            TryServeDrinkInternal(cocktailName, netObj.GetComponent<Glass>());
    }

    private void TryServeDrinkInternal(string cocktailName, Glass glass)
    {
        if (!wantsDrink || drinkServed) return;
        if (state.Value != CustomerState.Waiting) return;

        if (order != null && cocktailName == order.Recipe.CocktailName)
        {
            GameManager.Instance.AddCorrectDrink();
            drinkServed = true;
            servedGlass = glass;
            servedGlassNetId = glass.NetworkObjectId; // ← guarda el id

            if (locationType == CustomerLocationType.Bar && barPoint != null)
            {
                glass.transform.position = barPoint.position;
                glass.LockGlass();
            }

            if (!wantsSnack || snackServed) StartDrinking();
            else ApplyPartialDeliveryBonus();
        }
        else
        {
            GameManager.Instance.AddWrongDrink();
        }
    }

    public bool TryServeSnack(SnackType deliveredSnack)
    {
        if (!IsServer) { TryServeSnackRpc(deliveredSnack); return false; }
        return TryServeSnackInternal(deliveredSnack);
    }

    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    private void TryServeSnackRpc(SnackType deliveredSnack) => TryServeSnackInternal(deliveredSnack);

    private bool TryServeSnackInternal(SnackType deliveredSnack)
    {
        if (!wantsSnack || snackServed) return false;
        if (deliveredSnack == snackOrder)
        {
            snackServed = true;
            GameManager.Instance.AddCorrectSnack();
            if (!wantsDrink || drinkServed) StartDrinking();
            else ApplyPartialDeliveryBonus();
            return true;
        }
        else
        {
            if (!snackFailedAlready) { GameManager.Instance.AddWrongSnack(); snackFailedAlready = true; }
            return false;
        }
    }

    private void ApplyPartialDeliveryBonus()
    {
        if (patienceBonusApplied) return;
        patienceBonusApplied = true;
        currentPatience.Value = Mathf.Min(currentPatience.Value + partialDeliveryPatienceBonus, patience);
    }

    private void StartDrinking() { drinkTimer = drinkTime; state.Value = CustomerState.Drinking; }

    private void Update()
    {
        if (!IsServer) return;
        if (!RoundManager.Instance.IsRoundActive()) return;

        switch (state.Value)
        {
            case CustomerState.Waiting: HandleWaiting(); break;
            case CustomerState.Drinking: HandleDrinking(); break;
            case CustomerState.FinishedDrink: CheckMoneyCollected(); break;
        }

        if (state.Value == CustomerState.FinishedDrink && spawnedMoney == null)
        {
            transform.position = Vector3.MoveTowards(
                transform.position, exitPoint.position, moveSpeed * Time.deltaTime);
        }
    }

    private void HandleWaiting()
    {
        currentPatience.Value -= Time.deltaTime;

        if (currentPatience.Value <= 0f)
        {
            bool receivedPartial = (wantsDrink && drinkServed) || (wantsSnack && snackServed);
            if (receivedPartial) SpawnMoney();
            else GameManager.Instance.ApplyLostCustomerPenalty(wantsDrink, wantsSnack, false, false);
            LeaveBar(false);
        }
    }

    private void HandleDrinking()
    {
        drinkTimer -= Time.deltaTime;
        if (drinkTimer <= 0) FinishDrink();
    }

    private void FinishDrink()
    {
        // Recupera el vaso por NetworkObjectId si servedGlass es null
        if (servedGlass == null && servedGlassNetId != 0)
        {
            if (NetworkManager.SpawnManager.SpawnedObjects.TryGetValue(servedGlassNetId, out var netObj))
                servedGlass = netObj.GetComponent<Glass>();
        }

        if (wantsDrink && servedGlass != null)
        {
            servedGlass.MakeDirty();
            servedGlass.UnlockGlass();
        }

        if (Random.value < stealChance)
        {
            GameManager.Instance.ApplyLostCustomerPenalty(wantsDrink, wantsSnack, drinkServed, snackServed);
            state.Value = CustomerState.FinishedDrink;
            LeaveBar(true);
            return;
        }

        SpawnMoney();
        state.Value = CustomerState.FinishedDrink;
    }

    private void SpawnMoney()
    {
        if (moneyPrefab == null) return;

        int amount = 0;
        if (drinkServed && snackServed) amount = GameManager.Instance.GetPricing().bothPrice;
        else if (drinkServed) amount = GameManager.Instance.GetPricing().drinkOnlyPrice;
        else if (snackServed) amount = GameManager.Instance.GetPricing().snackOnlyPrice;
        if (amount == 0) return;

        Vector3 spawnPos;
        if (locationType == CustomerLocationType.Bar && barPoint != null)
            spawnPos = barPoint.position + Vector3.up * 0.1f;
        else if (tableCenter != null)
        {
            Vector3 dir = (tableCenter.position - transform.position).normalized;
            dir.y = 0;
            spawnPos = transform.position + dir * moneyDistanceFromClient + Vector3.up * 0.1f;
        }
        else spawnPos = transform.position + Vector3.up * 0.1f;

        GameObject moneyObj = Instantiate(moneyPrefab, spawnPos, Quaternion.identity);
        moneyObj.GetComponent<NetworkObject>().Spawn();
        spawnedMoney = moneyObj;

        Money money = moneyObj.GetComponent<Money>();
        if (money != null) money.SetValue(amount);
    }

    private void CheckMoneyCollected()
    {
        if (spawnedMoney == null) LeaveBar(true);
    }

    private void LeaveBar(bool wasServed)
    {
        state.Value = CustomerState.Leaving;
        manager?.CustomerLeft(this);
        if (!wasServed)
        {
            RoundManager.Instance.AddLostCustomer();
            GameManager.Instance.ApplyLostCustomerPenalty(wantsDrink, wantsSnack, drinkServed, snackServed);
        }
        GetComponent<NetworkObject>().Despawn();
    }

    public string GetCocktailName() => (wantsDrink && order != null) ? order.Recipe.CocktailName : "-";
    public string GetSnackName() => wantsSnack ? snackOrder.ToString() : "-";
    public bool WantsDrink() => wantsDrink;
    public bool WantsSnack() => wantsSnack;
}